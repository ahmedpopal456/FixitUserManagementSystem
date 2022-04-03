using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.License;
using Fixit.Core.DataContracts.Users.Operations.Licenses;

namespace Fixit.User.Management.Lib.Mediators
{
  public interface IUserLicensesMediator
  {
    Task<OperationStatusWithObject<UserLicenseDto>> CreateUserLicenseAsync(Guid userId, UserLicenseUpsertRequestDto userLicenseUpsertRequestDto, CancellationToken cancellationToken);
    Task<OperationStatusWithObject<UserLicenseDto>> UpdateUserLicenseAsync(Guid userId, Guid licenseId, UserLicenseUpsertRequestDto userLicenseUpsertRequestDto, CancellationToken cancellationToken);
    Task<OperationStatus> DeleteUserLicenseAsync(Guid userId, Guid userLicenseId, CancellationToken cancellationToken);
    Task<OperationStatusWithObject<UserLicenseDto>> GetUserLicenseByIdAsync(Guid userId, Guid licenseId, CancellationToken cancellationToken);
  }
}
