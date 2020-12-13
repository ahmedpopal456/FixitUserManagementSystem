using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fixit.User.Management.Lib.UnitTests
{
  [TestClass]
  public class TestBase
  {

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
