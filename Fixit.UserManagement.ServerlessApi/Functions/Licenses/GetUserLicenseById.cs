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

  public class GetUserLicenseById : AzureFunctionRoute
  {
    private readonly IUserLicensesMediator _userLicensesMediator;
    private readonly IMapper _mapper;
    public GetUserLicenseById(IUserLicensesMediator userLicensesMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(GetUserLicenseById)} expects a value for {nameof(mapper)}... null argument was provided");
      _userLicensesMediator = userLicensesMediator ?? throw new ArgumentNullException($"{nameof(GetUserLicenseById)} expects a value for {nameof(userLicensesMediator)}... null argument was provided");
    }

    [FunctionName("GetUserLicenseById")]
    [OpenApiOperation("get", "UserLicenses")]
    [OpenApiRequestBody("application/json", typeof(UserAddressUpsertRequestDto), Required = true)]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{id:Guid}/licenses/{licenseId:Guid}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id,
                                         Guid licenseId)
    {
      return await GetUserLicenseByIdAsync(id, licenseId, cancellationToken);
    }

    public async Task<IActionResult> GetUserLicenseByIdAsync(Guid id, Guid licenseId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var result = await _userLicensesMediator.GetUserLicenseByIdAsync(id, licenseId, cancellationToken);
      if (result.OperationException == null && !result.IsOperationSuccessful)
      {
        return new NotFoundObjectResult($"A user with the id {id} was not found...");
      }

      return new OkObjectResult(result);
    }
  }
}
