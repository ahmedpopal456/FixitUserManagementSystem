using AutoMapper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Fixit.User.Management.ServerlessApi;
using Fixit.User.Management.Lib.Mappers;
using Fixit.User.Management.Lib.Mediators;
using Fixit.User.Management.Lib.Mediators.Internal;
using Fixit.Core.Database;
using Fixit.Core.Database.Mediators;
using Fixit.Core.Connectors;
using Fixit.Core.Connectors.Mediators;
using Microsoft.Azure.Cosmos;
using Fixit.Core.Connectors.Mediators.GoogleApis;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Fixit.User.Management.ServerlessApi
{
  public class Startup : FunctionsStartup
  {
    private IConfiguration _configuration;

    public override void Configure(IFunctionsHostBuilder builder)
    {
      _configuration = (IConfiguration)builder.Services.BuildServiceProvider()
                                                       .GetService(typeof(IConfiguration));

      var mapperConfig = new MapperConfiguration(mc =>
      {
        mc.AddProfile(new UserManagementMapper());
      });

      DatabaseFactory databaseFactory = new DatabaseFactory(_configuration["FIXIT-UM-DB-EP"], _configuration["FIXIT-UM-DB-KEY"]);
      CosmosClient cosmosClient = new CosmosClient(_configuration["FIXIT-UM-DB-EP"], _configuration["FIXIT-UM-DB-KEY"]);

      builder.Services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
      builder.Services.AddSingleton<IDatabaseMediator>(databaseFactory.CreateCosmosClient());
      builder.Services.AddSingleton<IGoogleApiMediator>(ConnectorFactory.CreateGoogleApiClient(_configuration["FIXIT-UM-CONN-GOOGLEAPI-KEY"]));
      builder.Services.AddSingleton<IMicrosoftGraphMediator>(ConnectorFactory.CreateMicrosoftGraphClient(_configuration["FIXIT-UM-CONN-APPID"], _configuration["FIXIT-UM-CONN-TENANTID"], _configuration["FIXIT-UM-CONN-CLIENTSECRET"]));
      builder.Services.AddSingleton<IUserMediator, UserMediator>(provider =>
      {
        var mapper = provider.GetService<IMapper>();
        var databaseMediator = provider.GetService<IDatabaseMediator>();
        var msGraphMediator = provider.GetService<IMicrosoftGraphMediator>();
        var configuration = provider.GetService<IConfiguration>();
        return new UserMediator(mapper, databaseMediator, msGraphMediator, cosmosClient, configuration);
      });

      builder.Services.AddSingleton<IUserAddressesMediator, UserAddressesMediator>(provider =>
      {
        var mapper = provider.GetService<IMapper>();
        var databaseMediator = provider.GetService<IDatabaseMediator>();
        var configuration = provider.GetService<IConfiguration>();
        return new UserAddressesMediator(mapper, databaseMediator, cosmosClient, configuration);
      });
      
      builder.Services.AddSingleton<IUserRatingsMediator, UserRatingsMediator>(provider =>
      {
        var mapper = provider.GetService<IMapper>();
        var databaseMediator = provider.GetService<IDatabaseMediator>();
        var configuration = provider.GetService<IConfiguration>();
        return new UserRatingsMediator(mapper, databaseMediator, cosmosClient, configuration);
      });

      builder.Services.AddSingleton<IUserSkillMediator, UserSkillMediator>(provider =>
      {
        var mapper = provider.GetService<IMapper>();
        var databaseMediator = provider.GetService<IDatabaseMediator>();
        var configuration = provider.GetService<IConfiguration>();
        return new UserSkillMediator(mapper, databaseMediator, configuration);
      });

    }
  }
}
