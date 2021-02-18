using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts.Users.Ratings;
using Fixit.Core.DataContracts.Users.Operations.Ratings;

namespace Fixit.User.Management.Lib.Mediators
{
  public interface IUserRatingsMediator
  {
    /// <summary>
    /// Get user rating
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ratingId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RatingResponseDto> GetUserRatingAsync(Guid userId, Guid ratingId, CancellationToken cancellationToken);

    /// <summary>
    /// Get user rating average
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="minTimestampUtc"></param>
    /// <param name="maxTimestampUtc"></param>
    /// <returns></returns>
    Task<RatingsResponseDto> GetUserRatingsAverageAsync(Guid userId, CancellationToken cancellationToken, long? minTimestampUtc = null, long? maxTimestampUtc = null);

    /// <summary>
    /// Get paged user rating average
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageSize"></param>
    /// <param name="minTimestampUtc"></param>
    /// <param name="maxTimestampUtc"></param>
    /// <returns></returns>
    Task<RatingsResponseDto> GetPagedUserRatingsAverageAsync(Guid userId, int pageNumber, CancellationToken cancellationToken, int? pageSize = null, long? minTimestampUtc = null, long? maxTimestampUtc = null);

    /// <summary>
    /// Create user rating
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ratingCreateRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RatingResponseDto> CreateUserRatingAsync(Guid userId, UserRatingsCreateOrUpdateRequestDto ratingCreateRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// Update user rating
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ratingUpdateRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="ratingDto"></param>
    /// <param name="ratingId"></param>
    /// <returns></returns>
    Task<RatingResponseDto> UpdateUserRatingAsync(Guid userId, UserRatingsCreateOrUpdateRequestDto ratingUpdateRequestDto, CancellationToken cancellationToken, RatingDto ratingDto, Guid? ratingId = null);
  }
}
