using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.Database;
using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Documents;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Ratings;

namespace Fixit.User.Management.Lib.Models
{
  [DataContract]
  public class UserDocument : DocumentBase
  {
    [DataMember]
    public string UserPrincipalName { get; set; }

    [DataMember]
    public string ProfilePictureUrl { get; set; }

    [DataMember]
    public string FirstName { get; set; }

    [DataMember]
    public string LastName { get; set; }

    [DataMember]
    public UserState State { get; set; }

    [DataMember]
    public RatingsDto Rating { get; set; }

    [DataMember]
    public AddressDto Address { get; set; }

    [DataMember]
    public UserRole Role { get; set; }

    [DataMember]
    public Gender Gender { get; set; }

    [DataMember]
    public UserStatusDto Status { get; set; }

    [DataMember]
    public string TelephoneNumber { get; set; }

    [DataMember]
    public long CreatedTimestampsUtc { get; set; }

    [DataMember]
    public long UpdatedTimestampsUtc { get; set; }

    [DataMember]
    public IEnumerable<DocumentSummaryDto> Documents { get; set; }
  }
}
