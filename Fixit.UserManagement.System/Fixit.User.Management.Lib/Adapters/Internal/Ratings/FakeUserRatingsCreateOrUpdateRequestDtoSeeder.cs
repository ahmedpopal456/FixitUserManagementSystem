using System;
using System.Collections.Generic;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users;
using Fixit.User.Management.Lib.Models;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Operations.Ratings;

namespace Fixit.User.Management.Lib.Adapters.Internal.Ratings
{
  public class FakeUserRatingsCreateOrUpdateRequestDtoSeeder : IFakeSeederAdapter<UserRatingsCreateOrUpdateRequestDto>
  {
    public IList<UserRatingsCreateOrUpdateRequestDto> SeedFakeDtos()
    {
      UserRatingsCreateOrUpdateRequestDto firstRatingsRequest = new UserRatingsCreateOrUpdateRequestDto
      {
        Score = 3,
        Comment = "This is a comment",
        ReviewedByUser = new UserSummaryDto()
        {
          Id = new Guid("385de974-be63-4eb3-931b-a853d5f63729"),
          FirstName = "Janet",
          LastName = "Doe"
        },
        ReviewedUser = new UserSummaryDto()
        {
          Id = new Guid("20f5fc6d-7de9-4dbd-9289-23d32ea6548d"),
          FirstName = "John",
          LastName = "Doe"
        },
        Type = RatingType.User
      };

      UserRatingsCreateOrUpdateRequestDto secondRatingsRequest = null;

      return new List<UserRatingsCreateOrUpdateRequestDto>
      {
        firstRatingsRequest,
        secondRatingsRequest
      };
    }
  }
}
