using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using Fixit.User.Management.ServerlessApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;
using Fixit.Core.DataContracts.Users.Address;

namespace Fixit.User.Management.ServerlessApi.Functions.Accounts
{

  public class UpdateUserAddress : AzureFunctionRoute
  {
    private readonly IUserAddressesMediator _userAddressesMediator;
    private readonly IMapper _mapper;
    public UpdateUserAddress(IUserAddressesMediator userAddressesMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(mapper)}... null argument was provided");
      _userAddressesMediator = userAddressesMediator ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(userAddressesMediator)}... null argument was provided");
    }

    [FunctionName("UpdateUserAddress")]
    [OpenApiOperation("put", "UserAddresses")]
    [OpenApiRequestBody("application/json", typeof(UserAddressUpsertRequestDto), Required = true)]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/{id:Guid}/addresses/{addressId:Guid}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id,
                                         Guid addressId)
    {
      return await UpdateUserAddressAsync(id, addressId, httpRequest, cancellationToken);
    }

    public async Task<IActionResult> UpdateUserAddressAsync(Guid id, Guid addressId,  HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (!UserDtoValidators.IsValidUserAddressUpsertRequest(httpRequest.Content, out UserAddressUpsertRequestDto userAddressUpsertRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(userAddressUpsertRequestDto)} is null or has one or more invalid fields...");
      }

      var result = await _userAddressesMediator.UpdateUserAddressStatusAsync(id, addressId, userAddressUpsertRequestDto, cancellationToken);
      if (result.OperationException == null && !result.IsOperationSuccessful)
      {
        return new NotFoundObjectResult($"A user with the id {id} was not found...");
      }

      return new OkObjectResult(result);
    }
  }
}
