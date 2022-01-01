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

  public class DeleteUserAddress : AzureFunctionRoute
  {
    private readonly IUserAddressesMediator _userAddressesMediator;
    private readonly IMapper _mapper;
    public DeleteUserAddress(IUserAddressesMediator userAddressesMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(DeleteUserAddress)} expects a value for {nameof(mapper)}... null argument was provided");
      _userAddressesMediator = userAddressesMediator ?? throw new ArgumentNullException($"{nameof(DeleteUserAddress)} expects a value for {nameof(userAddressesMediator)}... null argument was provided");
    }

    [FunctionName("DeleteUserAddress")]
    [OpenApiOperation("delete", "UserAddresses")]
    [OpenApiRequestBody("application/json", typeof(UserAddressUpsertRequestDto), Required = true)]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "users/{id:Guid}/addresses/{addressId:Guid}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id,
                                         Guid addressId)
    {
      return await DeleteUserAddressAsync(id, addressId, httpRequest, cancellationToken);
    }

    public async Task<IActionResult> DeleteUserAddressAsync(Guid id, Guid addressId,  HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var result = await _userAddressesMediator.DeleteUserAddressAsync(id, addressId, cancellationToken);
      if (result.OperationException == null && !result.IsOperationSuccessful)
      {
        return new NotFoundObjectResult($"A user with the id {id} was not found...");
      }

      return new OkObjectResult(result);
    }
  }
}
