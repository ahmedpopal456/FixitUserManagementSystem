using System.Collections.Generic;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Account;
using System;

namespace Fixit.User.Management.Lib.Adapters.Internal.Account
{
  class FakeUserAccountStateUpdateRequestDtoSeeder : IFakeSeederAdapter<UserAccountStateDto>
  {
    public IList<UserAccountStateDto> SeedFakeDtos()
    {
      UserAccountStateDto firstUserAccountToUpdate = new UserAccountStateDto
      {
        State = UserState.Enabled,
        IsOperationSuccessful = true
      };

      UserAccountStateDto secondUserAccountToUpdate = null;


      return new List<UserAccountStateDto>
      {
        firstUserAccountToUpdate,
        secondUserAccountToUpdate
      };
    }
  }
}
