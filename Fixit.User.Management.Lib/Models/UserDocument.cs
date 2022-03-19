using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.Database;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.FixTemplates;
using Fixit.Core.DataContracts.Seeders;
using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Documents;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.Core.DataContracts.Users.Skill;

namespace Fixit.User.Management.Lib.Models
{
  [DataContract]
  public class UserDocument : DocumentBase, IFakeSeederAdapter<UserDocument>
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
    public List<UserAddressDto> SavedAddresses { get; set; } 

    [DataMember]
    public UserRole Role { get; set; }

    [DataMember]
    public Gender Gender { get; set; }

    [DataMember]
    public UserStatusDto Status { get; set; }

    [DataMember]
    public UserAvailabilityDto Availability { get; set; }

    [DataMember]
    public IEnumerable<LicenseDto> Licenses { get; set; }

    [DataMember]
    public string TelephoneNumber { get; set; }

    [DataMember]
    public long CreatedTimestampsUtc { get; set; }

    [DataMember]
    public long UpdatedTimestampsUtc { get; set; }

    [DataMember]
    public IEnumerable<DocumentSummaryDto> Documents { get; set; }

    public new IList<UserDocument> SeedFakeDtos()
    {
      UserDocument firstUserDocument = new UserDocument
      {
        ProfilePictureUrl = "something.something/something.png",
        FirstName = "John",
        SavedAddresses = new List<UserAddressDto>() { 
          new UserAddressDto() 
        { 
            Address = new AddressDto()
            {
              FormattedAddress = "",
            },
            Id = Guid.NewGuid(),
            IsCurrentAddress = true,
        }},
        LastName = "Doe",
        Licenses = new List<LicenseDto>
        {
          new LicenseDto
          {
            Name ="Hello"
          }   
        },
        UserPrincipalName = "johnDoe@test.com",
        Role = UserRole.Client,
      };

      UserDocument secondUserDocument = null;

      return new List<UserDocument>
      {
        firstUserDocument,
        secondUserDocument
      };
    }
  }
}
