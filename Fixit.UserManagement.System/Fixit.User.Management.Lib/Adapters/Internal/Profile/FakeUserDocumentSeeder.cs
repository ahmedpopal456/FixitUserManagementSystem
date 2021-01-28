using System.Collections.Generic;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.User.Management.Lib.Models;

namespace Fixit.User.Management.Lib.Adapters.Internal.Profile
{
  public class FakeUserDocumentSeeder : IFakeSeederAdapter<UserDocument>
  {
    public IList<UserDocument> SeedFakeDtos()
    {
      UserDocument firstUserDocument = new UserDocument
      {
        ProfilePictureUrl = "something.something/something.png",
        FirstName = "John",
        LastName = "Doe",
        UserPrincipalName = "johnDoe@test.com",
        Role = UserRole.Client,
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

      UserDocument secondUserDocument = null;

      return new List<UserDocument>
      {
        firstUserDocument,
        secondUserDocument
      };
    }
  }
}
