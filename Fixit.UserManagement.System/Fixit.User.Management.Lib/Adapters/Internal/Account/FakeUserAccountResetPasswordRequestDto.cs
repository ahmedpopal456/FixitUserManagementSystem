using System.Collections.Generic;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Operations.Account;

namespace Fixit.User.Management.Lib.Adapters.Internal.Account
{
  class FakeUserAccountResetPasswordRequestDtoSeeder : IFakeSeederAdapter<UserAccountResetPasswordRequestDto>
  {
    public IList<UserAccountResetPasswordRequestDto> SeedFakeDtos()
    {
      UserAccountResetPasswordRequestDto firstUserAccountToUpdate = new UserAccountResetPasswordRequestDto
      {
        NewPassword = "Fara0921",
      };

      UserAccountResetPasswordRequestDto secondUserAccountToUpdate = null;


      return new List<UserAccountResetPasswordRequestDto>
      {
        firstUserAccountToUpdate,
        secondUserAccountToUpdate
      };
    }
  }
}
