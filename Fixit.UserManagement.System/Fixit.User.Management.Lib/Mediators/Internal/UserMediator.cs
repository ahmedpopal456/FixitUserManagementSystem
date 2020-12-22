using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Database.DataContracts;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.User.Management.Lib.Models.Documents;
using Microsoft.Extensions.Configuration;

namespace Fixit.User.Management.Lib.Mediators.Internal
{
  public class UserMediator : IUserMediator
  {
    private readonly IMapper _mapper;
    private readonly IDatabaseTableEntityMediator _databaseUserTable;
    private readonly IConfiguration _configuration;

    public UserMediator(IMapper mapper,
                        IDatabaseMediator databaseMediator,
                        IConfiguration configurationProvider)
    {
      var databaseName = configurationProvider["FIXIT-UM-DB-NAME"];
      var databaseUserTableName = configurationProvider["FIXIT-UM-DB-USERTABLE"];
      _configuration = configurationProvider;

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

    #region UserAccountConfiguration
    #endregion

    #region UserProfileConfiguration
    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      UserProfileDto result = default(UserProfileDto);

      (DocumentCollectionDto<UserDocument> userDocumentCollection, string ContinuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, i => i.id == userId.ToString());
      if (userDocumentCollection != null)
      {
        if (userDocumentCollection.OperationException != null)
        {
          throw userDocumentCollection.OperationException;
        }
        if (userDocumentCollection.IsOperationSuccessful && userDocumentCollection.Results.Count != default(int))
        {
          result = _mapper.Map<UserDocument, UserProfileDto>(userDocumentCollection.Results[0]);
        }
      }
      return result;
    }

    public async Task<UserProfileInformationDto> UpdateUserProfileAsync(Guid userId, UserProfileUpdateRequestDto userProfileUpdateRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      UserProfileInformationDto result = default(UserProfileInformationDto);

      (DocumentCollectionDto<UserDocument> userDocumentCollection, string ContinuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, i => i.id == userId.ToString());
      if (userDocumentCollection != null)
      {
        if (userDocumentCollection.OperationException != null)
        {
          throw userDocumentCollection.OperationException;
        }
        if (userDocumentCollection.IsOperationSuccessful && userDocumentCollection.Results.Count != default(int))
        {
          UserDocument userDocument = userDocumentCollection.Results[0];
          userDocument.FirstName = userProfileUpdateRequestDto.FirstName;
          userDocument.LastName = userProfileUpdateRequestDto.LastName;
          userDocument.Address = userProfileUpdateRequestDto.Address;

          string partitionKey = userDocument.Role == UserRole.Client ? _configuration["FIXIT-UM-DB-CLIENTPK"] : _configuration["FIXIT-UM-DB-CRAFTSMANPK"];

          OperationStatus status = await _databaseUserTable.UpdateItemAsync(userDocument, partitionKey, cancellationToken);
          if (status.OperationException != null)
          {
            throw status.OperationException;
          }
          if (status.IsOperationSuccessful)
          {
            result = _mapper.Map<UserDocument, UserProfileInformationDto>(userDocument);
          }
        }
      }
      return result;
    }

    public async Task<UserProfilePictureDto> UpdateUserProfilePictureAsync(Guid userId, UserProfilePictureUpdateRequestDto userProfilePictureUpdateRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      UserProfilePictureDto result = default(UserProfilePictureDto);

      (DocumentCollectionDto<UserDocument> userDocumentCollection, string ContinuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, i => i.id == userId.ToString());
      if (userDocumentCollection != null)
      {
        if (userDocumentCollection.OperationException != null)
        {
          throw userDocumentCollection.OperationException;
        }
        if (userDocumentCollection.IsOperationSuccessful && userDocumentCollection.Results.Count != default(int))
        {
          UserDocument userDocument = userDocumentCollection.Results[0];
          userDocument.ProfilePictureUrl = userProfilePictureUpdateRequestDto.ProfilePictureUrl;

          string partitionKey = userDocument.Role == UserRole.Client ? _configuration["FIXIT-UM-DB-CLIENTPK"] : _configuration["FIXIT-UM-DB-CRAFTSMANPK"];

          OperationStatus status = await _databaseUserTable.UpdateItemAsync(userDocument, partitionKey, cancellationToken);
          if (status.OperationException != null)
          {
            throw status.OperationException;
          }
          if (status.IsOperationSuccessful)
          {
            result = _mapper.Map<UserDocument, UserProfilePictureDto>(userDocument);
          }
        }
      }
      return result;
    }
    #endregion
  }
}
