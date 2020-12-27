using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Database.DataContracts;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.User.Management.Lib.Models;
using Microsoft.Extensions.Configuration;

namespace Fixit.User.Management.Lib.Mediators.Internal
{
  public class UserMediator : IUserMediator
  {
    private readonly IMapper _mapper;
    private readonly IDatabaseTableEntityMediator _databaseUserTable;

    public UserMediator(IMapper mapper,
                        IDatabaseMediator databaseMediator,
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
      _databaseUserTable = databaseMediator.GetDatabase(databaseName).GetContainer(databaseUserTableName);
    }

    public UserMediator(IMapper mapper,
                        IDatabaseMediator databaseMediator,
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
      _databaseUserTable = databaseMediator.GetDatabase(databaseName).GetContainer(tableName);
    }

    #region UserAccountConfiguration
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
            userDocument = _mapper.Map<UserProfileUpdateRequestDto, UserDocument>(userProfileUpdateRequestDto, userDocument);
            userDocument.UpdatedTimestampsUtc = DateTimeOffset.Now.ToUnixTimeSeconds();

            var operationStatus = await _databaseUserTable.UpdateItemAsync(userDocument, userDocument.Role.ToString(), cancellationToken);
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
            userDocument = _mapper.Map<UserProfilePictureUpdateRequestDto, UserDocument>(userProfilePictureUpdateRequestDto, userDocument);
            userDocument.UpdatedTimestampsUtc = DateTimeOffset.Now.ToUnixTimeSeconds();

            var operationStatus = await _databaseUserTable.UpdateItemAsync(userDocument, userDocument.Role.ToString(), cancellationToken);
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
