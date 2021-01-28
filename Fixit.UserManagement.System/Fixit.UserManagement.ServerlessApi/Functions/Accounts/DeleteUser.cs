using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using System.Threading;
using Fixit.Core.DataContracts;

namespace Fixit.User.Management.ServerlessApi.Functions.Accounts
{
  public class DeleteUser : AzureFunctionRoute
  {
    private readonly IUserMediator _userMediator;
    private readonly IMapper _mapper;
    public DeleteUser(IUserMediator userMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(mapper)}... null argument was provided");
      _userMediator = userMediator ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(userMediator)}... null argument was provided");
    }

    [FunctionName("DeleteUser")]
    [OpenApiOperation("delete", "UserAccount")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(OperationStatus))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "{id:Guid}/account")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id)
    {
      return await DeleteUserAsync(id, cancellationToken);
    }

    public async Task<IActionResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(userId)} is not valid..");
      }

      var result = await _userMediator.DeleteUserAsync(userId, cancellationToken);
      if (!result.IsOperationSuccessful && result.OperationException == null)
      {
        return new NotFoundObjectResult($"User with id {userId} could not be found..");
      }

      return new OkObjectResult(result);
    }
  }
}
