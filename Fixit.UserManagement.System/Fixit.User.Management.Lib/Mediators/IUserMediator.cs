using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.User.Management.Lib.Models.Profile;

namespace Fixit.User.Management.Lib.Mediators
{
  public interface IUserMediator
  {
    Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken);
  }
}
