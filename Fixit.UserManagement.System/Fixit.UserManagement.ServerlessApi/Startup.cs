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
      ConnectorFactory connectorFactory = new ConnectorFactory(_configuration["FIXIT-UM-CONN-APPID"], _configuration["FIXIT-UM-CONN-TENANTID"], _configuration["FIXIT-UM-CONN-CLIENTSECRET"]);

      builder.Services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
      builder.Services.AddSingleton<IDatabaseMediator>(databaseFactory.CreateCosmosClient());
      builder.Services.AddSingleton<IMicrosoftGraphMediator>(connectorFactory.CreateMicrosoftGraphClient());
      builder.Services.AddSingleton<IUserMediator, UserMediator>(provider =>
      {
        var mapper = provider.GetService<IMapper>();
        var databaseMediator = provider.GetService<IDatabaseMediator>();
        var msGraphMediator = provider.GetService<IMicrosoftGraphMediator>();
        var configuration = provider.GetService<IConfiguration>();
        return new UserMediator(mapper, databaseMediator, msGraphMediator, configuration);
      });
    }
  }
}
