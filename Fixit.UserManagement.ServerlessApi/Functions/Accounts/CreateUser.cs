using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using Fixit.User.Management.ServerlessApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;

namespace Fixit.User.Management.ServerlessApi.Functions.Accounts
{

  public class CreateUser : AzureFunctionRoute
  {
    private readonly IUserMediator _userMediator;
    private readonly IMapper _mapper;
    public CreateUser(IUserMediator userMediator,
                     IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(mapper)}... null argument was provided");
      _userMediator = userMediator ?? throw new ArgumentNullException($"{nameof(CreateUser)} expects a value for {nameof(userMediator)}... null argument was provided");
    }

    [FunctionName("CreateUser")]
    [OpenApiOperation("post", "UserAccount")]
    [OpenApiRequestBody("application/json", typeof(UserAccountCreateRequestDto), Required = true)]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/me/account")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken)
    {
      return await CreateUserAsync(httpRequest, cancellationToken);
    }

    public async Task<IActionResult> CreateUserAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (!UserDtoValidators.IsValidUserCreationRequest(httpRequest.Content, out UserAccountCreateRequestDto userAccountCreateRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(userAccountCreateRequestDto)} is null or has one or more invalid fields...");
      }

      var result = await _userMediator.CreateUserAsync(userAccountCreateRequestDto, cancellationToken);
      if (!result.IsOperationSuccessful && result.OperationException == null)
      {
        return new BadRequestObjectResult($"A User with the id {userAccountCreateRequestDto.Id} already exists...");
      }

      return new OkObjectResult(result);
    }
  }
}
