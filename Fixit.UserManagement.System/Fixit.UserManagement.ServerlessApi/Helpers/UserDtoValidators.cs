using Newtonsoft.Json;
using System;
using System.Net.Http;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Address;

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

          bool isValidAddress = userProfileInformationDeserialized.Address != null && !HasNullOrEmpty(userProfileInformationDeserialized.Address);
          isValid = !string.IsNullOrWhiteSpace(userProfileInformationDeserialized.FirstName) && !string.IsNullOrWhiteSpace(userProfileInformationDeserialized.LastName) && isValidAddress;

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

    public static bool HasNullOrEmpty(AddressDto addressDto)
    {
      return string.IsNullOrWhiteSpace(addressDto.Address)
             || string.IsNullOrWhiteSpace(addressDto.City)
             || string.IsNullOrWhiteSpace(addressDto.Province)
             || string.IsNullOrWhiteSpace(addressDto.Country)
             || string.IsNullOrWhiteSpace(addressDto.PostalCode)
             || string.IsNullOrWhiteSpace(addressDto.PhoneNumber);
    }
    #endregion
  }
}
