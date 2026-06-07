using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourse
{
    public sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Course> _courseRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICourseQuery _courseQuery;

        public UpdateCourseCommandHandler(IUnitOfWork unitOfWork, IRepository<Course> courseRepository, ICurrentUserService currentUserService, ICourseQuery courseQuery)
        {
            _unitOfWork = unitOfWork;
            _courseRepository = courseRepository;
            _currentUserService = currentUserService;
            _courseQuery = courseQuery;
        }

        public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return Error.NotFound("course.not_found");

            if (course.InstructorId != _currentUserService.UserId)
                return Error.Forbidden("course.access_denied");

            DomainResult updateResult = DomainResult.Success();
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                updateResult = course.UpdateTitle(request.Title);
                if (updateResult.IsFailure)
                    return updateResult.Error;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                updateResult = course.UpdateDescription(request.Description!);
                if (updateResult.IsFailure)
                    return updateResult.Error;
            }

            if (request.Price.HasValue)
            {
                updateResult = course.UpdatePrice(request.Price.Value);
                if (updateResult.IsFailure)
                    return updateResult.Error;
            }

            if (request.Level.HasValue)
            {
                updateResult = course.UpdateLevel(request.Level.Value);
                if (updateResult.IsFailure)
                    return updateResult.Error;
            }

            if (request.Language.HasValue)
            {
                updateResult = course.UpdateLanguage(request.Language.Value);
                if (updateResult.IsFailure)
                    return updateResult.Error;
            }

            if (request.CategoryId.HasValue)
            {
                var categoryExists = await _courseQuery.CategoryExistsAsync(request.CategoryId.Value, cancellationToken);
                if (!categoryExists)
                    return Error.NotFound("category.not_found");

                updateResult = course.UpdateCategory(request.CategoryId.Value);
                if (updateResult.IsFailure)
                    return updateResult.Error;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
