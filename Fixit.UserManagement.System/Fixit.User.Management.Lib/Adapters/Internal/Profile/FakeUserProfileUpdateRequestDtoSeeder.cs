using System.Collections.Generic;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Operations.Profile;

namespace Fixit.User.Management.Lib.Adapters.Internal.Profile
{
  public class FakeUserProfileUpdateRequestDtoSeeder : IFakeSeederAdapter<UserProfileUpdateRequestDto>
  {
    public IList<UserProfileUpdateRequestDto> SeedFakeDtos()
    {
      UserProfileUpdateRequestDto firstUserProfileUpdateRequest = new UserProfileUpdateRequestDto
      {
        FirstName = "Jane",
        LastName = "Doe",
        Address = new AddressDto()
        {
          Address = "123 Something",
          City = "Montreal",
          Province = "Quebec",
          Country = "Canada",
          PostalCode = "A1A 1A1",
          PhoneNumber = "514-123-4567"
        }
      };

      UserProfileUpdateRequestDto secondUserProfileUpdateRequest = null;

      return new List<UserProfileUpdateRequestDto>
      {
        firstUserProfileUpdateRequest,
        secondUserProfileUpdateRequest
      };
    }
  }
}
