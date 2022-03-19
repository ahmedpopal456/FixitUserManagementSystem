using System;
using System.Net.Http;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Newtonsoft.Json;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.DataContracts.Users.Profile;
using System.Linq;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Operations.Addresses;

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
    internal static bool IsValidUserCreationRequest(HttpContent httpContent, out UserAccountCreateRequestDto userCreationDto)
    {
      bool isValid = false;
      userCreationDto = null;

      try
      {
        var userCreationDtoDeserialized = JsonConvert.DeserializeObject<UserAccountCreateRequestDto>(httpContent.ReadAsStringAsync().Result);
        isValid = (
          userCreationDtoDeserialized != null &&
          userCreationDtoDeserialized.UserPrincipalName != null && RegexUtilities.IsValidEmail(userCreationDtoDeserialized.UserPrincipalName) &&
          !string.IsNullOrWhiteSpace(userCreationDtoDeserialized.FirstName) &&
          !string.IsNullOrWhiteSpace(userCreationDtoDeserialized.LastName) &&
          Enum.IsDefined(typeof(UserRole), userCreationDtoDeserialized.Role)
        );

        if (isValid)
        {
          userCreationDto = userCreationDtoDeserialized;
        }
      }
      catch
      {
        // Fall through 
      }
      return isValid;
    }

    internal static bool IsValidUserStateUpdateRequest(HttpContent httpContent, out UserAccountStateDto userStateDto)
    {
      bool isValid = false;
      userStateDto = null;

      try
      {
        var userStateDtoDeserialized = JsonConvert.DeserializeObject<UserAccountStateDto>(httpContent.ReadAsStringAsync().Result);
        if (userStateDtoDeserialized != null)
        {
          userStateDto = userStateDtoDeserialized;
          isValid = true;
        }
      }
      catch
      {
        //fall through
      }
      return isValid;
    }

    internal static bool IsValidUserPasswordUpdateRequest(HttpContent httpContent, out UserAccountResetPasswordRequestDto userAccountResetPasswordRequestDto)
    {
      bool isValid = false;
      userAccountResetPasswordRequestDto = null;

      try
      {
        var userPasswordResetDtoDeserialized = JsonConvert.DeserializeObject<UserAccountResetPasswordRequestDto>(httpContent.ReadAsStringAsync().Result);
        isValid = userPasswordResetDtoDeserialized != null && !string.IsNullOrWhiteSpace(userPasswordResetDtoDeserialized.NewPassword);
        if (isValid)
        {
          userAccountResetPasswordRequestDto = userPasswordResetDtoDeserialized;
        }
      }
      catch
      {
        //fall through
      }
      return isValid;
    }
    #endregion

    #region UserProfileConfiguration

    public static bool IsValidUserAddressUpsertRequest(HttpContent httpContent, out UserAddressUpsertRequestDto userAddressUpsertRequest)
    {
      bool isValid = false;
      userAddressUpsertRequest = null;

      try
      {
        var userAdressDeserialized = JsonConvert.DeserializeObject<UserAddressUpsertRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (userAdressDeserialized != null)
        {

          bool isValidAddress = userAdressDeserialized.Address != null;
          isValid = isValidAddress;

          if (isValid)
          {
            userAddressUpsertRequest = userAdressDeserialized;
          }
        }

      }
      catch
      {
        // Fall through 
      }
      return isValid;
    }

    public static bool IsValidUserProfileUpdateRequest(HttpContent httpContent, out UserProfileUpdateRequestDto userProfileUpdateRequestDto)
    {
      bool isValid = false;
      userProfileUpdateRequestDto = null;

      try
      {
        var userProfileInformationDeserialized = JsonConvert.DeserializeObject<UserProfileUpdateRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (userProfileInformationDeserialized != null)
        {

          isValid = !string.IsNullOrWhiteSpace(userProfileInformationDeserialized.FirstName)
                    && !string.IsNullOrWhiteSpace(userProfileInformationDeserialized.LastName)
                    && IsUserAvailabilityValid(userProfileInformationDeserialized.Availability);

          if (isValid)
          {
            userProfileUpdateRequestDto = userProfileInformationDeserialized;
          }
        }

      }
      catch
      {
        // Fall through 
      }
      return isValid;
    }

    public static bool IsValidUserProfilePictureUpdateRequest(HttpContent httpContent, out UserProfilePictureUpdateRequestDto userProfilePictureUpdateRequestDto)
    {
      bool isValid = false;
      userProfilePictureUpdateRequestDto = null;

      try
      {
        var userProfilePictureDeserialized = JsonConvert.DeserializeObject<UserProfilePictureUpdateRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (userProfilePictureDeserialized != null)
        {
          isValid = !string.IsNullOrWhiteSpace(userProfilePictureDeserialized.ProfilePictureUrl);

          if (isValid)
          {
            userProfilePictureUpdateRequestDto = userProfilePictureDeserialized;
          }
        }

      }
      catch
      {
        // Fall through 
      }
      return isValid;
    }

    public static bool IsValidEntityId(string entityId, out string entity)
    {
      entity = default;
      var isValid = !string.IsNullOrWhiteSpace(entityId) && (entityId.ToLower().Equals("craftsman") || entityId.ToLower().Equals("client"));
      if (isValid)
      {
        entity = entityId.First().ToString().ToUpper() + entityId[1..].ToLower();
      }

      return isValid;
    }

    public static bool IsUserAvailabilityValid(UserAvailabilityDto userAvailabilityDto)
    {
      var isValid = true;
      if (userAvailabilityDto != null && userAvailabilityDto.Type == AvailabilityType.Custom && userAvailabilityDto.Schedule == null)
      {
        isValid = false;
      }
      return isValid;
    }
    #endregion

  }
}
