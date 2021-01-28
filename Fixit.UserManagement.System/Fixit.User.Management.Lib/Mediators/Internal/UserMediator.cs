﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Connectors.Mediators;
using Fixit.Core.DataContracts;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.User.Management.Lib.Models;
using Fixit.Core.DataContracts.Users.Enums;
using Microsoft.Extensions.Configuration;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.Connectors.DataContracts;

namespace Fixit.User.Management.Lib.Mediators.Internal
{
  public class UserMediator : IUserMediator
  {
    private readonly IMapper _mapper;
    private readonly IDatabaseTableEntityMediator _databaseUserTable;
    private readonly IMicrosoftGraphMediator _msGraphClient;

    public UserMediator(IMapper mapper,
                        IDatabaseMediator databaseMediator,
                        IMicrosoftGraphMediator msGraphMediator,
                        IConfiguration configurationProvider)
    {
      var databaseName = configurationProvider["FIXIT-UM-DB-NAME"];
      var databaseUserTableName = configurationProvider["FIXIT-UM-DB-USERTABLE"];

      if (string.IsNullOrWhiteSpace(databaseName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects the {nameof(configurationProvider)} to have defined the Fix Management Database as {{FIXIT-UM-USERDB}} ");
      }

      if (string.IsNullOrWhiteSpace(databaseUserTableName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects the {nameof(configurationProvider)} to have defined the Fix Management Table as {{FIXIT-UM-USERTABLE}} ");
      }

      if (databaseMediator == null)
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(databaseMediator)}... null argument was provided");
      }

      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _msGraphClient = msGraphMediator ?? throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(msGraphMediator)}... null argument was provided");
      _databaseUserTable = databaseMediator.GetDatabase(databaseName).GetContainer(databaseUserTableName);
    }

