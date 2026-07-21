using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourseReview
{
    public sealed class UpdateCourseReviewCommandHandler : IRequestHandler<UpdateCourseReviewCommand, Result>
    {
        private readonly IRepository<CourseReview> _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCourseReviewCommandHandler(IRepository<CourseReview> reviewRepository, IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateCourseReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);

            if (review == null || review.CourseId != request.CourseId)
                return Error.NotFound("course_review.not_found");

            if (review.UserId != request.UserId)
                return Error.Forbidden("course.access_denied");

            if (request.Rating.HasValue)
            {
                var result = review.UpdateRating(request.Rating.Value);
                if (result.IsFailure)
                    return result.Error;
            }

            if (request.ReviewContent != null)
            {
                var result = review.UpdateContent(request.ReviewContent);
                if (result.IsFailure)
                    return result.Error;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
