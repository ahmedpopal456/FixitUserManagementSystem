using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.User.Management.Lib.Adapters.Internal.Profile;
using Fixit.User.Management.Lib.Models;

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
          return (IFakeSeederAdapter<T>) new FakeUserProfileUpdateRequestDtoSeeder();
        case nameof(UserProfilePictureUpdateRequestDto):
          return (IFakeSeederAdapter<T>) new FakeUserProfilePictureUpdateRequestDtoSeeder();
        default:
          return null;
      }
    }
  }
}
