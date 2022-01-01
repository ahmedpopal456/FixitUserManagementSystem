using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Fixit.Core.Security.Authorization.AzureFunctions;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading;
using System.Net.Http;
using Fixit.Core.Connectors.Mediators.GoogleApis;
using Fixit.Core.DataContracts.Users.Address.Query;
using System.Collections.Generic;

namespace Fixit.User.Management.ServerlessApi.Functions.Profile
{
  public class GetAddressBySearch : AzureFunctionRoute
  {
    private readonly IGoogleApiMediator _googleApiMediator;
    private readonly IMapper _mapper;

    public GetAddressBySearch(IGoogleApiMediator googleApiMediator,
                          IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(GetAddressBySearch)} expects a value for {nameof(mapper)}... null argument was provided");
      _googleApiMediator = googleApiMediator ?? throw new ArgumentNullException($"{nameof(GetAddressBySearch)} expects a value for {nameof(googleApiMediator)}... null argument was provided");
    }

    [FunctionName("GetAddressBySearch")]
    [OpenApiOperation("get", "UserAddresses")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "addresses/search/{searchText}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         string searchText)
    {

      return await GetAddressBySearchAsync(searchText, cancellationToken);
    }

    public async Task<IActionResult> GetAddressBySearchAsync(string searchText, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(searchText))
      {
        return new BadRequestObjectResult($"{nameof(searchText)} is not valid..");
      }

      var result = await _googleApiMediator.GetAddressesBySearchAsync(searchText, cancellationToken);
      return new OkObjectResult(result);
    }
  }
}
