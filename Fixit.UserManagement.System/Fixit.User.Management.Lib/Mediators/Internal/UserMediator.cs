using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Database.Mediators;
using Fixit.User.Management.Lib.Models.Profile;
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
      var databaseName = configurationProvider[""];
      var databaseUserTableName = configurationProvider[""];

      if (string.IsNullOrWhiteSpace(databaseName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects the {nameof(configurationProvider)} to have defined the Fix Management Database as {{}} ");
      }

      if (string.IsNullOrWhiteSpace(databaseUserTableName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects the {nameof(configurationProvider)} to have defined the Fix Management Table as {{}} ");
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
                        string databaseUserTableName)
    {
      if (string.IsNullOrWhiteSpace(databaseName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a valid string for {databaseName}");
      }

      if (string.IsNullOrWhiteSpace(databaseUserTableName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a valid string for {databaseUserTableName}");
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
      throw new NotImplementedException();
    }
    #endregion
  }
}
