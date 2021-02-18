﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.DataContracts.Users.Operations.Account;

namespace Fixit.User.Management.Lib.Mediators
{
  public interface IUserMediator
  {
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
    Task<UserAccountRoleResponseDto> CreateUserAsync(UserAccountCreateRequestDto userAccountCreateRequestDto, CancellationToken cancellationToken);

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
  }
}
