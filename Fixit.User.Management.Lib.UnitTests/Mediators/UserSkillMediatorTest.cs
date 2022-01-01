using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Users.Operations;
using Fixit.Core.DataContracts.Users.Operations.Skills;
using Fixit.User.Management.Lib.Mediators.Internal;
using Fixit.User.Management.Lib.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fixit.User.Management.Lib.UnitTests.Mediators
{
  [TestClass]
  public class UserSkillMediatorTest : TestBase
  {
    private UserSkillMediator _userSkillMediator;

    private IEnumerable<UserDocument> _fakeUserDocuments;
    private IEnumerable<UpdateUserSkillRequestDto> _fakeUserSkillUpdateDocument;

    private readonly string _userDatabaseName = "TestDatabaseName";
    private readonly string _userDatabaseTableName = "TestTableName";

    #region TestInitialize
    [TestInitialize]
    public void TestInitialize()
    {
      // Setup all needed Interfaces to project test controllers
      _configuration = new Mock<IConfiguration>();
      _databaseMediator = new Mock<IDatabaseMediator>();
      _databaseTableMediator = new Mock<IDatabaseTableMediator>();
      _databaseTableEntityMediator = new Mock<IDatabaseTableEntityMediator>();

      // Create Seeders
      _fakeUserDocuments = _fakeDtoSeedFactory.CreateSeederFactory<UserDocument>(new UserDocument());
      _fakeUserSkillUpdateDocument = _fakeDtoSeedFactory.CreateSeederFactory(new UpdateUserSkillRequestDto());

      _databaseMediator.Setup(databaseMediator => databaseMediator.GetDatabase(_userDatabaseName))
                       .Returns(_databaseTableMediator.Object);
      _databaseTableMediator.Setup(databaseTableMediator => databaseTableMediator.GetContainer(_userDatabaseTableName))
                       .Returns(_databaseTableEntityMediator.Object);

      _userSkillMediator = new UserSkillMediator(_mapperConfiguration.CreateMapper(), _databaseMediator.Object, _userDatabaseName, _userDatabaseTableName);
    }
    #endregion

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task GetUserSkillAsync_UserIdNotFound_ReturnsFailure(string userId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new DocumentCollectionDto<UserDocument>()
      {
        Results = { },
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      await Assert.ThrowsExceptionAsync<NullReferenceException>(async () => await _userSkillMediator.GetUserSkillAsync(userIdGuid, cancellationToken));
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task GetUserSkillAsync_GetRequestSuccess_ReturnsSuccess(string userId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new DocumentCollectionDto<UserDocument>()
      {
        Results = { _fakeUserDocuments.First() },
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      //Act
      var actionResult = await _userSkillMediator.GetUserSkillAsync(userIdGuid, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsNotNull(actionResult.FirstOrDefault().Id);
      Assert.IsNotNull(actionResult.FirstOrDefault().Name);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserSKillAsync_GetRequestException_ReturnsFailure(string userId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new DocumentCollectionDto<UserDocument>()
      {
        Results = { },
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };
      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userSkillUpdateRequestDto = _fakeUserSkillUpdateDocument.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      await Assert.ThrowsExceptionAsync<NullReferenceException>(async () => await _userSkillMediator.UpdateUserSkillAsync(userIdGuid, userSkillUpdateRequestDto, cancellationToken));
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserSkillAsync_AllRequestsSuccess_ReturnsSuccess(string userId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new DocumentCollectionDto<UserDocument>()
      {
        Results = { _fakeUserDocuments.First() },
        IsOperationSuccessful = true
      };
      var databaseUpdateOperationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userStateUpdateRequestDto = _fakeUserSkillUpdateDocument.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(databaseUpdateOperationStatus);

      //Act
      var actionResult = await _userSkillMediator.UpdateUserSkillAsync(userIdGuid, userStateUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsNull(actionResult.OperationException);
    }
  }
}
