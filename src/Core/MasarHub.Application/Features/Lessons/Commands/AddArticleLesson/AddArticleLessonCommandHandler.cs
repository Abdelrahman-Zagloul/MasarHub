using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Lessons.Commands.AddArticleLesson;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson
{
    public sealed class AddArticleLessonCommandHandler : IRequestHandler<AddArticleLessonCommand, Result<AddArticleLessonResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly ILessonQuery _lessonQuery;

        public AddArticleLessonCommandHandler(IUnitOfWork unitOfWork, IRepository<Lesson> lessonRepository, ILessonQuery lessonQuery)
        {
            _unitOfWork = unitOfWork;
            _lessonRepository = lessonRepository;
            _lessonQuery = lessonQuery;
        }

        public async Task<Result<AddArticleLessonResponse>> Handle(AddArticleLessonCommand request, CancellationToken cancellationToken)
        {
            var creationData = await _lessonQuery.GetCreationDataAsync(request.CourseId, request.ModuleId, request.InstructorId);

            if (!creationData.IsOwner)
                return Error.Forbidden("course.access_denied");

            if (!creationData.ModuleExist)
                return Error.NotFound("module.not_found");

            var lessonResult = ArticleLesson.Create(request.ModuleId, request.IsPreviewable, request.Title, creationData.NextDisplayOrder, request.Description, request.Content);
            if (lessonResult.IsFailure)
                return lessonResult.Error;

            await _lessonRepository.AddAsync(lessonResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AddArticleLessonResponse(
                lessonResult.Value.Id,
                request.ModuleId,
                request.IsPreviewable,
                creationData.NextDisplayOrder,
                request.Title,
                request.Content,
                request.Description);
        }
    }
}
