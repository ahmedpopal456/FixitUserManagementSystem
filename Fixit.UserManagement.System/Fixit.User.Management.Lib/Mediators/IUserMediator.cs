using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.User.Management.Lib.Models.Profile;

namespace Fixit.User.Management.Lib.Mediators
{
  public interface IUserMediator
  {
    Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken);

    Task<UserProfileInformationDto> UpdateUserProfileAsync(Guid userId, UserProfileUpdateRequestDto userProfileUpdateRequestDto, CancellationToken cancellationToken);

    Task<UserProfilePictureDto> UpdateUserProfilePictureAsync(Guid userId, UserProfilePictureUpdateRequestDto userProfilePictureUpdateRequestDto, CancellationToken cancellationToken);
  }
}
