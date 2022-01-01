using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading;
using System.Net.Http;
using Fixit.Core.DataContracts.Users.Operations.Profile;

namespace Fixit.User.Management.ServerlessApi.Functions.Profile
{
  public class GetUserSummary : AzureFunctionRoute
  {
    private readonly IUserMediator _userMediator;
    private readonly IMapper _mapper;

    public GetUserSummary(IUserMediator userMediator,
                          IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(GetUserSummary)} expects a value for {nameof(mapper)}... null argument was provided");
      _userMediator = userMediator ?? throw new ArgumentNullException($"{nameof(GetUserSummary)} expects a value for {nameof(userMediator)}... null argument was provided");
    }

    [FunctionName("GetUserSummary")]
    [OpenApiOperation("get", "UserSummary")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{id:Guid}/account/profile/summary")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id)
    {
      return await GetUserSummaryAsync(id, cancellationToken);
    }

    public async Task<IActionResult> GetUserSummaryAsync(Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(userId)} is not valid..");
      }

      var result = await _userMediator.GetUserSummaryAsync(userId, cancellationToken);
      if (!result.IsOperationSuccessful)
      {
        if (result.OperationException != null)
        {
          return new BadRequestObjectResult(result);
        }
        return new NotFoundObjectResult($"Summary of user with id {userId} could not be found..");
      }

      return new OkObjectResult(result);
    }
  }
}
