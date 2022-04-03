using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using Fixit.User.Management.ServerlessApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Operations.Addresses;
using Fixit.Core.DataContracts.Users.Operations.Licenses;

namespace Fixit.User.Management.ServerlessApi.Functions.Accounts
{

  public class DeleteUserLicense : AzureFunctionRoute
  {
    private readonly IUserLicensesMediator _userLicensesMediator;
    private readonly IMapper _mapper;
    public DeleteUserLicense(IUserLicensesMediator userLicensesMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(DeleteUserLicense)} expects a value for {nameof(mapper)}... null argument was provided");
      _userLicensesMediator = userLicensesMediator ?? throw new ArgumentNullException($"{nameof(DeleteUserLicense)} expects a value for {nameof(userLicensesMediator)}... null argument was provided");
    }

    [FunctionName("DeleteUserLicense")]
    [OpenApiOperation("delete", "UserLicenses")]
    [OpenApiRequestBody("application/json", typeof(UserAddressUpsertRequestDto), Required = true)]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "users/{id:Guid}/licenses/{licenseId:Guid}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id,
                                         Guid licenseId)
    {
      return await deleteUserLicenseAsync(id, licenseId, httpRequest, cancellationToken);
    }

    public async Task<IActionResult> deleteUserLicenseAsync(Guid id, Guid licenseId, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var result = await _userLicensesMediator.DeleteUserLicenseAsync(id, licenseId, cancellationToken);
      if (result.OperationException == null && !result.IsOperationSuccessful)
      {
        return new NotFoundObjectResult($"A user with the id {id} was not found...");
      }

      return new OkObjectResult(result);
    }
  }
}
