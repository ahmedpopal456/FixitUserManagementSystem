using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Fixit.Core.Database.Mediators;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Ratings;
using Fixit.User.Management.Lib.Models;
using Fixit.Core.DataContracts.Users.Operations.Ratings;

namespace Fixit.User.Management.Lib.Mediators.Internal
{
  public class UserRatingsMediator : IUserRatingsMediator
  {
    private readonly IMapper _mapper;
    private readonly IDatabaseTableEntityMediator _userRatingTable;
    private const int PageSize = 20;

    public UserRatingsMediator(IMapper mapper,
                              IDatabaseMediator databaseMediator,
                              IConfiguration configurationProvider)
    {
      var databaseName = configurationProvider["FIXIT-UM-DB-NAME"];
      var databaseRatingTableName = configurationProvider["FIXIT-UM-DB-RATINGSTABLE"];

      if (string.IsNullOrWhiteSpace(databaseName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects the {nameof(configurationProvider)} to have defined the Fix Management Database as {{FIXIT-UM-USERDB}} ");
      }

      if (string.IsNullOrWhiteSpace(databaseRatingTableName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects the {nameof(configurationProvider)} to have defined the Fix Management Table as {{FIXIT-UM-RATINGSTABLE}} ");
      }

      if (databaseMediator == null)
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(databaseMediator)}... null argument was provided");
      }

      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _userRatingTable = databaseMediator.GetDatabase(databaseName).GetContainer(databaseRatingTableName);
    }

    public UserRatingsMediator(IMapper mapper,
                              IDatabaseMediator databaseMediator,
                              string databaseName,
                              string tableName)
    {
      if (string.IsNullOrWhiteSpace(databaseName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(databaseName)}... null argument was provided");
      }

      if (string.IsNullOrWhiteSpace(tableName))
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(tableName)}... null argument was provided");
      }

