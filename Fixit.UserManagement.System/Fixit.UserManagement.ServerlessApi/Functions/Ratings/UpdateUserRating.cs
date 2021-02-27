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
using Fixit.User.Management.ServerlessApi.Helpers;
using Fixit.Core.DataContracts.Users.Ratings;
using Fixit.Core.DataContracts.Users.Operations.Ratings;

namespace Fixit.User.Management.ServerlessApi.Functions.Ratings
{
  class UpdateUserRating : AzureFunctionRoute
  {
    private readonly IUserRatingsMediator _userRatingMediator;
    private readonly IMapper _mapper;

    public UpdateUserRating(IUserRatingsMediator userRatingMediator,
                            IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UpdateUserRating)} expects a value for {nameof(mapper)}... null argument was provided");
      _userRatingMediator = userRatingMediator ?? throw new ArgumentNullException($"{nameof(UpdateUserRating)} expects a value for {nameof(userRatingMediator)}... null argument was provided");
    }

    [FunctionName("UpdateUserRatingAsync")]
    [OpenApiOperation("put", "UserRating")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiRequestBody("application/json", typeof(UserRatingsCreateOrUpdateRequestDto), Required = true)]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(RatingResponseDto))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/{id:Guid}/account/ratings/{ratingId:Guid}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id,
                                         Guid ratingId)
    {
      return await UpdateUserRatingAsync(httpRequest, id, ratingId, cancellationToken);
    }

    public async Task<IActionResult> UpdateUserRatingAsync(HttpRequestMessage httpRequest, Guid userId, Guid ratingId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(UpdateUserRating)} expects a value for {nameof(userId)}... null argument was provided.");
      }
      if (ratingId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(UpdateUserRating)} expects a value for {nameof(ratingId)}... null argument was provided.");
      }

      if (!UserRatingDtoValidators.IsValidUserRatingUpdateRequest(httpRequest.Content, out UserRatingsCreateOrUpdateRequestDto ratingsUpdateRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(ratingsUpdateRequestDto)} is null or has one or more invalid fields...");
      }

      var result = await _userRatingMediator.UpdateUserRatingAsync(userId, ratingsUpdateRequestDto, cancellationToken, null, ratingId);
      if (!result.IsOperationSuccessful)
      {
        return new NotFoundObjectResult($"Rating of user with id {userId} could not be found in {nameof(UpdateUserRating)}...");
      }

      return new OkObjectResult(result);
    }
  }
}
