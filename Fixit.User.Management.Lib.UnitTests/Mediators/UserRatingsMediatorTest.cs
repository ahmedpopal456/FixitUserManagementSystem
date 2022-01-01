using System;
using System.Collections.Generic;
using Moq;
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
using OperationStatus = Fixit.Core.DataContracts.OperationStatus;
using Fixit.Core.DataContracts.Users.Operations.Ratings;
using Microsoft.Azure.Cosmos;

namespace Fixit.User.Management.Lib.UnitTests.Mediators
{
  [TestClass]
  public class UserRatingsMediatorTest : TestBase
  {
    private UserRatingsMediator _userRatingsMediator;

    // Fake data
    private IEnumerable<RatingsDocument> _fakeUserRatingsDocuments;
    private IEnumerable<UserRatingsCreateOrUpdateRequestDto> _fakeUserRatingsCreateOrUpdateRequestDto;

    // Db and table name
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

      _cosmosClient = new Mock<CosmosClient>();

      // Create Seeders
      _fakeUserRatingsDocuments = _fakeDtoSeedFactory.CreateSeederFactory<RatingsDocument>(new RatingsDocument());
      _fakeUserRatingsCreateOrUpdateRequestDto = _fakeDtoSeedFactory.CreateSeederFactory(new UserRatingsCreateOrUpdateRequestDto());

      _databaseMediator.Setup(databaseMediator => databaseMediator.GetDatabase(_userDatabaseName))
                       .Returns(_databaseTableMediator.Object);
      _databaseTableMediator.Setup(databaseTableMediator => databaseTableMediator.GetContainer(_userDatabaseTableName))
                       .Returns(_databaseTableEntityMediator.Object);

      _userRatingsMediator = new UserRatingsMediator(_mapperConfiguration.CreateMapper(),
                                                     _databaseMediator.Object,
                                                     _cosmosClient.Object,
                                                     _userDatabaseName,
                                                     _userDatabaseTableName);
    }
    #endregion

    #region UserRatingsTests

