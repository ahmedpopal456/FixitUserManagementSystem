using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts.Seeders;
using System.Collections.Generic;

namespace Fixit.User.Management.Lib.Adapters
{
  public class FakeDtoSeederFactory : IFakeSeederFactory
  {
    public IList<T> CreateSeederFactory<T>(IFakeSeederAdapter<T> fakeSeederAdapter) where T : class
    {
      return fakeSeederAdapter.SeedFakeDtos();
    }
  }
}
