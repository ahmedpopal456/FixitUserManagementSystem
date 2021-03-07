using System;
using System.Runtime.Serialization;
using Fixit.Core.Database;

namespace Fixit.User.Management.Lib.Models
{
  public class SkillDocument : DocumentBase
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Category { get; set; }

    [DataMember]
    public long AttributedAtTimeStampUtc { get; set; }

    [DataMember]
    public long ExpiredAtTimeStampUtc { get; set; }
  }
}
