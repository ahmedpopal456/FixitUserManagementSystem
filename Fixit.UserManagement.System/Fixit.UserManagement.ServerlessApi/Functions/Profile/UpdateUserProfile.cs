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
using Fixit.User.Management.ServerlessApi.Helpers;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;

namespace Fixit.User.Management.ServerlessApi.Functions.Profile
{
  public class UpdateUserProfile : AzureFunctionRoute
  {
    private readonly IUserMediator _userMediator;
    private readonly IMapper _mapper;

    public UpdateUserProfile(IUserMediator userMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UpdateUserProfile)} expects a value for {nameof(mapper)}... null argument was provided");
      _userMediator = userMediator ?? throw new ArgumentNullException($"{nameof(UpdateUserProfile)} expects a value for {nameof(userMediator)}... null argument was provided");
    }

    [FunctionName("UpdateUserProfile")]
    [OpenApiOperation("put", "UserProfile")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiRequestBody("application/json", typeof(UserProfileUpdateRequestDto), Required = true)]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(UserProfileInformationDto))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{id:Guid}/account/profile")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id)
    {
      return await UpdateUserProfileAsync(httpRequest, id, cancellationToken);
    }

    public async Task<IActionResult> UpdateUserProfileAsync(HttpRequestMessage httpRequest, Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(userId)} is not valid..");
      }

      if (!UserDtoValidators.IsValidUserProfileUpdateRequest(httpRequest.Content, out UserProfileUpdateRequestDto userProfileUpdateRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(userProfileUpdateRequestDto)} is null or has one or more invalid fields...");
      }

      var result = await _userMediator.UpdateUserProfileAsync(userId, userProfileUpdateRequestDto, cancellationToken);
      if (result == null)
      {
        return new NotFoundObjectResult($"Profile of user with id {userId} could not be found..");
      }
      if (!result.IsOperationSuccessful)
      {
        return new BadRequestObjectResult(result);
      }

      return new OkObjectResult(result);
    }
  }
}
