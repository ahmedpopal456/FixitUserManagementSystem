using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts.Users.Operations;
using Fixit.Core.DataContracts.Users.Skills;

namespace Fixit.User.Management.Lib.Mediators
{
  public interface IUserSkillMediator
  {
    /// <summary>
    /// Get user skill
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<SkillDto>> GetUserSkillAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Update user skill
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UpdateUserSkillRequestDto> UpdateUserSkillAsync(Guid userId, UpdateUserSkillRequestDto updateUserSkillRequestDto, CancellationToken cancellationToken);
  }
}
