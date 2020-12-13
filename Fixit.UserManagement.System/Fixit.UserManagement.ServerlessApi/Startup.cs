using AutoMapper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Fixit.User.Management.ServerlessApi;
using Fixit.User.Management.Lib.Mappers;
using Fixit.User.Management.Lib.Mediators;
using Fixit.User.Management.Lib.Mediators.Internal;

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

      builder.Services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
      builder.Services.AddTransient<IUserMediator, UserMediator>();
    }
  }
}
