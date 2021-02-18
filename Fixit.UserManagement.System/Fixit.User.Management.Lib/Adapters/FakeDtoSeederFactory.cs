using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Ratings;
using Fixit.User.Management.Lib.Adapters.Internal.Profile;
using Fixit.User.Management.Lib.Adapters.Internal.Account;
using Fixit.User.Management.Lib.Adapters.Internal.Ratings;
using Fixit.User.Management.Lib.Models;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.DataContracts.Users.Operations.Ratings;

namespace Fixit.User.Management.Lib.Adapters
{
  public class FakeDtoSeederFactory : IFakeSeederFactory
  {
    public IFakeSeederAdapter<T> CreateFakeSeeder<T>() where T : class
    {
      string type = typeof(T).Name;

      switch (type)
      {
        case nameof(UserDocument):
          return (IFakeSeederAdapter<T>)new FakeUserDocumentSeeder();
        case nameof(UserProfileUpdateRequestDto):
          return (IFakeSeederAdapter<T>)new FakeUserProfileUpdateRequestDtoSeeder();
        case nameof(UserProfilePictureUpdateRequestDto):
          return (IFakeSeederAdapter<T>)new FakeUserProfilePictureUpdateRequestDtoSeeder();
        case nameof(UserAccountCreateRequestDto):
          return (IFakeSeederAdapter<T>)new FakeUserAccountCreateRequestDtoSeeder();
        case nameof(UserAccountStateDto):
          return (IFakeSeederAdapter<T>)new FakeUserAccountStateUpdateRequestDtoSeeder();
        case nameof(RatingsDocument):
          return (IFakeSeederAdapter<T>) new FakeUserRatingsDocumentSeeder();
        case nameof(UserRatingsCreateOrUpdateRequestDto):
          return (IFakeSeederAdapter<T>)new FakeUserRatingsCreateOrUpdateRequestDtoSeeder();
        default:
          return null;
      }
    }
  }
}