    #region GetUserRatingsAsync
    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", "f89ca93e-1814-4b73-a18b-e386415f4f82", DisplayName = "Any_UserId, Any_RatingId")]
    public async Task GetUserRatingAsync_UserIdNotFound_ReturnsFailure(string userId, string ratingId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      Guid ratingIdGuid = new Guid(ratingId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { },
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                    .ReturnsAsync((documentCollection, continuationToken));

      // Act
      var actionResult = await _userRatingsMediator.GetUserRatingAsync(userIdGuid, ratingIdGuid, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", "f89ca93e-1814-4b73-a18b-e386415f4f82", DisplayName = "Any_UserId, Any_RatingId")]
    public async Task GetUserRatingAsync_GetRequestException_ReturnsException(string userId, string ratingId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      Guid ratingIdGuid = new Guid(ratingId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { },
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                    .ReturnsAsync((documentCollection, continuationToken));

      // Act
      var actionResult = await _userRatingsMediator.GetUserRatingAsync(userIdGuid, ratingIdGuid, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", "f89ca93e-1814-4b73-a18b-e386415f4f82", DisplayName = "Any_UserId, Any_RatingId")]
    public async Task GetUserRatingAsync_GetRequestSuccess_ReturnsSuccess(string userId, string ratingId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      Guid ratingIdGuid = new Guid(ratingId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { _fakeUserRatingsDocuments.First() },
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                    .ReturnsAsync((documentCollection, continuationToken));

      // Act
      var actionResult = await _userRatingsMediator.GetUserRatingAsync(userIdGuid, ratingIdGuid, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsNotNull(actionResult.Rating.Comment);
      Assert.IsNotNull(actionResult.Rating.Score);
    }
    #endregion

    #region GetUserRatingAverageAsync
    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", DisplayName = "Any_UserId")]
    public async Task GetUserRatingAverageAsync_UserIdNotFound_ReturnsFailure(string userId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { },
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                    .ReturnsAsync((documentCollection, continuationToken));

      // Act
      var actionResult = await _userRatingsMediator.GetUserRatingsWithAverageAsync(userIdGuid, cancellationToken, null, null);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", DisplayName = "Any_UserId")]
    public async Task GetUserRatingAverageAsync_GetRequestException_ReturnsException(string userId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { },
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                    .ReturnsAsync((documentCollection, continuationToken));

      // Act
      var actionResult = await _userRatingsMediator.GetUserRatingsWithAverageAsync(userIdGuid, cancellationToken, null, null);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", DisplayName = "Any_UserId")]
    public async Task GetUserRatingAverageAsync_GetRequestSuccess_ReturnsSuccess(string userId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { _fakeUserRatingsDocuments.First() },
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                    .ReturnsAsync((documentCollection, continuationToken));

      // Act
      var actionResult = await _userRatingsMediator.GetUserRatingsWithAverageAsync(userIdGuid, cancellationToken, null);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsNotNull(actionResult.Ratings);
    }
    #endregion

    #region GetPagedUserRatingsWithAverageAsync
    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", 1, 20, DisplayName = "Any_UserId, Any_PageNumber, Any_PageSize")]
    public async Task GetPagedUserRatingsWithAverageAsync_UserIdNotFound_ReturnsFailure(string userId, int pageSize, int pageNumber)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new PagedDocumentCollectionDto<RatingsDocument>()
      {
        Results = { },
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableByPageAsync<RatingsDocument>(pageNumber, It.IsAny<Microsoft.Azure.Cosmos.QueryRequestOptions>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>()))
                                    .ReturnsAsync(documentCollection);

      // Act
      var actionResult = await _userRatingsMediator.GetPagedUserRatingsWithAverageAsync(userIdGuid, pageNumber, cancellationToken, pageSize, null, null);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", 1, 20, DisplayName = "Any_UserId, Any_PageNumber, Any_PageSize")]
    public async Task GetPagedUserRatingsWithAverageAsync_GetRequestException_ReturnsException(string userId, int pageSize, int pageNumber)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new PagedDocumentCollectionDto<RatingsDocument>()
      {
        Results = { },
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableByPageAsync<RatingsDocument>(pageNumber, It.IsAny<Microsoft.Azure.Cosmos.QueryRequestOptions>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>()))
                                    .ReturnsAsync(documentCollection);

      // Act
      var actionResult = await _userRatingsMediator.GetPagedUserRatingsWithAverageAsync(userIdGuid, pageNumber, cancellationToken, pageSize, null, null);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", 1, 20, DisplayName = "Any_UserId, Any_PageNumber, Any_PageSize")]
    public async Task GetPagedUserRatingsWithAverageAsync_GetRequestSuccess_ReturnsSuccess(string userId, int pageSize, int pageNumber)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      Guid userIdGuid = new Guid(userId);
      var documentCollection = new PagedDocumentCollectionDto<RatingsDocument>()
      {
        Results = { _fakeUserRatingsDocuments.First() },
        IsOperationSuccessful = true
      };

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableByPageAsync<RatingsDocument>(pageNumber, It.IsAny<Microsoft.Azure.Cosmos.QueryRequestOptions>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>()))
                                    .ReturnsAsync(documentCollection);

      // Act
      var actionResult = await _userRatingsMediator.GetPagedUserRatingsWithAverageAsync(userIdGuid, pageNumber, cancellationToken, pageSize, null, null);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
      //Assert.IsNotNull(actionResult.Ratings);
    }
    #endregion

    #region CreateUserRatingsAsync
    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", DisplayName = "Any_UserId")]

    public async Task CreateUserRatingsAsync_DatabaseRequestException_ReturnsException(string userId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollectionGet = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { },
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };
      var documentCollectionCreate = new CreateDocumentDto<RatingsDocument>()
      {
        Document = { },
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };

      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userRatingsCreateOrUpdateRequestDto = _fakeUserRatingsCreateOrUpdateRequestDto.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                    .ReturnsAsync((documentCollectionGet, continuationToken));

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.CreateItemAsync<RatingsDocument>(It.IsAny<RatingsDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(documentCollectionCreate);

      // Act
      var actionResult = await _userRatingsMediator.CreateUserRatingAsync(userIdGuid, userRatingsCreateOrUpdateRequestDto, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", DisplayName = "Any_UserId")]
    public async Task CreateUserRatingsAsync_CreateRequestSuccess_ReturnsSuccess(string userId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      var documentCollectionGet = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { _fakeUserRatingsDocuments.First() },
        IsOperationSuccessful = true
      };

      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userRatingsCreateOrUpdateRequestDto = _fakeUserRatingsCreateOrUpdateRequestDto.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                    .ReturnsAsync((documentCollectionGet, continuationToken));

      // UpdateItemAsync is used here because it updates the Rating list of an existing item in the Database
      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync<RatingsDocument>(It.IsAny<RatingsDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(operationStatus);

      // Act
      var actionResult = await _userRatingsMediator.CreateUserRatingAsync(userIdGuid, userRatingsCreateOrUpdateRequestDto, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }
    #endregion

    #region UpdateUserRatingAsync
    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", "f89ca93e-1814-4b73-a18b-e386415f4f82", DisplayName = "Any_UserId, Any_RatingId")]
    public async Task UpdateUserRatingAsync_UserIdNotFound_ReturnsFailure(string userId, string ratingId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      Guid ratingIdGuid = new Guid(ratingId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { },
        IsOperationSuccessful = true
      };

      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userRatingsCreateOrUpdateRequestDto = _fakeUserRatingsCreateOrUpdateRequestDto.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<RatingsDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userRatingsMediator.UpdateUserRatingAsync(userIdGuid, userRatingsCreateOrUpdateRequestDto, cancellationToken, null, ratingIdGuid);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", "f89ca93e-1814-4b73-a18b-e386415f4f82", DisplayName = "Any_UserId, Any_RatingId")]
    public async Task UpdateUserRatingAsync_GetRequestException_ReturnsException(string userId, string ratingId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      Guid ratingIdGuid = new Guid(ratingId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { },
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };

      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userRatingsCreateOrUpdateRequestDto = _fakeUserRatingsCreateOrUpdateRequestDto.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<RatingsDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userRatingsMediator.UpdateUserRatingAsync(userIdGuid, userRatingsCreateOrUpdateRequestDto, cancellationToken, null, ratingIdGuid);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", "f89ca93e-1814-4b73-a18b-e386415f4f82", DisplayName = "Any_UserId, Any_RatingId")]
    public async Task UpdateUserRatingAsync_UpdateRequestFailure_ReturnsFailure(string userId, string ratingId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      Guid ratingIdGuid = new Guid(ratingId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { _fakeUserRatingsDocuments.First() },
        IsOperationSuccessful = true
      };
      var operationStatus = new OperationStatus() { IsOperationSuccessful = false };
      var userRatingsCreateOrUpdateRequestDto = _fakeUserRatingsCreateOrUpdateRequestDto.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<RatingsDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userRatingsMediator.UpdateUserRatingAsync(userIdGuid, userRatingsCreateOrUpdateRequestDto, cancellationToken, null, ratingIdGuid);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", "f89ca93e-1814-4b73-a18b-e386415f4f82", DisplayName = "Any_UserId, Any_RatingId")]
    public async Task UpdateUserRatingAsync_UpdateRequestException_ReturnsException(string userId, string ratingId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      Guid ratingIdGuid = new Guid(ratingId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { _fakeUserRatingsDocuments.First() },
        IsOperationSuccessful = true
      };
      var operationStatus = new OperationStatus()
      {
        IsOperationSuccessful = false,
        OperationException = new Exception()
      };
      var userRatingsCreateOrUpdateRequestDto = _fakeUserRatingsCreateOrUpdateRequestDto.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<RatingsDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userRatingsMediator.UpdateUserRatingAsync(userIdGuid, userRatingsCreateOrUpdateRequestDto, cancellationToken, null, ratingIdGuid);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
      Assert.IsNotNull(actionResult.OperationException);
    }

    [TestMethod]
    [DataRow("0547d952-af8a-4444-abf8-2b68b8711106", "f89ca93e-1814-4b73-a18b-e386415f4f82", DisplayName = "Any_UserId, Any_RatingId")]
    public async Task UpdateUserRatingAsync_GetAndUpdateRequestsSuccess_ReturnsSuccess(string userId, string ratingId)
    {
      //Arrange
      var cancellationToken = CancellationToken.None;
      string continuationToken = null;
      Guid userIdGuid = new Guid(userId);
      Guid ratingIdGuid = new Guid(ratingId);
      var documentCollection = new DocumentCollectionDto<RatingsDocument>()
      {
        Results = { _fakeUserRatingsDocuments.First() },
        IsOperationSuccessful = true
      };
      var operationStatus = new OperationStatus() { IsOperationSuccessful = true };
      var userRatingsCreateOrUpdateRequestDto = _fakeUserRatingsCreateOrUpdateRequestDto.First();

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync<RatingsDocument>(continuationToken, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<RatingsDocument, bool>>>(), null))
                                  .ReturnsAsync((documentCollection, continuationToken));

      _databaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpsertItemAsync(It.IsAny<RatingsDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(operationStatus);

      //Act
      var actionResult = await _userRatingsMediator.UpdateUserRatingAsync(userIdGuid, userRatingsCreateOrUpdateRequestDto, cancellationToken, null, ratingIdGuid);

      //Assert
      Assert.IsNotNull(actionResult);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
      Assert.IsNull(actionResult.OperationException);
      Assert.AreEqual(userRatingsCreateOrUpdateRequestDto.Score, actionResult.Rating.Score);
      Assert.AreEqual(userRatingsCreateOrUpdateRequestDto.Comment, actionResult.Rating.Comment);
      Assert.AreEqual(userRatingsCreateOrUpdateRequestDto.ReviewedByUser.Id, actionResult.Rating.ReviewedByUser.Id);
      Assert.AreEqual(userRatingsCreateOrUpdateRequestDto.ReviewedByUser.FirstName, actionResult.Rating.ReviewedByUser.FirstName);
      Assert.AreEqual(userRatingsCreateOrUpdateRequestDto.ReviewedByUser.LastName, actionResult.Rating.ReviewedByUser.LastName);
      Assert.AreEqual(userRatingsCreateOrUpdateRequestDto.ReviewedUser.Id, actionResult.Rating.ReviewedUser.Id);
      Assert.AreEqual(userRatingsCreateOrUpdateRequestDto.ReviewedUser.FirstName, actionResult.Rating.ReviewedUser.FirstName);
      Assert.AreEqual(userRatingsCreateOrUpdateRequestDto.ReviewedUser.LastName, actionResult.Rating.ReviewedUser.LastName);
      Assert.AreEqual(userRatingsCreateOrUpdateRequestDto.Type, actionResult.Rating.Type);
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
      _fakeUserRatingsCreateOrUpdateRequestDto = null;
    }
    #endregion
  }
}
