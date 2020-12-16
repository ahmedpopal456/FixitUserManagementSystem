using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Fixit.Core.DataContracts.Users;
using System.Net.Http;

namespace Fixit.User.Management.ServerlessApi.Helpers
{
  public static class UserDtoValidators
  {
    public static bool TryValidatingGuid(string id, out Guid resultingGuid)
    {
      bool isValid = Guid.TryParse(id, out var guidId) && !Guid.Empty.Equals(guidId);
      resultingGuid = guidId;

      return isValid;
    }

    #region UserAccountConfiguration
    #endregion

    #region UserProfileConfiguration
    #endregion
  }
}
