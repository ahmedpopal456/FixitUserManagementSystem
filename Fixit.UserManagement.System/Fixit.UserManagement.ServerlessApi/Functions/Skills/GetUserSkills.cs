using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using AutoMapper;
using Fixit.Core.DataContracts.Users.Details.Craftsman;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;

namespace Fixit.User.Management.ServerlessApi.Functions
{
  public class GetUserSkills : AzureFunctionRoute
  {
    private readonly IUserSkillMediator _userSkillMediator;
    private readonly IMapper _mapper;

    public GetUserSkills(IUserSkillMediator userSkillMediator, IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(GetUserSkills)} expects a value for {nameof(mapper)}... null argument was provided");
      _userSkillMediator = userSkillMediator ?? throw new ArgumentNullException($"{nameof(GetUserSkills)} expects a value for {nameof(_userSkillMediator)}... null argument was provided");
    }

    [FunctionName("GetUserSkillsAsync")]
    [OpenApiOperation("get", "UserSkill")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(SkillDto))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{id:Guid}/skills")]
                                          CancellationToken cancellationToken,
                                          Guid id)
    {
      return await GetUserSkillAsync(id, cancellationToken);
    }

    private async Task<IActionResult> GetUserSkillAsync(Guid userId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(userId)} is not valid..");
      }

      var result = await _userSkillMediator.GetUserSkillAsync(userId, cancellationToken);

      return new OkObjectResult(result);
    }
  }
}
