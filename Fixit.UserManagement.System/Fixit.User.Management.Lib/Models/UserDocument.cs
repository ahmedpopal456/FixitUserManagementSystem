using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.Database;
using Fixit.Core.DataContracts.Seeders;
using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Documents;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.Core.DataContracts.Users.Skills;

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
    public AddressDto Address { get; set; }

    [DataMember]
    public UserRole Role { get; set; }

    [DataMember]
    public Gender Gender { get; set; }

    [DataMember]
    public UserStatusDto Status { get; set; }

    [DataMember]
    public UserAvailabilityDto Availability { get; set; }

    [DataMember]
    public IEnumerable<SkillDto> Skills { get; set; }

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
        LastName = "Doe",
        Skills = new List<SkillDto>
        {
          new SkillDto
          {
            Id = new Guid("db53bc7b-2893-47a9-9134-3696579878df"),
            Name ="Hello"
          }   
        },
        UserPrincipalName = "johnDoe@test.com",
        Role = UserRole.Client,
        Address = new AddressDto()
        {
          Address = "123 Something",
          City = "Montreal",
          Province = "Quebec",
          Country = "Canada",
          PostalCode = "A1A 1A1",
          PhoneNumber = "514-123-4567"
        }
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
