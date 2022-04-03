using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Connectors.Mediators;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.License;
using Fixit.Core.DataContracts.Users.Operations.Licenses;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Fixit.User.Management.Lib.Models;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Operations.Addresses;

[assembly: InternalsVisibleTo("Fixit.User.Management.Lib.UnitTests")]
[assembly: InternalsVisibleTo("Fixit.User.Management.ServerlessApi")]
namespace Fixit.User.Management.Lib.Mediators.Internal
{
  public class UserLicensesMediator : IUserLicensesMediator
  {
    private readonly IMapper _mapper;
    private readonly IDatabaseTableEntityMediator _databaseUserTable;
    private readonly IMicrosoftGraphMediator _msGraphClient;
    private readonly Container _userContainer;

    public UserLicensesMediator(IMapper mapper,
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

    public async Task<OperationStatusWithObject<UserLicenseDto>> CreateUserLicenseAsync(Guid userId, UserLicenseUpsertRequestDto userLicenseUpsertRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var result = new OperationStatusWithObject<UserLicenseDto>()
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
          var licenseToAdd = _mapper.Map<UserLicenseUpsertRequestDto, UserLicenseDto>(userLicenseUpsertRequestDto);

          userDocument.Licenses ??= new List<UserLicenseDto>();
          licenseToAdd.CreatedTimestampUtc = licenseToAdd.UpdatedTimestampUtc = currentTime;
          licenseToAdd.Id = Guid.NewGuid();

          userDocument.Licenses.Add(licenseToAdd);

          var updatedUser = await _databaseUserTable.UpsertItemAsync(userDocument, userDocument.EntityId, cancellationToken);
          if (updatedUser.IsOperationSuccessful)
          {
            result.Result = licenseToAdd;
            result.IsOperationSuccessful = true;
          }
        }
      }

      return result;
    }

    public async Task<OperationStatusWithObject<UserLicenseDto>> UpdateUserLicenseAsync(Guid userId, Guid userLicenseId, UserLicenseUpsertRequestDto userLicenseUpsertRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var result = new OperationStatusWithObject<UserLicenseDto>()
      {
        IsOperationSuccessful = false,
      };

      var (userDocumentCollection, continuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id == userId.ToString());
      result.OperationException = userDocumentCollection.OperationException;
      if (userDocumentCollection.IsOperationSuccessful)
      {
        UserDocument userDocument = userDocumentCollection.Results.SingleOrDefault();
        if (userDocument is { Licenses: { } })
        {
          long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

          var userLicenseToUpdate = userDocument.Licenses.SingleOrDefault(license => license.Id == userLicenseId);
          if (userLicenseToUpdate != null)
          {
            _mapper.Map<UserLicenseUpsertRequestDto, UserLicenseDto>(userLicenseUpsertRequestDto, userLicenseToUpdate);
            userLicenseToUpdate.UpdatedTimestampUtc = currentTime;

            var updatedUser = await _databaseUserTable.UpsertItemAsync(userDocument, userDocument.EntityId, cancellationToken);
            if (updatedUser.IsOperationSuccessful)
            {
              result.Result = userLicenseToUpdate;
              result.IsOperationSuccessful = true;
            }
          }
        }
      }

      return result;
    }

    public async Task<OperationStatus> DeleteUserLicenseAsync(Guid userId, Guid userLicenseId, CancellationToken cancellationToken)
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
          var userLicenseToDelete = userDocument.Licenses.SingleOrDefault(license => license.Id == userLicenseId);
          if (userLicenseToDelete != null)
          {
            userDocument.Licenses.Remove(userLicenseToDelete);

            var updatedUser = await _databaseUserTable.UpsertItemAsync(userDocument, userDocument.EntityId, cancellationToken);
            if (updatedUser.IsOperationSuccessful)
            {
              result.IsOperationSuccessful = true;
            }
          }

        }
      }
      return result;
    }

    public async Task<OperationStatusWithObject<UserLicenseDto>> GetUserLicenseByIdAsync(Guid userId, Guid userLicenseId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var result = new OperationStatusWithObject<UserLicenseDto>()
      {
        IsOperationSuccessful = false
      };

      var (userDocumentCollection, continuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id.Equals(userId.ToString()));
      result.OperationException = userDocumentCollection.OperationException;
      if (userDocumentCollection.IsOperationSuccessful)
      {
        UserDocument userDocument = userDocumentCollection.Results.SingleOrDefault();
        if (userDocument is { Licenses: { } })
        {
          var userLicenseToReturn = userDocument.Licenses.SingleOrDefault(license => license.Id == userLicenseId);
          result.Result = userLicenseToReturn;
          result.IsOperationSuccessful = true;
        }
      }
      return result;
    }

  }
}
