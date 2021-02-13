using System.Collections.Generic;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts.Users.Enums;

namespace Fixit.User.Management.Lib.Adapters.Internal.Account
{
  class FakeUserAccountCreateRequestDtoSeeder : IFakeSeederAdapter<UserAccountCreateRequestDto>
  {
    public IList<UserAccountCreateRequestDto> SeedFakeDtos()
    {
      UserAccountCreateRequestDto firstUserAccountToCreate = new UserAccountCreateRequestDto
      {
        Id = "some_id",
        FirstName = "John",
        LastName = "Doe",
        Role = UserRole.Client,
        UserPrincipalName = "johnDoe@test.com"

      };

      UserAccountCreateRequestDto secondUserAccountToCreate = null;

      return new List<UserAccountCreateRequestDto>
      {
        firstUserAccountToCreate,
        secondUserAccountToCreate
      };
    }
  }
}
