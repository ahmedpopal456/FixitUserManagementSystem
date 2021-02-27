using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using Fixit.Core.DataContracts.Users.Ratings;
using Fixit.Core.DataContracts.Users.Operations.Ratings;

namespace Fixit.User.Management.ServerlessApi.Functions.Ratings
{
  public class GetUserRating : AzureFunctionRoute
  {
    private readonly IUserRatingsMediator _userRatingMediator;
    private readonly IMapper _mapper;

    public GetUserRating(IUserRatingsMediator userRatingMediator,
                         IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(GetUserRating)} expects a value for {nameof(mapper)}... null argument was provided");
      _userRatingMediator = userRatingMediator ?? throw new ArgumentNullException($"{nameof(GetUserRating)} expects a value for {nameof(userRatingMediator)}... null argument was provided");
    }

    [FunctionName("GetUserRatingAsync")]
    [OpenApiOperation("get", "UserRating")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(RatingResponseDto))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{id:Guid}/account/ratings/{ratingId:Guid}")]
                                          HttpRequestMessage httpRequest,
                                          CancellationToken cancellationToken,
                                          Guid id,
                                          Guid ratingId)
    {
      return await GetUserRatingAsync(id, ratingId, cancellationToken);
    }

    public async Task<IActionResult> GetUserRatingAsync(Guid userId, Guid ratingId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(GetUserRating)} expects a value for {nameof(userId)}... null argument was provided.");
      }
      if (ratingId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(GetUserRating)} expects a value for {nameof(ratingId)}... null argument was provided.");
      }

      var result = await _userRatingMediator.GetUserRatingAsync(userId, ratingId, cancellationToken);
      if (!result.IsOperationSuccessful)
      {
        return new NotFoundObjectResult($"User rating with user with id {userId} could not be found in {nameof(GetUserRating)}...");
      }
      return new OkObjectResult(result);
    }
  }
}
