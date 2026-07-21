using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourseReview
{
    public sealed class CreateCourseReviewCommandHandler : IRequestHandler<CreateCourseReviewCommand, Result<CreateCourseReviewResponse>>
    {
        private readonly ICourseReviewQuery _courseReviewQuery;
        private readonly IRepository<CourseReview> _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseReviewCommandHandler(ICourseReviewQuery courseReviewQuery, IRepository<CourseReview> reviewRepository, IUnitOfWork unitOfWork)
        {
            _courseReviewQuery = courseReviewQuery;
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateCourseReviewResponse>> Handle(CreateCourseReviewCommand request, CancellationToken cancellationToken)
        {
            var checkResult = await _courseReviewQuery.GetCreateReviewCheckAsync(request.CourseId, request.UserId, cancellationToken);

            if (!checkResult.IsEnrolled)
                return Error.Forbidden("course.not_enrolled");

            if (checkResult.HasExistingReview)
                return Error.Conflict("course_review.already_reviewed");

            var reviewResult = CourseReview.Create(request.UserId, request.CourseId, request.Rating, request.ReviewContent);
            if (reviewResult.IsFailure)
                return reviewResult.Error;

            var review = reviewResult.Value;

            await _reviewRepository.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateCourseReviewResponse(
                review.Id, review.CourseId, review.UserId, review.Rating, review.ReviewContent, review.CreatedAt);
        }
    }
}
