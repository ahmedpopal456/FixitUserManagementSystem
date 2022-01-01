using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using AutoMapper;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using Fixit.User.Management.Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;

namespace Fixit.User.Management.ServerlessApi.Functions.Ratings
{
  public class GetAllUserRatings : AzureFunctionRoute
  {
    private readonly IUserRatingsMediator _userRatingMediator;

    public GetAllUserRatings(IUserRatingsMediator userRatingMediator) : base()
    {
      _userRatingMediator = userRatingMediator ?? throw new ArgumentNullException($"{nameof(GetAllUserRatings)} expects a value for {nameof(userRatingMediator)}... null argument was provided");
    }

    [FunctionName("GetAllUserRatings")]
    [OpenApiOperation("get", "UserRating")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/ratings")]
                                         CancellationToken cancellationToken)
    {
      return await GetAllUserRatingsAsync(cancellationToken);
    }

    public async Task<IActionResult> GetAllUserRatingsAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var result = await _userRatingMediator.GetAllUserRatingsAsync(cancellationToken);

      return new OkObjectResult(result);
    }
  }
}

