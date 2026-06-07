using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.SubmitCourseForApproval
{
    public sealed class SubmitCourseForApprovalCommandHandler : IRequestHandler<SubmitCourseForApprovalCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Course> _courseRepository;
        private readonly ICourseQuery _courseQuery;

        public SubmitCourseForApprovalCommandHandler(IUnitOfWork unitOfWork, IRepository<Course> courseRepository, ICourseQuery courseQuery)
        {
            _unitOfWork = unitOfWork;
            _courseRepository = courseRepository;
            _courseQuery = courseQuery;
        }

        public async Task<Result> Handle(SubmitCourseForApprovalCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return Error.NotFound("course.not_found");

            if (course.InstructorId != request.InstructorId)
                return Error.Forbidden("course.access_denied");

            var hasContent = await _courseQuery.HasLecturesAsync(request.CourseId, cancellationToken);
            if (!hasContent)
                return Error.BadRequest("course.cannot_submit_empty_course");

            var result = course.SubmitForApproval();
            if (result.IsFailure)
                return result.Error;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}