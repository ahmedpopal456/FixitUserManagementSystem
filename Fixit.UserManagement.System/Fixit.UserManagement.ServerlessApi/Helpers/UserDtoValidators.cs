using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Fixit.Core.DataContracts.Users;
using System.Net.Http;
using Fixit.User.Management.Lib.Models.Profile;
using Fixit.Core.DataContracts.Users.Operations.Profile;

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
    public static bool IsValidUserProfileUpdateRequest(HttpContent httpContent, out UpdateUserProfileRequestDto userProfileInformationDto)
    {
      bool isValid = false;
      userProfileInformationDto = null;

      try
      {
        var userProfileInformationDeserialized = JsonConvert.DeserializeObject<UpdateUserProfileRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (userProfileInformationDeserialized != null)
        {
          isValid = userProfileInformationDeserialized.FirstName != null || userProfileInformationDeserialized.LastName != null || userProfileInformationDeserialized.Address != null;

          if (isValid)
          {
            userProfileInformationDto = userProfileInformationDeserialized;
          }
        }

      }
      catch
      {
        // Fall through 
      }
      return isValid;
    }
    #endregion
  }
}
