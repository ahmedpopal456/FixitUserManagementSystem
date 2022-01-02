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
using Fixit.Core.Connectors.Mediators;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.Connectors.DataContracts;
using OperationStatus = Fixit.Core.DataContracts.OperationStatus;
using Fixit.Core.Database.DataContracts;
using Microsoft.Azure.Cosmos;

namespace Fixit.User.Management.Lib.UnitTests.Mediators
{
  [TestClass]
  public class UserMediatorTests : TestBase
  {
    private UserMediator _userMediator;

    // Fake data
    private IEnumerable<UserDocument> _fakeUserDocuments;
    private IEnumerable<UserAccountCreateRequestDto> _fakeUserAccountCreateRequestDtos;
    private IEnumerable<UserAccountStateDto> _fakeUserAccountStateUpdateRequestDtos;
    private IEnumerable<UserAccountResetPasswordRequestDto> _fakeUserAccountResetPasswordeRequestDtos;
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

      _msGraphMediator = new Mock<IMicrosoftGraphMediator>();

      _cosmosClient = new Mock<CosmosClient>();

      // Create Seeders
      _fakeUserDocuments = _fakeDtoSeedFactory.CreateSeederFactory<UserDocument>(new UserDocument());
      _fakeUserAccountCreateRequestDtos = _fakeDtoSeedFactory.CreateSeederFactory(new UserAccountCreateRequestDto());
      _fakeUserAccountStateUpdateRequestDtos = _fakeDtoSeedFactory.CreateSeederFactory(new UserAccountStateDto());
      _fakeUserAccountResetPasswordeRequestDtos = _fakeDtoSeedFactory.CreateSeederFactory(new UserAccountResetPasswordRequestDto());
      _fakeUserProfileUpdateRequestDtos = _fakeDtoSeedFactory.CreateSeederFactory(new UserProfileUpdateRequestDto());
      _fakeUserProfilePictureUpdateRequestDtos = _fakeDtoSeedFactory.CreateSeederFactory(new UserProfilePictureUpdateRequestDto());

      _databaseMediator.Setup(databaseMediator => databaseMediator.GetDatabase(_userDatabaseName))
                       .Returns(_databaseTableMediator.Object);
      _databaseTableMediator.Setup(databaseTableMediator => databaseTableMediator.GetContainer(_userDatabaseTableName))
                       .Returns(_databaseTableEntityMediator.Object);


      _userMediator = new UserMediator(_mapperConfiguration.CreateMapper(),
                                       _databaseMediator.Object,
                                       _msGraphMediator.Object,
                                       _cosmosClient.Object,
                                       _userDatabaseName,
                                       _userDatabaseTableName);
    }
    #endregion

    #region UserAccountTests

