using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Attachments.Commands.DeleteAttachment
{
    public sealed record DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<LessonAttachment> _lessonAttachmentRepo;
        private readonly ILessonQuery _lessonQuery;
        public DeleteAttachmentCommandHandler(IUnitOfWork unitOfWork, IRepository<LessonAttachment> lessonAttachmentRepo, ILessonQuery lessonQuery)
        {
            _unitOfWork = unitOfWork;
            _lessonAttachmentRepo = lessonAttachmentRepo;
            _lessonQuery = lessonQuery;
        }

        public async Task<Result> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
        {
            var attachment = await _lessonAttachmentRepo.GetByIdAsync(request.AttachmentId);

            if (attachment == null || attachment.LessonId != request.LessonId)
                return Error.NotFound("attachment.not_found");

            var isOwner = await _lessonQuery.IsLessonOwnedByInstructorAsync(request.LessonId, request.InstructorId);
            if (!isOwner)
                return Error.Forbidden("course.access_denied");

            attachment.Delete();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
