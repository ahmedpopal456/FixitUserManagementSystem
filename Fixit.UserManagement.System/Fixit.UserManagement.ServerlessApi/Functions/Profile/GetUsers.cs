using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using AutoMapper;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using Fixit.User.Management.Lib.Models;
using Fixit.User.Management.ServerlessApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Fixit.User.Management.ServerlessApi.Functions.Profile
{
  public class GetUsers : AzureFunctionRoute
  {
    private readonly IUserMediator _userMediator;

    public GetUsers(IUserMediator userMediator) : base()
    {
      _userMediator = userMediator ?? throw new ArgumentNullException($"{nameof(GetUsers)} expects a value for {nameof(userMediator)}... null argument was provided");
    }

    [FunctionName("GetUsers")]
    [OpenApiOperation("get", "Users")]
    [OpenApiParameter("entityId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(UserDocument))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{entityId}")]
                                         HttpRequestMessage httpRequest, 
                                         string entityId,
                                         CancellationToken cancellationToken)
    {

      return await GetUsersAsync(entityId, cancellationToken);
    }

    public async Task<IActionResult> GetUsersAsync(string entityId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var entity = default(string);

      if (entityId == null || !UserDtoValidators.IsValidEntityId(entityId, out entity))
      {
        return new BadRequestObjectResult($"{nameof(entityId)} is null or invalid, can only evaluate 'Craftsman' or 'Client' entities...");
      }

      var result = await _userMediator.GetUsersAsync(entity, cancellationToken);

      return new OkObjectResult(result);
    }
  }
}

