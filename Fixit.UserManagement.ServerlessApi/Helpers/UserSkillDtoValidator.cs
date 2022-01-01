using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Fixit.Core.DataContracts.Users.Operations;
using Fixit.Core.DataContracts.Users.Operations.Skills;
using Newtonsoft.Json;

namespace Fixit.User.Management.ServerlessApi.Helpers
{
  public static class UserSkillDtoValidator
  {
    public static bool IsValidUserSkillUpdateRequest(HttpContent httpContent, out UpdateUserSkillRequestDto skillDto)
    {
      bool isValid = false;
      skillDto = null;
      
      try
      {
        var userDeserialized = JsonConvert.DeserializeObject<UpdateUserSkillRequestDto>(httpContent.ReadAsStringAsync().Result);
        foreach(var skill in userDeserialized.Skills)
        {
          if (skill.Id != Guid.Empty && skill.Name != string.Empty)
          {
            skillDto = userDeserialized;
            isValid = true;
          }
        }
      }
      catch
      {
        // Fall through
      }
      return isValid;
    }
  }
}
