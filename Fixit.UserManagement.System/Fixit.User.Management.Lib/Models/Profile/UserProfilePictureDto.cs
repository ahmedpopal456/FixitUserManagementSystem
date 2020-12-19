using System.Runtime.Serialization;

namespace Fixit.User.Management.Lib.Models.Profile
{
  [DataContract]
  public class UserProfilePictureDto
  {
    [DataMember]
    public string ProfilePictureUrl { get; set; }
  }
}
