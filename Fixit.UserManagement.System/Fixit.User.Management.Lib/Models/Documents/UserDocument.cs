using System;
using System.Collections.Generic;
using Fixit.Core.Database;
using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Documents;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Ratings;

namespace Fixit.User.Management.Lib.Models.Documents
{
  public class UserDocument : DocumentBase
  {
    public Guid Id { get; set; }

    public string UserPrincipalName { get; set; }

    public string ProfilePictureUrl { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public UserState State { get; set; }

    public RatingsSummaryDto Rating { get; set; }

    public AddressDto Address { get; set; }

    public UserRole Role { get; set; }

    public Gender Gender { get; set; }

    public UserStatusDto Status { get; set; }

    public string TelephoneNumber { get; set; }

    public long CreatedTimestampsUtc { get; set; }

    public long UpdatedTimestampsUtc { get; set; }

    public IEnumerable<DocumentSummaryDto> Documents { get; set; }
  }
}
