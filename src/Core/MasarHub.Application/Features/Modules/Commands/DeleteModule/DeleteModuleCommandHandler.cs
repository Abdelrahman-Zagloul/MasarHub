using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.DeleteModule
{
    public sealed class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseModuleQuery _courseModuleQuery;
        private readonly IRepository<CourseModule> _courseModuleRepository;

        public DeleteModuleCommandHandler(IUnitOfWork unitOfWork, ICourseModuleQuery courseModuleQuery, IRepository<CourseModule> courseModuleRepository)
        {
            _unitOfWork = unitOfWork;
            _courseModuleQuery = courseModuleQuery;
            _courseModuleRepository = courseModuleRepository;
        }

        public async Task<Result> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
        {
            var (moduleExists, isOwner, courseId) =
                await _courseModuleQuery.GetUpdateDataAsync(request.ModuleId, request.InstructorId, cancellationToken);

            if (!moduleExists)
                return Error.NotFound("module.not_found");

            if (!isOwner)
                return Error.Forbidden("course.access_denied");

            if (request.CourseId != courseId)
                return Error.BadRequest("module.course_mismatch");

            var courseModule = await _courseModuleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
            if (courseModule == null)
                return Error.NotFound("module.not_found");

            courseModule.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
