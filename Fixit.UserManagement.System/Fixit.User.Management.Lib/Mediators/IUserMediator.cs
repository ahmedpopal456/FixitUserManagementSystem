using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;

namespace Fixit.User.Management.Lib.Mediators
{
  public interface IUserMediator
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userProfileUpdateRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserProfileInformationDto> UpdateUserProfileAsync(Guid userId, UserProfileUpdateRequestDto userProfileUpdateRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userProfilePictureUpdateRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserProfilePictureDto> UpdateUserProfilePictureAsync(Guid userId, UserProfilePictureUpdateRequestDto userProfilePictureUpdateRequestDto, CancellationToken cancellationToken);
  }
}
