using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCoursePrerequisites
{
    public sealed class UpdateCoursePrerequisitesCommandHandler : IRequestHandler<UpdateCoursePrerequisitesCommand, Result>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public UpdateCoursePrerequisitesCommandHandler(IRepository<Course> courseRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateCoursePrerequisitesCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course is null)
                return Error.NotFound("course.not_found");

            if (course.InstructorId != _currentUserService.UserId)
                return Error.Forbidden("course.access_denied");

            var result = course.SetPrerequisites(request.Prerequisites);
            if (result.IsFailure)
                return result.Error;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}