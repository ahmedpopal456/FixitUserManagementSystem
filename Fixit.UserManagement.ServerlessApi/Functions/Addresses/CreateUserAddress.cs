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

namespace Fixit.User.Management.ServerlessApi.Functions.Accounts
{

  public class CreateUserAddress : AzureFunctionRoute
  {
    private readonly IUserAddressesMediator _userAddressesMediator;
    private readonly IMapper _mapper;
    public CreateUserAddress(IUserAddressesMediator userAddressesMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(mapper)}... null argument was provided");
      _userAddressesMediator = userAddressesMediator ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(userAddressesMediator)}... null argument was provided");
    }

    [FunctionName("CreateUserAddress")]
    [OpenApiOperation("post", "UserAddresses")]
    [OpenApiRequestBody("application/json", typeof(UserAddressUpsertRequestDto), Required = true)]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/{id:Guid}/addresses")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id)
    {
      return await CreateUserAddressAsync(id, httpRequest, cancellationToken);
    }

    public async Task<IActionResult> CreateUserAddressAsync(Guid id, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (!UserDtoValidators.IsValidUserAddressUpsertRequest(httpRequest.Content, out UserAddressUpsertRequestDto userAddressUpsertRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(userAddressUpsertRequestDto)} is null or has one or more invalid fields...");
      }

      var result = await _userAddressesMediator.CreateUserAddressAsync(id, userAddressUpsertRequestDto, cancellationToken);
      if (result.OperationException == null && !result.IsOperationSuccessful)
      {
        return new NotFoundObjectResult($"A user with the id {id} was not found...");
      }

      return new OkObjectResult(result);
    }
  }
}
