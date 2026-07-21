using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.DeleteCourseReview
{
    public sealed class DeleteCourseReviewCommandHandler : IRequestHandler<DeleteCourseReviewCommand, Result>
    {
        private readonly IRepository<CourseReview> _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCourseReviewCommandHandler(IRepository<CourseReview> reviewRepository, IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCourseReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);

            if (review == null || review.CourseId != request.CourseId)
                return Error.NotFound("course_review.not_found");

            if (review.UserId != request.UserId)
                return Error.Forbidden("course.access_denied");

            review.Delete();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
