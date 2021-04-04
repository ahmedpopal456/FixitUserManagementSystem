using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.DataContracts.Users.Operations.Account;
using System.Collections.Generic;
using Fixit.Core.DataContracts.Users;

namespace Fixit.User.Management.Lib.Mediators
{
  public interface IUserMediator
  {
    /// <summary>
    /// Get user summary
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserSummaryResponseDto> GetUserSummaryAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userProfileUpdateRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserProfileInformationDto> UpdateUserProfileAsync(Guid userId, UserProfileUpdateRequestDto userProfileUpdateRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// Update user profile picture
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userProfilePictureUpdateRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserProfilePictureDto> UpdateUserProfilePictureAsync(Guid userId, UserProfilePictureUpdateRequestDto userProfilePictureUpdateRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// Create user
    /// </summary>
    /// <param name="userAccountCreateRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserAccountDto> CreateUserAsync(UserAccountCreateRequestDto userAccountCreateRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// Update user status
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userAccountStateDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserAccountStateDto> UpdateUserStatusAsync(Guid userId, UserAccountStateDto userAccountStateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OperationStatus> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get a list of all user pertaining to the entity provided: either "Craftsman" or "Client".
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<UserDto>> GetUsersAsync(string entity, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userAccountResetPasswordRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OperationStatus> UpdateUserPasswordAsync(Guid userId, UserAccountResetPasswordRequestDto userAccountResetPasswordRequestDto, CancellationToken cancellationToken);
  }
}