      if (databaseMediator == null)
      {
        throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(databaseMediator)}... null argument was provided");
      }

      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UserMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _userRatingTable = databaseMediator.GetDatabase(databaseName).GetContainer(tableName);
    }

    #region UserRatingConfiguration
    public async Task<RatingResponseDto> GetUserRatingAsync(Guid userId, Guid ratingId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var result = default(RatingResponseDto);

      var (ratingDocumentCollection, token) = await _userRatingTable.GetItemQueryableAsync<RatingsDocument>(null, cancellationToken, ratingDocument => ratingDocument.EntityId == userId.ToString());

      if (ratingDocumentCollection != null)
      {
        result = new RatingResponseDto()
        {
          OperationException = ratingDocumentCollection.OperationException,
          OperationMessage = ratingDocumentCollection.OperationMessage
        };

        if (ratingDocumentCollection.IsOperationSuccessful)
        {
          RatingsDocument ratingDocument = ratingDocumentCollection.Results.SingleOrDefault();
          if (ratingDocument != null)
          {
            result.Rating = ratingDocument.Ratings.FirstOrDefault(rating => rating.Id == ratingId);
            result.IsOperationSuccessful = true;
          }
        }
      }

      return result;
    }

    public async Task<RatingsResponseDto> GetUserRatingsAverageAsync(Guid userId, CancellationToken cancellationToken, long? minTimestampUtc = null, long? maxTimestampUtc = null)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var result = default(RatingsResponseDto);

      var (ratingDocumentCollection, token) = await _userRatingTable.GetItemQueryableAsync<RatingsDocument>(null, cancellationToken,
                                                          ratingDocument => ratingDocument.EntityId == userId.ToString());

      if (ratingDocumentCollection != null)
      {
        result = new RatingsResponseDto()
        {
          OperationException = ratingDocumentCollection.OperationException,
          OperationMessage = ratingDocumentCollection.OperationMessage
        };

        if (ratingDocumentCollection.IsOperationSuccessful)
        {
          RatingsDocument ratingDocument = ratingDocumentCollection.Results.SingleOrDefault();
          if (maxTimestampUtc != null || minTimestampUtc != null)
          {
            ratingDocument.Ratings = ratingDocument.Ratings.Where(rating => rating.CreatedTimestampUtc <= maxTimestampUtc || rating.CreatedTimestampUtc >= minTimestampUtc);
          }
          if (ratingDocument != null)
          {
            result.Ratings = _mapper.Map<RatingsDocument, RatingsDto>(ratingDocument);
            result.IsOperationSuccessful = true;
          }

        }
      }

      return result;
    }

    public async Task<RatingsResponseDto> GetPagedUserRatingsAverageAsync(Guid userId, int pageNumber, CancellationToken cancellationToken, int? pageSize = null, long? minTimestampUtc = null, long? maxTimestampUtc = null)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var result = default(RatingsResponseDto);
      var ratingDocumentCollection = default(PagedDocumentCollectionDto<RatingsDocument>);
      pageSize = (pageSize == null || pageSize == 0) ? PageSize : pageSize;

      var queryRequestOptions = new QueryRequestOptions()
      {
        MaxItemCount = pageSize
      };

      ratingDocumentCollection = await _userRatingTable.GetItemQueryableByPageAsync<RatingsDocument>(pageNumber, queryRequestOptions, cancellationToken, ratingDocument => ratingDocument.EntityId == userId.ToString());

      if (ratingDocumentCollection != null)
      {
        result = new RatingsResponseDto()
        {
          OperationException = ratingDocumentCollection.OperationException,
          OperationMessage = ratingDocumentCollection.OperationMessage
        };

        if (ratingDocumentCollection.IsOperationSuccessful)
        {
          RatingsDocument ratingDocument = ratingDocumentCollection.Results.SingleOrDefault();
          if (maxTimestampUtc != null || minTimestampUtc != null)
          {
            ratingDocument.Ratings = ratingDocument.Ratings.Where(rating => rating.CreatedTimestampUtc <= maxTimestampUtc || rating.CreatedTimestampUtc >= minTimestampUtc);
          }
          if (ratingDocument != null)
          {
            result.Ratings = _mapper.Map<RatingsDocument, RatingsDto>(ratingDocument);
            result.IsOperationSuccessful = true;
          }

        }
      }

      return result;
    }

    public async Task<RatingResponseDto> CreateUserRatingAsync(Guid userId, UserRatingsCreateOrUpdateRequestDto ratingCreateRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var result = default(RatingResponseDto);
      var (ratingDocumentCollection, token) = await _userRatingTable.GetItemQueryableAsync<RatingsDocument>(null, cancellationToken, ratingDocument => ratingDocument.EntityId == userId.ToString());

      if (ratingDocumentCollection != null)
      {
        var currentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        var ratingDto = _mapper.Map<UserRatingsCreateOrUpdateRequestDto, RatingDto>(ratingCreateRequestDto);
        ratingDto.Id = Guid.NewGuid();
        ratingDto.CreatedTimestampUtc = currentTimestamp;
        ratingDto.UpdatedTimestampUtc = currentTimestamp;
        ratingDto.Type = RatingType.User;

        if (ratingDocumentCollection.Results.Count == 0)
        {
          RatingsDocument ratingDocument = new RatingsDocument
          {
            Ratings = new List<RatingDto>() { ratingDto },
            AverageRating = ratingDto.Score,
            RatingsOfUser = ratingDto.ReviewedUser,
            CreatedTimestampUtc = currentTimestamp,
            UpdatedTimestampUtc = currentTimestamp
          };

          var createdResponse = await _userRatingTable.CreateItemAsync(ratingDocument, userId.ToString(), cancellationToken);
          if (createdResponse != null)
          {
            result = new RatingResponseDto()
            {
              OperationException = createdResponse.OperationException,
              OperationMessage = createdResponse.OperationMessage
            };

            if (createdResponse.IsOperationSuccessful && createdResponse.Document != null)
            {
              result.Rating = createdResponse.Document.Ratings.FirstOrDefault();
              result.IsOperationSuccessful = true;
            }
          }
        }

        else if (ratingDocumentCollection.Results.Count > 0)
        {
          result = await UpdateUserRatingAsync(userId, ratingCreateRequestDto, cancellationToken, ratingDto, null);
        }
      }

      return result;
    }

    public async Task<RatingResponseDto> UpdateUserRatingAsync(Guid userId, UserRatingsCreateOrUpdateRequestDto ratingUpdateRequestDto, CancellationToken cancellationToken, RatingDto ratingDto, Guid? ratingId = null)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var result = default(RatingResponseDto);

      var (ratingDocumentCollection, token) = await _userRatingTable.GetItemQueryableAsync<RatingsDocument>(null, cancellationToken, ratingDocument => ratingDocument.EntityId == userId.ToString());

      result = new RatingResponseDto()
      {
        OperationException = ratingDocumentCollection.OperationException,
        OperationMessage = ratingDocumentCollection.OperationMessage
      };

      if (ratingDocumentCollection.IsOperationSuccessful)
      {
        RatingsDocument ratingDocument = ratingDocumentCollection.Results.SingleOrDefault();
        if (ratingDocument != null)
        {
          // Add rating item into Ratings List
          if (ratingDto != null)
          {
            ratingDocument.Ratings = ratingDocument.Ratings.Append(ratingDto);
          }
          // Update score and comment of existing item in Database
          else
          {
            var rating = ratingDocument.Ratings.First(rating => rating.Id == ratingId);
            rating.Score = ratingUpdateRequestDto.Score;
            rating.Comment = ratingUpdateRequestDto.Comment;
            rating.UpdatedTimestampUtc = DateTimeOffset.Now.ToUnixTimeSeconds();
          }

          // Calculate average score
          ratingDocument.AverageRating = ratingDocument.Ratings.Average(rating => rating.Score);

          ratingDocument.UpdatedTimestampUtc = DateTimeOffset.Now.ToUnixTimeSeconds();

          var operationStatus = await _userRatingTable.UpdateItemAsync(ratingDocument, ratingDocument.EntityId, cancellationToken);

          result.OperationException = operationStatus.OperationException;
          result.OperationMessage = operationStatus.OperationMessage;

          if (operationStatus.IsOperationSuccessful)
          {
            result.Rating = ratingDocument.Ratings.First(rating => rating.Id == (ratingId != null ? ratingId : ratingDto.Id));
            result.IsOperationSuccessful = true;
          }
        }
      }

      return result;
    }
    #endregion
  }
}
