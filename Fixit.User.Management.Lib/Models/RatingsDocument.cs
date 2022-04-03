using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.Database;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Seeders;
using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Ratings;

namespace Fixit.User.Management.Lib.Models
{
  [DataContract]
  public class RatingsDocument : DocumentBase, IFakeSeederAdapter<RatingsDocument>
  {
    [DataMember]
    public float AverageRating { get; set; }

    [DataMember]
    public IEnumerable<RatingDto> Ratings { get; set; }

    [DataMember]
    public UserSummaryDto RatingsOfUser { get; set; }

    [DataMember]  
    public long CreatedTimestampUtc { get; set; }

    [DataMember]
    public long UpdatedTimestampUtc { get; set; }

    #region IFakeSeederAdapter
    public new IList<RatingsDocument> SeedFakeDtos()
    {
      return new List<RatingsDocument>
      {
        new RatingsDocument
        {
          AverageRating = 10,
          Ratings = new List<RatingDto>()
          {
            new RatingDto
            {
              Id = new Guid("f89ca93e-1814-4b73-a18b-e386415f4f82"),
              Score = 10,
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
              Comment = "Test comment.",
              CreatedTimestampUtc = 1611206466,
              UpdatedTimestampUtc = 1611206466,
              Type = RatingType.User
            }
          },
          RatingsOfUser = new UserSummaryDto()
          {
            Id = new Guid("20f5fc6d-7de9-4dbd-9289-23d32ea6548d"),
            FirstName = "John",
            LastName = "Doe"
          },
          CreatedTimestampUtc = 1611206466,
          UpdatedTimestampUtc = 1611206466

        }
      };
    }
    #endregion
  }
}