    public UserMediator(IMapper mapper,
                        IDatabaseMediator databaseMediator,
                        IMicrosoftGraphMediator msGraphMediator,
                        string databaseName,
                        string tableName)
    {

      if (string.IsNullOrWhiteSpace(databaseName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(databaseName)}... null argument was provided");
      }

      if (string.IsNullOrWhiteSpace(tableName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(tableName)}... null argument was provided");
      }

      if (databaseMediator == null)
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(databaseMediator)}... null argument was provided");
      }

      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _msGraphClient = msGraphMediator ?? throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(msGraphMediator)}... null argument was provided");
      _databaseUserTable = databaseMediator.GetDatabase(databaseName).GetContainer(tableName);
    }

    #region UserAccountConfiguration
    public async Task<UserAccountRoleResponseDto> CreateUserAsync(UserAccountCreateRequestDto userAccountCreateRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      UserAccountRoleResponseDto result = new UserAccountRoleResponseDto()
      {
        IsOperationSuccessful = false,
      };

      var (userDocumentCollection, token) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.UserPrincipalName == userAccountCreateRequestDto.UserPrincipalName);
      result.OperationException = userDocumentCollection.OperationException;
      if (userDocumentCollection != null && userDocumentCollection.IsOperationSuccessful)
      {
        UserDocument userDocument = userDocumentCollection.Results.SingleOrDefault();
        result = _mapper.Map<UserDocument, UserAccountRoleResponseDto>(userDocument);

        if (userDocument == null)
        {
          long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
          string partitionKey = userAccountCreateRequestDto.Role.ToString();

          var userDocumentToCreate = _mapper.Map<UserAccountCreateRequestDto, UserDocument>(userAccountCreateRequestDto);
          userDocumentToCreate.UpdatedTimestampsUtc = currentTime;
          userDocumentToCreate.CreatedTimestampsUtc = currentTime;

          CreateDocumentDto<UserDocument> createdUser = await _databaseUserTable.CreateItemAsync(userDocumentToCreate, partitionKey, cancellationToken);
          result = _mapper.Map<CreateDocumentDto<UserDocument>, UserAccountRoleResponseDto>(createdUser);
        }
      }

      return result;
    }

    public async Task<UserAccountStateDto> UpdateUserStatusAsync(Guid userId, UserAccountStateDto userAccountStateDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      UserAccountStateDto result = new UserAccountStateDto()
      {
        IsOperationSuccessful = false,
      };

      var (userDocumentCollection, token) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id.Equals(userId.ToString()));
      result.OperationException = userDocumentCollection.OperationException;
      if (userDocumentCollection != null && userDocumentCollection.IsOperationSuccessful)
      {
          UserDocument userDocumentToUpdate = userDocumentCollection.Results.SingleOrDefault();
        
          if (userDocumentToUpdate != null)
          { 
            result = _mapper.Map<UserDocument, UserAccountStateDto>(userDocumentToUpdate);
            bool shouldBlockSignIn = userAccountStateDto.State.Equals(UserState.Disabled);
            var userAdUpdateResponse = await _msGraphClient.UpdateAccountSignInStatusAsync(userDocumentToUpdate.UserPrincipalName, shouldBlockSignIn, cancellationToken);
            result = _mapper.Map<ConnectorDto<UserAccountStateDto>, UserAccountStateDto>(userAdUpdateResponse, result);

            if (result.IsOperationSuccessful)
            {
              userDocumentToUpdate.State = userAccountStateDto.State;
              userDocumentToUpdate.UpdatedTimestampsUtc = DateTimeOffset.Now.ToUnixTimeSeconds();

              string partitionKey = userDocumentToUpdate.Role.ToString();
              var databaseUpdateResponse = await _databaseUserTable.UpdateItemAsync(userDocumentToUpdate, partitionKey, cancellationToken);
              result = _mapper.Map<OperationStatus, UserAccountStateDto>(databaseUpdateResponse, result);

              if (result.IsOperationSuccessful)
              {
                result = _mapper.Map<UserDocument, UserAccountStateDto>(userDocumentToUpdate, result);
              }
            }
          }
      }
      return result;
    }

    public async Task<OperationStatus> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      OperationStatus result = new OperationStatus()
      {
        IsOperationSuccessful = false
      };

      var (userDocumentCollection, token) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id.Equals(userId.ToString()));
      result.OperationException = userDocumentCollection.OperationException;
      if (userDocumentCollection != null && userDocumentCollection.IsOperationSuccessful)
      {
        UserDocument userDocumentToDelete = userDocumentCollection.Results.SingleOrDefault();
        if (userDocumentToDelete != null)
        {
          string partitionKey = userDocumentToDelete.Role.ToString();
          result = await _msGraphClient.DeleteAccountAsync(userDocumentToDelete.UserPrincipalName, cancellationToken);

          if (result.IsOperationSuccessful)
          {
            result = await _databaseUserTable.DeleteItemAsync<UserDocument>(userId.ToString(), partitionKey, cancellationToken);
          }
        }
      }

      return result;
    }
    #endregion

    #region UserProfileConfiguration
    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      UserProfileDto result = default(UserProfileDto);

      var (userDocumentCollection, token) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id == userId.ToString());
      if (userDocumentCollection != null)
      {
        result = new UserProfileDto()
        {
          OperationException = userDocumentCollection.OperationException,
          OperationMessage = userDocumentCollection.OperationMessage
        };
        if (userDocumentCollection.IsOperationSuccessful)
        {
          UserDocument userDocument = userDocumentCollection.Results.SingleOrDefault();
          if (userDocument != null)
          {
            result = _mapper.Map<UserDocument, UserProfileDto>(userDocument);
            result.IsOperationSuccessful = true;
          }
        }
      }
      return result;
    }

    public async Task<UserProfileInformationDto> UpdateUserProfileAsync(Guid userId, UserProfileUpdateRequestDto userProfileUpdateRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      UserProfileInformationDto result = default(UserProfileInformationDto);

      var (userDocumentCollection, token) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id == userId.ToString());
      if (userDocumentCollection != null)
      {
        result = new UserProfileInformationDto()
        {
          OperationException = userDocumentCollection.OperationException,
          OperationMessage = userDocumentCollection.OperationMessage
        };
        if (userDocumentCollection.IsOperationSuccessful)
        {
          UserDocument userDocument = userDocumentCollection.Results.SingleOrDefault();
          if (userDocument != null)
          {
            string partitionKey = userDocument.Role.ToString();
            userDocument = _mapper.Map<UserProfileUpdateRequestDto, UserDocument>(userProfileUpdateRequestDto, userDocument);
            userDocument.UpdatedTimestampsUtc = DateTimeOffset.Now.ToUnixTimeSeconds();

            var operationStatus = await _databaseUserTable.UpdateItemAsync(userDocument, partitionKey, cancellationToken);
            result.OperationException = operationStatus.OperationException;
            result.OperationMessage = operationStatus.OperationMessage;

            if (operationStatus.IsOperationSuccessful)
            {
              result = _mapper.Map<UserDocument, UserProfileInformationDto>(userDocument);
              result.IsOperationSuccessful = true;
            }
          }
        }
      }
      return result;
    }

    public async Task<UserProfilePictureDto> UpdateUserProfilePictureAsync(Guid userId, UserProfilePictureUpdateRequestDto userProfilePictureUpdateRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      UserProfilePictureDto result = default(UserProfilePictureDto);

      var (userDocumentCollection, token) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id == userId.ToString());
      if (userDocumentCollection != null)
      {
        result = new UserProfilePictureDto()
        {
          OperationException = userDocumentCollection.OperationException,
          OperationMessage = userDocumentCollection.OperationMessage
        };
        if (userDocumentCollection.IsOperationSuccessful)
        {
          UserDocument userDocument = userDocumentCollection.Results.SingleOrDefault();
          if (userDocument != null)
          {
            string partitionKey = userDocument.Role.ToString();
            userDocument = _mapper.Map<UserProfilePictureUpdateRequestDto, UserDocument>(userProfilePictureUpdateRequestDto, userDocument);
            userDocument.UpdatedTimestampsUtc = DateTimeOffset.Now.ToUnixTimeSeconds();

            var operationStatus = await _databaseUserTable.UpdateItemAsync(userDocument, partitionKey, cancellationToken);
            result.OperationException = operationStatus.OperationException;
            result.OperationMessage = operationStatus.OperationMessage;

            if (operationStatus.IsOperationSuccessful)
            {
              result = _mapper.Map<UserDocument, UserProfilePictureDto>(userDocument);
              result.IsOperationSuccessful = true;
            }
          }
        }
      }
      return result;
    }

    #endregion
  }
}
