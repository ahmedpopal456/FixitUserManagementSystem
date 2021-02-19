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
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Operations.Account;

namespace Fixit.User.Management.ServerlessApi.Functions.Accounts
{
  public class UpdateUserAccountPassword : AzureFunctionRoute
  {
    private readonly IUserMediator _userMediator;
    private readonly IMapper _mapper;
    public UpdateUserAccountPassword(IUserMediator userMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(mapper)}... null argument was provided");
      _userMediator = userMediator ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(userMediator)}... null argument was provided");
    }

    [FunctionName("UpdateUserAccountPassword")]
    [OpenApiOperation("put", "UserAccount")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiRequestBody("application/json", typeof(UserAccountResetPasswordRequestDto), Required = true)]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(OperationStatus))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{id:Guid}/account/security/resetPassword")]
                                          HttpRequestMessage httpRequest,
                                          CancellationToken cancellationToken,
                                          Guid id)
    {
      return await UpdateUserAccountPasswordAsync(httpRequest, id, cancellationToken);
    }

    public async Task<IActionResult> UpdateUserAccountPasswordAsync(HttpRequestMessage httpRequest, Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(userId)} is not valid..");
      }

      if (!UserDtoValidators.IsValidUserPasswordUpdateRequest(httpRequest.Content, out UserAccountResetPasswordRequestDto userAccountResetPasswordRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(userAccountResetPasswordRequestDto)} is null or has one or more invalid fields...");
      }

      OperationStatus result = await _userMediator.UpdateUserPasswordAsync(userId, userAccountResetPasswordRequestDto, cancellationToken);
      if (!result.IsOperationSuccessful && result.OperationException == null)
      {
        return new NotFoundObjectResult($"User with id {userId} could not be found..");
      }
      return new OkObjectResult(result);
    }
  }
}
