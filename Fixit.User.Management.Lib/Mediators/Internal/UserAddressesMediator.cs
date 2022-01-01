using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Connectors.DataContracts;
using Fixit.Core.Connectors.Mediators;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.DataContracts;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts.Users;
using Fixit.User.Management.Lib.Models;
using Fixit.User.Management.Lib.EnumExtension;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Fixit.Core.DataContracts.Users.Address;

[assembly: InternalsVisibleTo("Fixit.User.Management.Lib.UnitTests")]
[assembly: InternalsVisibleTo("Fixit.User.Management.ServerlessApi")]
[assembly: InternalsVisibleTo("Fixit.User.Management.Triggers")]
namespace Fixit.User.Management.Lib.Mediators.Internal
{
  internal class UserAddressesMediator : IUserAddressesMediator
  {
    private readonly IMapper _mapper;
    private readonly IDatabaseTableEntityMediator _databaseUserTable;
    private readonly IMicrosoftGraphMediator _msGraphClient;
    private readonly Container _userContainer;

    public UserAddressesMediator(IMapper mapper,
                                 IDatabaseMediator databaseMediator,
                                 CosmosClient cosmosClient,
                                 IConfiguration configurationProvider)
    {
      var databaseName = configurationProvider["FIXIT-UM-DB-NAME"];
      var databaseUserTableName = configurationProvider["FIXIT-UM-DB-USERTABLE"];

      if (string.IsNullOrWhiteSpace(databaseName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects the {nameof(configurationProvider)} to have defined the User Management Database as {{FIXIT-UM-DB-NAME}} ");
      }

      if (string.IsNullOrWhiteSpace(databaseUserTableName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects the {nameof(configurationProvider)} to have defined the User Management Table as {{FIXIT-UM-DB-USERTABLE}} ");
      }

      if (databaseMediator == null)
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(databaseMediator)}... null argument was provided");
      }

      if (cosmosClient == null)
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(cosmosClient)}... null argument was provided");
      }

      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _databaseUserTable = databaseMediator.GetDatabase(databaseName).GetContainer(databaseUserTableName);
      _userContainer = cosmosClient.GetContainer(databaseName, databaseUserTableName);
    }

    public UserAddressesMediator(IMapper mapper,
                        IDatabaseMediator databaseMediator,
                        CosmosClient cosmosClient,
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

      if (cosmosClient == null)
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(cosmosClient)}... null argument was provided");
      }

      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _databaseUserTable = databaseMediator.GetDatabase(databaseName).GetContainer(tableName);
      _userContainer = cosmosClient.GetContainer(databaseName, tableName);
    }

    public async Task<OperationStatusWithObject<UserAddressDto>> CreateUserAddressAsync(Guid userId, UserAddressUpsertRequestDto userAddressUpsertRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var result = new OperationStatusWithObject<UserAddressDto>()
      {
        IsOperationSuccessful = false,
      };

      var (userDocumentCollection, continuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id == userId.ToString());
      result.OperationException = userDocumentCollection.OperationException;
      if (userDocumentCollection.IsOperationSuccessful)
      {
        UserDocument userDocument = userDocumentCollection.Results.SingleOrDefault();
        if (userDocument is { })
        {
          long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
          var addressToAdd = _mapper.Map<UserAddressUpsertRequestDto, UserAddressDto>(userAddressUpsertRequestDto);

          userDocument.SavedAddresses ??= new List<UserAddressDto>();
          addressToAdd.CreatedTimestampUtc = addressToAdd.UpdatedTimestampUtc = currentTime;
          addressToAdd.Id = Guid.NewGuid();

          userDocument.SavedAddresses.Add(addressToAdd);
          
          var updatedUser = await _databaseUserTable.UpsertItemAsync(userDocument, userDocument.EntityId, cancellationToken);
          if(updatedUser.IsOperationSuccessful)
          {
            result.Result = addressToAdd;
            result.IsOperationSuccessful = true;
          }
        }
      }

      return result;
    }

    public async Task<OperationStatusWithObject<UserAddressDto>> UpdateUserAddressStatusAsync(Guid userId, Guid userAddressId, UserAddressUpsertRequestDto userAddressUpsertRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var result = new OperationStatusWithObject<UserAddressDto>()
      {
        IsOperationSuccessful = false,
      };

      var (userDocumentCollection, continuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id == userId.ToString());
      result.OperationException = userDocumentCollection.OperationException;
      if (userDocumentCollection.IsOperationSuccessful)
      {
        UserDocument userDocument = userDocumentCollection.Results.SingleOrDefault();
        if (userDocument is { })
        {
          long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

          var userAddressToUpdate = userDocument.SavedAddresses.SingleOrDefault(sa => sa.Id == userAddressId);
          if(userAddressToUpdate != null)
          {
            _mapper.Map<UserAddressUpsertRequestDto, UserAddressDto>(userAddressUpsertRequestDto, userAddressToUpdate);
            userAddressToUpdate.UpdatedTimestampUtc = currentTime;

            var updatedUser = await _databaseUserTable.UpsertItemAsync(userDocument, userDocument.EntityId, cancellationToken);
            if (updatedUser.IsOperationSuccessful)
            {
              result.Result = userAddressToUpdate;
              result.IsOperationSuccessful = true;
            }
          }
        }
      }

      return result;
    }

    public async Task<OperationStatus> DeleteUserAddressAsync(Guid userId, Guid userAddressId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      OperationStatus result = new OperationStatus()
      {
        IsOperationSuccessful = false
      };

      var (userDocumentCollection, continuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id.Equals(userId.ToString()));
      result.OperationException = userDocumentCollection.OperationException;
      if (userDocumentCollection.IsOperationSuccessful)
      {
        UserDocument userDocument = userDocumentCollection.Results.SingleOrDefault();
        if (userDocument is { })
        {
          var userAddressToDelete = userDocument.SavedAddresses.SingleOrDefault(sa => sa.Id == userAddressId);
          if (userAddressToDelete != null)
          {
            userDocument.SavedAddresses.Remove(userAddressToDelete);

            var updatedUser = await _databaseUserTable.UpsertItemAsync(userDocument, userDocument.EntityId, cancellationToken);
            if (updatedUser.IsOperationSuccessful)
            {
              result.IsOperationSuccessful = true;
            }
          }

      }        }
      return result;
    }
  }
}
