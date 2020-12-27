using System.Collections.Generic;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Operations.Profile;

namespace Fixit.User.Management.Lib.Adapters.Internal.Profile
{
  public class FakeUserProfilePictureUpdateRequestDtoSeeder : IFakeSeederAdapter<UserProfilePictureUpdateRequestDto>
  {
    public IList<UserProfilePictureUpdateRequestDto> SeedFakeDtos()
    {
      UserProfilePictureUpdateRequestDto firstUserProfilePictureUpdateRequest = new UserProfilePictureUpdateRequestDto
      {
        ProfilePictureUrl = "something.something/somethingnew.png"
      };

      UserProfilePictureUpdateRequestDto secondUserProfilePictureUpdateRequest = null;

      return new List<UserProfilePictureUpdateRequestDto>
      {
        firstUserProfilePictureUpdateRequest,
        secondUserProfilePictureUpdateRequest
      };
    }
  }
}
