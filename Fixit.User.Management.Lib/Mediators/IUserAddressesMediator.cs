using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Address;

namespace Fixit.User.Management.Lib.Mediators
{
  public interface IUserAddressesMediator
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userAddressUpsertRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OperationStatusWithObject<UserAddressDto>> CreateUserAddressAsync(Guid userId, UserAddressUpsertRequestDto userAddressUpsertRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userAddressId"></param>
    /// <param name="userAddressUpsertRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OperationStatusWithObject<UserAddressDto>> UpdateUserAddressStatusAsync(Guid userId, Guid userAddressId, UserAddressUpsertRequestDto userAddressUpsertRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userAddressId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OperationStatus> DeleteUserAddressAsync(Guid userId, Guid userAddressId, CancellationToken cancellationToken);
  }
}
