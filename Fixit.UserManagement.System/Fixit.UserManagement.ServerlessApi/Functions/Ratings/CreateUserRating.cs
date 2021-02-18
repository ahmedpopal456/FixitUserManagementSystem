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
  public class CreateUserRating
  {
    private readonly IUserRatingsMediator _userRatingMediator;
    private readonly IMapper _mapper;

    public CreateUserRating(IUserRatingsMediator userRatingMediator,
                          IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(CreateUserRating)} expects a value for {nameof(mapper)}... null argument was provided");
      _userRatingMediator = userRatingMediator ?? throw new ArgumentNullException($"{nameof(CreateUserRating)} expects a value for {nameof(userRatingMediator)}... null argument was provided");
    }

    [FunctionName("CreateUserRatingAsync")]
    [OpenApiOperation("post", "UserRating")]
    [OpenApiRequestBody("application/json", typeof(UserRatingsCreateOrUpdateRequestDto), Required = true)]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(RatingResponseDto))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/{id:Guid}/account/ratings")]
                                          HttpRequestMessage httpRequest,
                                          CancellationToken cancellationToken,
                                          Guid id)
    {
      return await CreateUserRatingAsync(httpRequest, id, cancellationToken);
    }

    public async Task<IActionResult> CreateUserRatingAsync(HttpRequestMessage httpRequest, Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(userId)} is not valid...");
      }

      if (!UserRatingDtoValidators.IsValidUserRatingCreateRequest(httpRequest.Content, out UserRatingsCreateOrUpdateRequestDto ratingsCreateRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(ratingsCreateRequestDto)} is null or has one or more invalid fields...");
      }
      
      var result = await _userRatingMediator.CreateUserRatingAsync(userId, ratingsCreateRequestDto, cancellationToken);
      return new OkObjectResult(result);
    }
  }
}
