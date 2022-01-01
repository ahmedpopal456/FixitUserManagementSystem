using System;
using System.Net.Http;
using Newtonsoft.Json;
using Fixit.Core.DataContracts.Users.Ratings;
using Fixit.Core.DataContracts.Users.Operations.Ratings;

namespace Fixit.User.Management.ServerlessApi.Helpers
{
  public static class UserRatingDtoValidators
  {
    #region UserRatingConfiguration
    public static bool IsValidUserRatingCreateRequest(HttpContent httpContent, out UserRatingsCreateOrUpdateRequestDto ratingsDto)
    {
      bool isValid = false;
      ratingsDto = null;

      try
      {
        var ratingDeserialized = JsonConvert.DeserializeObject<UserRatingsCreateOrUpdateRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (ratingDeserialized != null && !ratingDeserialized.ReviewedUser.Equals(null) && !ratingDeserialized.ReviewedUser.Id.Equals(null) 
          && !ratingDeserialized.ReviewedUser.FirstName.Equals(null) && !ratingDeserialized.ReviewedUser.LastName.Equals(null) 
          && !ratingDeserialized.ReviewedByUser.Equals(null) && !ratingDeserialized.ReviewedByUser.Id.Equals(null) 
          && !ratingDeserialized.ReviewedByUser.FirstName.Equals(null) && !ratingDeserialized.ReviewedByUser.LastName.Equals(null)
          && (ratingDeserialized.Score >= 0 && ratingDeserialized.Score <= 5))
        {
          ratingsDto = ratingDeserialized;
          isValid = true;
        }
      }
      catch
      {
        // Fall through
      }
      return isValid;
    }

    public static bool IsValidUserRatingUpdateRequest(HttpContent httpContent, out UserRatingsCreateOrUpdateRequestDto ratingsDto)
    {
      bool isValid = false;
      ratingsDto = null;

      try
      {
        var ratingDeserialized = JsonConvert.DeserializeObject<UserRatingsCreateOrUpdateRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (ratingDeserialized != null && (ratingDeserialized.Score >= 0 && ratingDeserialized.Score <= 5))
        {
          ratingsDto = ratingDeserialized;
          isValid = true;
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
