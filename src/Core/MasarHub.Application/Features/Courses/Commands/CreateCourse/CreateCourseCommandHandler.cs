using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Common.Utilities;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourse
{
    public sealed class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Result<CreateCourseResponse>>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ICourseQuery _courseQuery;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public CreateCourseCommandHandler(IRepository<Course> courseRepository, ICourseQuery courseQuery, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _courseRepository = courseRepository;
            _courseQuery = courseQuery;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<CreateCourseResponse>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var baseSlug = SlugGenerator.GenerateSlug(request.Title);

            var creationData = await _courseQuery.GetCreationDataAsync(baseSlug, request.CategoryId, cancellationToken);
            if (!creationData.CategoryExists)
                return Error.NotFound("category.not_found");

            var slug = creationData.SlugCount == 0 ? baseSlug : $"{baseSlug}-{creationData.SlugCount + 1}";
            var courseResult = Course.Create(
                request.Title,
                slug,
                request.Description,
                request.Price,
                request.Language,
                request.Level,
                _currentUserService.UserId,
                request.CategoryId
            );

            if (courseResult.IsFailure)
                return courseResult.Error;

            var course = courseResult.Value;
            await _courseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateCourseResponse(
                course.Id,
                course.Title,
                course.Slug,
                course.Price,
                course.Status,
                course.InstructorId,
                course.CategoryId
            );
        }
    }
}