    #region CreateUserAsync
    [TestMethod]
    public async Task CreateUserAsync_PrincipalNameExists_ReturnsFailure()
    {
      // Arange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      var documentCollection = new DocumentCollectionDto<UserDocument>()
      {
        Results = { _fakeUserDocuments.First() },
        IsOperationSuccessful = true
      };

      var userAccountCreateRequestDto = _fakeUserAccountCreateRequestDtos.First();
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      // Act
      var actionResult = await _userMediator.CreateUserAsync(userAccountCreateRequestDto, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    public async Task CreateUserAsync_DatabaseGetRequestException_ReturnsException()
    {
      // Arange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      var documentCollection = new DocumentCollectionDto<UserDocument>()
      {
        Results = { },
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };
      var createdDocument = new CreateDocumentDto<UserDocument>()
      {
        IsOperationSuccessful = true,
        Document = _fakeUserDocuments.First()
      };

      var userAccountCreateRequestDto = _fakeUserAccountCreateRequestDtos.First();
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      // Act
      var actionResult = await _userMediator.CreateUserAsync(userAccountCreateRequestDto, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    public async Task CreateUserAsync_DatabaseCreateRequestException_ReturnsException()
    {
      // Arange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      var documentCollection = new DocumentCollectionDto<UserDocument>()
      {
        Results = { },
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };
      var createdDocument = new CreateDocumentDto<UserDocument>()
      {
        IsOperationSuccessful = true,
        Document = _fakeUserDocuments.First()
      };

      var userAccountCreateRequestDto = _fakeUserAccountCreateRequestDtos.First();
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.CreateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(createdDocument);
      // Act
      var actionResult = await _userMediator.CreateUserAsync(userAccountCreateRequestDto, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    public async Task CreateUserAsync_AllRequestsSuccess_ReturnsSuccess()
    {
      // Arange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      var documentCollection = new DocumentCollectionDto<UserDocument>()
      {
        Results = { },
        IsOperationSuccessful = true,
      };

      var createdDocument = new CreateDocumentDto<UserDocument>()
      {
        IsOperationSuccessful = true,
        Document = _fakeUserDocuments.First()
      };
      var userAccountCreateRequestDto = _fakeUserAccountCreateRequestDtos.First();
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.CreateItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(createdDocument);

      // Act
      var actionResult = await _userMediator.CreateUserAsync(userAccountCreateRequestDto, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }
    #endregion

    #region UpdateUserStatusAsync
    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserAccountStatusAsync_UserIdNotFound_ReturnsFailure(string userId)
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
      var userProfileUpdateRequestDto = _fakeUserAccountStateUpdateRequestDtos.First();
      //var connectorDto = new ConnectorDto<UserAccountStateDto>() { Result = _fakeUserAccountStateUpdateRequestDtos.First() };
      var connectorDto = new ConnectorDto<UserAccountStateDto>() { Result = _fakeUserAccountStateUpdateRequestDtos.First() };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);
      //_msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.UpdateAccountSignInStatusAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
      //                          .ReturnsAsync(connectorDto);

      _msGraphMediator.Setup(x => x.UpdateAccountSignInStatusAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.FromResult((Core.Connectors.DataContracts.ConnectorDto<Core.DataContracts.Users.Account.UserAccountStateDto>)connectorDto));


      //Act
      var actionResult = await _userMediator.UpdateUserStatusAsync(userIdGuid, userProfileUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserAccountStatusAsync_GetRequestException_ReturnsFailure(string userId)
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
      var userProfileUpdateRequestDto = _fakeUserAccountStateUpdateRequestDtos.First();
      var connectorDto = new ConnectorDto<UserAccountStateDto>() { Result = _fakeUserAccountStateUpdateRequestDtos.First() };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.UpdateAccountSignInStatusAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(connectorDto);

      //Act
      var actionResult = await _userMediator.UpdateUserStatusAsync(userIdGuid, userProfileUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserAccountStatusAsync_ConnectorException_ReturnsFailure(string userId)
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
      var userStateUpdateRequestDto = _fakeUserAccountStateUpdateRequestDtos.First();

      var connectorDto = new ConnectorDto<UserAccountStateDto>()
      {
        Result = _fakeUserAccountStateUpdateRequestDtos.First(),
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(databaseUpdateOperationStatus);
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.UpdateAccountSignInStatusAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(connectorDto);

      //Act
      var actionResult = await _userMediator.UpdateUserStatusAsync(userIdGuid, userStateUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserAccountStatusAsync_DatabaseUpdateException_ReturnsFailure(string userId)
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
      var userStateUpdateRequestDto = _fakeUserAccountStateUpdateRequestDtos.First();
      var databaseUpdateOperationStatus = new OperationStatus() { IsOperationSuccessful = false, OperationException = new Exception() };
      var connectorUpdateOperationStatus = new ConnectorDto<UserAccountStateDto>()
      {
        Result = _fakeUserAccountStateUpdateRequestDtos.First(),
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(databaseUpdateOperationStatus);
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.UpdateAccountSignInStatusAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(connectorUpdateOperationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserStatusAsync(userIdGuid, userStateUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserAccountStatusAsync_AllRequestsSuccess_ReturnsSuccess(string userId)
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
      var userStateUpdateRequestDto = _fakeUserAccountStateUpdateRequestDtos.First();

      var connectorDto = new ConnectorDto<UserAccountStateDto>()
      {
        Result = _fakeUserAccountStateUpdateRequestDtos.First(),
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(databaseUpdateOperationStatus);
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.UpdateAccountSignInStatusAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(connectorDto);

      //Act
      var actionResult = await _userMediator.UpdateUserStatusAsync(userIdGuid, userStateUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
      Assert.AreEqual(userStateUpdateRequestDto.State, actionResult.State);
    }

    #endregion

    #region DeleteUserAsync

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task DeleteUserAsync_UserIdNotFound_ReturnsFailure(string userId)
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
      var databaseDeleteOperationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var connectorDeleteOperationStatus = new OperationStatus() { IsOperationSuccessful = true };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.DeleteItemAsync<UserDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(databaseDeleteOperationStatus);
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(connectorDeleteOperationStatus);

      //Act
      var actionResult = await _userMediator.DeleteUserAsync(userIdGuid, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task DeleteUserAsync_DatabaseDeleteException_ReturnsFailure(string userId)
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
      var databaseDeleteOperationStatus = new OperationStatus()
      {
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };
      var connectorDeleteOperationStatus = new OperationStatus() { IsOperationSuccessful = true };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.DeleteItemAsync<UserDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(databaseDeleteOperationStatus);
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(connectorDeleteOperationStatus);

      //Act
      var actionResult = await _userMediator.DeleteUserAsync(userIdGuid, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task DeleteUserAsync_GetUserException_ReturnsFailure(string userId)
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
      var databaseDeleteOperationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var connectorDeleteOperationStatus = new OperationStatus() { IsOperationSuccessful = true };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.DeleteItemAsync<UserDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(databaseDeleteOperationStatus);
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(connectorDeleteOperationStatus);

      //Act
      var actionResult = await _userMediator.DeleteUserAsync(userIdGuid, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task DeleteUserAsync_ConnectorDeleteException_ReturnsFailure(string userId)
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
      var databaseDeleteOperationStatus = new OperationStatus() { IsOperationSuccessful = true };

      var connectorDeleteOperationStatus = new OperationStatus()
      {
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.DeleteItemAsync<UserDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(databaseDeleteOperationStatus);
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(connectorDeleteOperationStatus);

      //Act
      var actionResult = await _userMediator.DeleteUserAsync(userIdGuid, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task DeleteUserAsync_AllRequestsSuccess_ReturnsSuccess(string userId)
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
      var databaseDeleteOperationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var connectorDeleteOperationStatus = new OperationStatus() { IsOperationSuccessful = true };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.DeleteItemAsync<UserDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(databaseDeleteOperationStatus);
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(connectorDeleteOperationStatus);

      //Act
      var actionResult = await _userMediator.DeleteUserAsync(userIdGuid, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }
    #endregion
    #region ResetPasswordAsync

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserPasswordAsync_UserIdNotFound_ReturnsFailure(string userId)
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
      var connectorUpdateOperationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var resetPasswordRequestDto = _fakeUserAccountResetPasswordeRequestDtos.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.UpdateAccountPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(connectorUpdateOperationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserPasswordAsync(userIdGuid, resetPasswordRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserPasswordAsync_GetUserException_ReturnsFailure(string userId)
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
      var connectorUpdateOperationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var resetPasswordRequestDto = _fakeUserAccountResetPasswordeRequestDtos.First();


      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.UpdateAccountPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(connectorUpdateOperationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserPasswordAsync(userIdGuid, resetPasswordRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserPasswordAsync_ConnectorDeleteException_ReturnsFailure(string userId)
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

      var connectorUpdateOperationStatus = new OperationStatus()
      {
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };
      var resetPasswordRequestDto = _fakeUserAccountResetPasswordeRequestDtos.First();


      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.UpdateAccountPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(connectorUpdateOperationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserPasswordAsync(userIdGuid, resetPasswordRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("db53bc7b-2892-47a9-9134-3696579878df", DisplayName = "Any_UserId")]
    public async Task UpdateUserPasswordAsync_AllRequestsSuccess_ReturnsSuccess(string userId)
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
      var connectorUpdateOperationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var resetPasswordRequestDto = _fakeUserAccountResetPasswordeRequestDtos.First();


      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<UserDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));
      _msGraphMediator.Setup(msGraphClientMediator => msGraphClientMediator.UpdateAccountPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(connectorUpdateOperationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserPasswordAsync(userIdGuid, resetPasswordRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    #endregion

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
        Results = { },
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userMediator.UpdateUserProfileAsync(userIdGuid, userProfileUpdateRequestDto, cancellationToken);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
      Assert.AreEqual(userProfileUpdateRequestDto.FirstName, actionResult.FirstName);
      Assert.AreEqual(userProfileUpdateRequestDto.LastName, actionResult.LastName);
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<UserDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
      _msGraphMediator.Reset();

      // Clean-up data objects
      _fakeUserProfilePictureUpdateRequestDtos = null;
      _fakeUserProfileUpdateRequestDtos = null;
    }
    #endregion
  }
}
