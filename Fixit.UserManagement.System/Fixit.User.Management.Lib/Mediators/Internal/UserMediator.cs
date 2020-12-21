using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Database.DataContracts;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.User.Management.Lib.Models.Documents;
using Fixit.User.Management.Lib.Models.Profile;
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
      var databaseName = configurationProvider["FIXIT-UM-USERDB"];
      var databaseUserTableName = configurationProvider["FIXIT-UM-USERTABLE"];
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

      DocumentCollectionDto<UserDocument> documentCollection = (await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, i => i.id == userId.ToString())).DocumentCollection;
      if (documentCollection.OperationException != null)
      {
        throw documentCollection.OperationException;
      }
      if (!documentCollection.IsOperationSuccessful || documentCollection.Results.Count == 0)
      {
        return null;
      }

      UserProfileDto result = _mapper.Map<UserDocument, UserProfileDto>(documentCollection.Results[0]);
      return result;
    }

    public async Task<UserProfileInformationDto> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequestDto userProfileInformationDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      UserProfileInformationDto result = null;

      DocumentCollectionDto<UserDocument> documentCollection = (await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, i => i.id == userId.ToString())).DocumentCollection;
      if (documentCollection.OperationException != null)
      {
        throw documentCollection.OperationException;
      }
      if (!documentCollection.IsOperationSuccessful || documentCollection.Results.Count == 0 )
      {
        return null;
      }

      UserDocument userDocument = documentCollection.Results[0];
      if (!string.IsNullOrWhiteSpace(userProfileInformationDto.FirstName))
      {
        userDocument.FirstName = userProfileInformationDto.FirstName;
      }
      if (!string.IsNullOrWhiteSpace(userProfileInformationDto.LastName))
      {
        userDocument.LastName = userProfileInformationDto.LastName;
      }
      if (userProfileInformationDto.Address != null)
      {
        userDocument.Address = userProfileInformationDto.Address;
      }

      string partitionKey = userDocument.Role == UserRole.Client ? _configuration["FIXIT-UM-CLIENTPK"] : _configuration["FIXIT-UM-CRAFTSMANPK"];

      OperationStatus status = await _databaseUserTable.UpdateItemAsync(userDocument, partitionKey, cancellationToken);
      if (status.OperationException != null)
      {
        throw status.OperationException;
      }
      if (!status.IsOperationSuccessful)
      {
        return null;
      }

      result = _mapper.Map<UserDocument, UserProfileInformationDto>(userDocument);
      return result;
    }
    #endregion
  }
}
