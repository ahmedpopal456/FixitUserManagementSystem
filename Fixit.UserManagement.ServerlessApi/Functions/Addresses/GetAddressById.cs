using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Fixit.Core.Security.Authorization.AzureFunctions;
using Fixit.User.Management.Lib.Mediators;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading;
using System.Net.Http;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.Core.Connectors.Mediators.GoogleApis;
using Fixit.Core.DataContracts.Users.Address;

namespace Fixit.User.Management.ServerlessApi.Functions.Profile
{
  public class GetAddressById : AzureFunctionRoute
  {
    private readonly IGoogleApiMediator _googleApiMediator;
    private readonly IMapper _mapper;

    public GetAddressById(IGoogleApiMediator googleApiMediator,
                          IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(GetAddressById)} expects a value for {nameof(mapper)}... null argument was provided");
      _googleApiMediator = googleApiMediator ?? throw new ArgumentNullException($"{nameof(GetAddressById)} expects a value for {nameof(googleApiMediator)}... null argument was provided");
    }

    [FunctionName("GetAddressById")]
    [OpenApiOperation("get", "UserAddresses")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "addresses/{id}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         string id)
    {

      return await GetAddressByIdAsync(id, cancellationToken);
    }

    public async Task<IActionResult> GetAddressByIdAsync(string id, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(id))
      {
        return new BadRequestObjectResult($"{nameof(id)} is not valid..");
      }

      var result = await _googleApiMediator.GetAddressByIdAsync(id, cancellationToken);
      if (!result.IsOperationSuccessful)
      {
        if (result.OperationException != null)
        {
          return new BadRequestObjectResult(result);
        }
        return new NotFoundObjectResult($"Address with id {id} could not be found..");
      }

      return new OkObjectResult(result);
    }
  }
}
