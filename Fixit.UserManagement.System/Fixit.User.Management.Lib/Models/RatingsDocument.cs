using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.Database;
using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Ratings;

namespace Fixit.User.Management.Lib.Models
{
  [DataContract]
  public class RatingsDocument : DocumentBase
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
  }
}
