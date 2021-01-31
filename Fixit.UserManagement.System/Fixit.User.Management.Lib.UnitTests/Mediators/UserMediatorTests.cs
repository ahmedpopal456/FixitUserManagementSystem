using System;
using System.Collections.Generic;
using Moq;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.User.Management.Lib.Mediators.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Fixit.Core.Database.Mediators;
using System.Threading.Tasks;
using System.Threading;
using Fixit.User.Management.Lib.Models;
using Fixit.Core.Database.DataContracts.Documents;
using System.Linq.Expressions;
using System.Linq;
using Fixit.Core.Database.DataContracts;

namespace Fixit.User.Management.Lib.UnitTests.Mediators
{
  [TestClass]
  public class UserMediatorTests : TestBase
  {
    private UserMediator _userMediator;

    // Fake data
    private IEnumerable<UserDocument> _fakeUserDocuments;
    private IEnumerable<UserProfileUpdateRequestDto> _fakeUserProfileUpdateRequestDtos;
    private IEnumerable<UserProfilePictureUpdateRequestDto> _fakeUserProfilePictureUpdateRequestDtos;

    // DB and table name

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
      var fakeUserDocumentSeeder = _fakeDtoSeedFactory.CreateFakeSeeder<UserDocument>();
      var fakeUserProfileUpdateRequestDtoSeeder = _fakeDtoSeedFactory.CreateFakeSeeder<UserProfileUpdateRequestDto>();
      var fakeUserProfilePictureUpdateRequestDtoSeeder = _fakeDtoSeedFactory.CreateFakeSeeder<UserProfilePictureUpdateRequestDto>();

      // Create fake data objects
      _fakeUserDocuments = fakeUserDocumentSeeder.SeedFakeDtos();
      _fakeUserProfileUpdateRequestDtos = fakeUserProfileUpdateRequestDtoSeeder.SeedFakeDtos();
      _fakeUserProfilePictureUpdateRequestDtos = fakeUserProfilePictureUpdateRequestDtoSeeder.SeedFakeDtos();

      _databaseMediator.Setup(databaseMediator => databaseMediator.GetDatabase(_userDatabaseName))
                       .Returns(_databaseTableMediator.Object);
      _databaseTableMediator.Setup(databaseTableMediator => databaseTableMediator.GetContainer(_userDatabaseTableName))
                            .Returns(_databaseTableEntityMediator.Object);

      _userMediator = new UserMediator(_mapperConfiguration.CreateMapper(),
                                       _databaseMediator.Object,
                                       _userDatabaseName,
                                       _userDatabaseTableName);
    }
    #endregion

    #region UserAccountTests
    #endregion

    #region UserProfileTests

    #region GetUserProfileAsync
    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task GetUserProfileAsync_UserIdNotFound_ReturnsFailure(string userId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new DocumentCollectionDto<UserDocument>()
      {
        Results = {  },
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      //Act
      var actionResult = await _userMediator.GetUserProfileAsync(userIdGuid, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task GetUserProfileAsync_GetRequestException_ReturnsException(string userId)
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

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      //Act
      var actionResult = await _userMediator.GetUserProfileAsync(userIdGuid, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task GetUserProfileAsync_GetRequestSuccess_ReturnsSuccess(string userId)
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
      var actionResult = await _userMediator.GetUserProfileAsync(userIdGuid, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsNotNull(actionResult.ProfilePictureUrl);
      Assert.IsNotNull(actionResult.FirstName);
      Assert.IsNotNull(actionResult.LastName);
      Assert.IsNotNull(actionResult.Address);
    }
    #endregion

    #region UpdateUserProfileAsync
    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfileAsync_UserIdNotFound_ReturnsFailure(string userId)
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
      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userProfileUpdateRequestDto = _fakeUserProfileUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfileAsync(userIdGuid, userProfileUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfileAsync_GetRequestException_ReturnsException(string userId)
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
      var userProfileUpdateRequestDto = _fakeUserProfileUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfileAsync(userIdGuid, userProfileUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfileAsync_UpdateRequestFailure_ReturnsFailure(string userId)
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
      var operationStatus = new OperationStatus() { IsOperationSuccessful = false };
      var userProfileUpdateRequestDto = _fakeUserProfileUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfileAsync(userIdGuid, userProfileUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfileAsync_UpdateRequestException_ReturnsException(string userId)
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
      var operationStatus = new OperationStatus()
      {
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };
      var userProfileUpdateRequestDto = _fakeUserProfileUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfileAsync(userIdGuid, userProfileUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfileAsync_GetAndUpdateRequestsSuccess_ReturnsSuccess(string userId)
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
      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userProfileUpdateRequestDto = _fakeUserProfileUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfileAsync(userIdGuid, userProfileUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
      Assert.AreEqual(userProfileUpdateRequestDto.FirstName, actionResult.FirstName);
      Assert.AreEqual(userProfileUpdateRequestDto.LastName, actionResult.LastName);
      Assert.AreEqual(userProfileUpdateRequestDto.Address, actionResult.Address);
    }
    #endregion

    #region UpdateUserProfilePictureAsync
    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfilePictureAsync_UserIdNotFound_ReturnsFailure(string userId)
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
      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userProfilePictureUpdateRequestDto = _fakeUserProfilePictureUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfilePictureAsync(userIdGuid, userProfilePictureUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfilePictureAsync_GetRequestException_ReturnsException(string userId)
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
      var userProfilePictureUpdateRequestDto = _fakeUserProfilePictureUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfilePictureAsync(userIdGuid, userProfilePictureUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfilePictureAsync_UpdateRequestFailure_ReturnsFailure(string userId)
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
      var operationStatus = new OperationStatus() { IsOperationSuccessful = false };
      var userProfilePictureUpdateRequestDto = _fakeUserProfilePictureUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfilePictureAsync(userIdGuid, userProfilePictureUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfilePictureAsync_UpdateRequestException_ReturnsException(string userId)
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
      var operationStatus = new OperationStatus()
      {
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };
      var userProfilePictureUpdateRequestDto = _fakeUserProfilePictureUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfilePictureAsync(userIdGuid, userProfilePictureUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserProfilePictureAsync_GetAndUpdateRequestsSuccess_ReturnsSuccess(string userId)
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
      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userProfilePictureUpdateRequestDto = _fakeUserProfilePictureUpdateRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfilePictureAsync(userIdGuid, userProfilePictureUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
      Assert.AreEqual(userProfilePictureUpdateRequestDto.ProfilePictureUrl, actionResult.ProfilePictureUrl);
    }
    #endregion

    #endregion

    #region TestCleanup
    [TestCleanup]
    public void TestCleanup()
    {
      // Clean-up mock objects
      _configuration.Reset();
      _databaseMediator.Reset();
      _databaseTableMediator.Reset();
      _databaseTableEntityMediator.Reset();

      // Clean-up data objects
      _fakeUserProfilePictureUpdateRequestDtos = null;
      _fakeUserProfileUpdateRequestDtos = null;
    }
    #endregion
  }
}
