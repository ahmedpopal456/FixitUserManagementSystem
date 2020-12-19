using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Fixit.User.Management.Lib.Models.Profile;
using System.Net.Http;
using System.Threading;
using Fixit.User.Management.ServerlessApi.Helpers;
using Fixit.Core.DataContracts.Users.Operations.Profile;

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
    [OpenApiRequestBody("application/json", typeof(UserProfileInformationDto), Required = true)] // change this to UpdateUserProfileRequestDto
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(UserProfileInformationDto))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "userManagement/{id:Guid}/account/profile")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid id)
    {
      return await UpdateUserProfileAsync(httpRequest, id, cancellationToken);
    }

    public async Task<IActionResult> UpdateUserProfileAsync(HttpRequestMessage httpRequest, Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId == Guid.Empty)
      {
        return new BadRequestObjectResult($"{nameof(userId)} is not a valid {nameof(Guid)}..");
      }

      if (!UserDtoValidators.IsValidUserProfileUpdateRequest(httpRequest.Content, out UpdateUserProfileRequestDto userProfileInformationDto))
      {
        return new BadRequestObjectResult($"Either {nameof(userProfileInformationDto)} is null or has one or more invalid fields...");
      }

      UserProfileInformationDto result = null;
      try
      {
        result = await _userMediator.UpdateUserProfileAsync(userId, userProfileInformationDto, cancellationToken);

        if (result == null)
        {
          return new NotFoundObjectResult($"Profile of user with id {userId} could not be found..");
        }
      }
      catch(Exception exception)
      {
        return new BadRequestObjectResult(exception);
      }

      return new OkObjectResult(result);
    }
  }
}
