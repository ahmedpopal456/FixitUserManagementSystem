using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;
using System;
using System.Web;
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
  public class GetUserRatingsWithAverage : AzureFunctionRoute
  {
    private readonly IUserRatingsMediator _userRatingMediator;
    private readonly IMapper _mapper;

    public GetUserRatingsWithAverage(IUserRatingsMediator userRatingMediator,
                         IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(GetUserRatingsWithAverage)} expects a value for {nameof(mapper)}... null argument was provided");
      _userRatingMediator = userRatingMediator ?? throw new ArgumentNullException($"{nameof(GetUserRatingsWithAverage)} expects a value for {nameof(userRatingMediator)}... null argument was provided");
    }

    [FunctionName("GetUserRatingAverageAsync")]
    [OpenApiOperation("get", "UserRating")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("minTimestampUtc", In = ParameterLocation.Query, Required = false, Type = typeof(string))]
    [OpenApiParameter("maxTimestampUtc", In = ParameterLocation.Query, Required = false, Type = typeof(string))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{id:Guid}/account/ratings")]
                                          HttpRequestMessage httpRequest,
                                          CancellationToken cancellationToken,
                                          Guid id)
    {
      int.TryParse(HttpUtility.ParseQueryString(httpRequest.RequestUri.Query).Get("pageSize"), out var parsedPageSize);
      var minTimestampUtc = HttpUtility.ParseQueryString(httpRequest.RequestUri.Query).Get("minTimestampUtc");
      var maxTimestampUtc = HttpUtility.ParseQueryString(httpRequest.RequestUri.Query).Get("maxTimestampUtc");
      return await GetUserRatingAverageAsync(id, cancellationToken, minTimestampUtc, maxTimestampUtc);
    }

    public async Task<IActionResult> GetUserRatingAverageAsync(Guid userId, CancellationToken cancellationToken, string minTimestampUtc, string maxTimestampUtc)
    {
      cancellationToken.ThrowIfCancellationRequested();

      long? minTimestampUtcResult = default;
      long? maxTimestampUtcResult = default;

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult(($"{nameof(GetUserRatingsWithAverage)} expects a value for {nameof(userId)}... null argument was provided."));
      }

      if ((minTimestampUtc != null && !OptionalQueryValidators.TryParseTimestampUtc(minTimestampUtc, out minTimestampUtcResult))
          || (maxTimestampUtc != null && !OptionalQueryValidators.TryParseTimestampUtc(maxTimestampUtc, out maxTimestampUtcResult)))
      {
        return new BadRequestObjectResult($"Either {nameof(minTimestampUtc)} or {nameof(maxTimestampUtc)} is invalid, cannot validate TimestampUtc...");
      }

      var result = await _userRatingMediator.GetUserRatingsWithAverageAsync(userId, cancellationToken, minTimestampUtcResult, maxTimestampUtcResult);

      if (!result.IsOperationSuccessful)
      {
        return new NotFoundObjectResult($"User rating with user with id {userId} could not be found in {nameof(GetUserRatingsWithAverage)}...");
      }
      return new OkObjectResult(result);
    }
  }
}
