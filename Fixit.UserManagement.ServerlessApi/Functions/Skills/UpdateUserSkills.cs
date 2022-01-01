using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using AutoMapper;
using Fixit.Core.DataContracts.Users.Details.Craftsman;
using Fixit.Core.DataContracts.Users.Operations;
using Fixit.Core.DataContracts.Users.Operations.Skills;
using Fixit.Core.DataContracts.Users.Skill;
using Fixit.User.Management.Lib.Mediators;
using Fixit.User.Management.ServerlessApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;

namespace Fixit.User.Management.ServerlessApi.Functions.Skills
{
  public class UpdateUserSkills
  {
    private readonly IUserSkillMediator _userSkillMediator;
    private readonly IMapper _mapper;

    public UpdateUserSkills(IUserSkillMediator userSkillMediator, IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UpdateUserSkills)} expects a value for {nameof(mapper)}... null argument was provided");
      _userSkillMediator = userSkillMediator ?? throw new ArgumentNullException($"{nameof(GetUserSkills)} expects a value for {nameof(_userSkillMediator)}... null argument was provided");
    }

    [FunctionName("UpdateUserSkillsAsync")]
    [OpenApiOperation("put", "UserSkill")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/{id:Guid}/skills")]
                                          HttpRequestMessage requestMessage,
                                          CancellationToken cancellationToken,
                                          Guid id)
    {
      return await UpdateUserSkillAsync(id, requestMessage, cancellationToken);
    }

    private async Task<IActionResult> UpdateUserSkillAsync(Guid userId, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (userId.Equals(Guid.Empty))
      {
        return new BadRequestObjectResult($"{nameof(userId)} is not valid..");
      }

      if (!UserSkillDtoValidator.IsValidUserSkillUpdateRequest(requestMessage.Content, out UpdateUserSkillRequestDto userSkillRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(UpdateUserSkillRequestDto)} is null or has one or more invalid fields...");
      }

      var result = await _userSkillMediator.UpdateUserSkillAsync(userId, userSkillRequestDto, cancellationToken);

      return new OkObjectResult(result);
    }
  }
}
