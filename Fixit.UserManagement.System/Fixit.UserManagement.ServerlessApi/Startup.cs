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

      DatabaseFactory factory = new DatabaseFactory(_configuration["FIXIT-CDB-EP"], _configuration["FIXIT-CDB-CS"]);

      builder.Services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
      builder.Services.AddSingleton<IDatabaseMediator>(factory.CreateCosmosClient());
      builder.Services.AddSingleton<IUserMediator, UserMediator>(provider =>
      {
        var mapper = provider.GetService<IMapper>();
        var databaseMediator = provider.GetService<IDatabaseMediator>();
        var configuration = provider.GetService<IConfiguration>();
        return new UserMediator(mapper, databaseMediator, configuration);
      });
    }
  }
}
