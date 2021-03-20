using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts.Users.Operations;
using Fixit.Core.DataContracts.Users.Skills;
using Fixit.User.Management.Lib.Models;
using Microsoft.Extensions.Configuration;

namespace Fixit.User.Management.Lib.Mediators.Internal
{
  public class UserSkillMediator : IUserSkillMediator
  {
    private readonly IDatabaseTableEntityMediator _databaseUserTable;

    public UserSkillMediator(IMapper mapper,
                             IDatabaseMediator databaseMediator,
                             IConfiguration configurationProvider)
    {
      var databaseName = configurationProvider["FIXIT-UM-DB-NAME"];
      var databaseUserTableName = configurationProvider["FIXIT-UM-DB-USERTABLE"];

      if (string.IsNullOrWhiteSpace(databaseName))
      {
        throw new ArgumentNullException($"{nameof(UserSkillMediator)} expects the {nameof(configurationProvider)} to have defined the User Management Database as {{FIXIT-UM-DB-NAME}} ");
      }

      if (string.IsNullOrWhiteSpace(databaseUserTableName))
      {
        throw new ArgumentNullException($"{nameof(UserSkillMediator)} expects the {nameof(configurationProvider)} to have defined the User Skill Management Table as {{FIXIT-UM-DB-USERTABLE}} ");
      }

      if (databaseMediator == null)
      {
        throw new ArgumentNullException($"{nameof(UserSkillMediator)} expects a value for {nameof(databaseMediator)}... null argument was provided");
      }

      _databaseUserTable = databaseMediator.GetDatabase(databaseName).GetContainer(databaseUserTableName);
    }

    public UserSkillMediator(IMapper mapper,
                        IDatabaseMediator databaseMediator,
                        string databaseName,
                        string tableName)
    {

      if (string.IsNullOrWhiteSpace(databaseName))
      {
        throw new ArgumentNullException($"{nameof(UserSkillMediator)} expects a value for {nameof(databaseName)}... null argument was provided");
      }

      if (string.IsNullOrWhiteSpace(tableName))
      {
        throw new ArgumentNullException($"{nameof(UserSkillMediator)} expects a value for {nameof(tableName)}... null argument was provided");
      }

      if (databaseMediator == null)
      {
        throw new ArgumentNullException($"{nameof(UserSkillMediator)} expects a value for {nameof(databaseMediator)}... null argument was provided");
      }

      _databaseUserTable = databaseMediator.GetDatabase(databaseName).GetContainer(tableName);
    }



    public async Task<IEnumerable<SkillDto>> GetUserSkillAsync(Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      List<SkillDto> result = new List<SkillDto>();

      var (userDocumentCollection, continuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, userDocument => userDocument.id == userId.ToString());

      if (userDocumentCollection != null && userDocumentCollection.IsOperationSuccessful)
      {
        var userDocument = userDocumentCollection.Results.SingleOrDefault();
        result = userDocument.Skills.Any() ? userDocument.Skills.ToList() : default;
      }
      return result;
    }

    public async Task<UpdateUserSkillRequestDto> UpdateUserSkillAsync(Guid userId, UpdateUserSkillRequestDto updateUserSkillRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var result = new UpdateUserSkillRequestDto();

      var (userSkillDocumentCollection, continuationToken) = await _databaseUserTable.GetItemQueryableAsync<UserDocument>(null, cancellationToken, skillDocument => skillDocument.id == userId.ToString());

      if(userSkillDocumentCollection != null)
      {
        var userDocument = userSkillDocumentCollection.Results.SingleOrDefault();
        userDocument.Skills = updateUserSkillRequestDto.Skill;
        var operationStatus = await _databaseUserTable.UpsertItemAsync(userDocument, userDocument.EntityId, cancellationToken);

        if (operationStatus.IsOperationSuccessful && Guid.TryParse(userDocument.id, out var newGuid))
        {
          result = new UpdateUserSkillRequestDto {Skill = userDocument.Skills, UserId = Guid.Parse(userDocument.id)};
          result.AttributedAtTimestampUtc = result.ExpiresAtTimestampUtc = DateTimeOffset.Now.ToUnixTimeSeconds();
          result.IsOperationSuccessful = true;
        }     
      }

      return result;
    }
  }
}
