using System;
using Moq;
using Fixit.Core.DataContracts;
using Fixit.User.Management.Lib.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using Fixit.Core.Database.Mediators;
using Microsoft.Extensions.Configuration;
using Fixit.User.Management.Lib.Mappers;
using Fixit.Core.Connectors.Mediators;
using Fixit.Core.DataContracts.Seeders;

namespace Fixit.User.Management.Lib.UnitTests
{
  [TestClass]
  public class TestBase
  {
    public IFakeSeederFactory _fakeDtoSeedFactory;

    // Main Object Mocks
    protected Mock<IConfiguration> _configuration;

    // Database System Mocks
    protected Mock<IDatabaseMediator> _databaseMediator;
    protected Mock<IDatabaseTableMediator> _databaseTableMediator;
    protected Mock<IDatabaseTableEntityMediator> _databaseTableEntityMediator;

    //Connectors Mocks
    protected Mock<IMicrosoftGraphMediator> _msGraphMediator;

    // Mapper
    protected MapperConfiguration _mapperConfiguration = new MapperConfiguration(config =>
    {
      config.AddProfile(new UserManagementMapper());
    });

    public TestBase()
    {
      _fakeDtoSeedFactory = new FakeDtoSeederFactory();
    }

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
    }

    [AssemblyCleanup]
    public static void AfterSuiteTests()
    {
    }
  }
}
