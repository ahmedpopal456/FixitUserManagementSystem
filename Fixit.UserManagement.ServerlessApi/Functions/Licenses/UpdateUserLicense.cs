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

  public class UpdateUserLicense : AzureFunctionRoute
  {
    private readonly IUserLicensesMediator _userLicensesMediator;
    private readonly IMapper _mapper;
    public UpdateUserLicense(IUserLicensesMediator userLicensesMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UpdateUserLicense)} expects a value for {nameof(mapper)}... null argument was provided");
      _userLicensesMediator = userLicensesMediator ?? throw new ArgumentNullException($"{nameof(UpdateUserLicense)} expects a value for {nameof(userLicensesMediator)}... null argument was provided");
    }

    [FunctionName("UpdateUserLicense")]
    [OpenApiOperation("put", "UserLicenses")]
    [OpenApiRequestBody("application/json", typeof(UserAddressUpsertRequestDto), Required = true)]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/{id:Guid}/licenses/{licenseId:Guid}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id,
                                         Guid licenseId)
    {
      return await updateUserLicenseAsync(id, licenseId, httpRequest, cancellationToken);
    }

    public async Task<IActionResult> updateUserLicenseAsync(Guid id, Guid licenseId, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (!UserDtoValidators.IsValidUserLicenseUpsertRequest(httpRequest.Content, out UserLicenseUpsertRequestDto userLicenseUpsertRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(userLicenseUpsertRequestDto)} is null or has one or more invalid fields...");
      }

      var result = await _userLicensesMediator.UpdateUserLicenseAsync(id, licenseId, userLicenseUpsertRequestDto, cancellationToken);
      if (result.OperationException == null && !result.IsOperationSuccessful)
      {
        return new NotFoundObjectResult($"A user with the id {id} was not found...");
      }

      return new OkObjectResult(result);
    }
  }
}
