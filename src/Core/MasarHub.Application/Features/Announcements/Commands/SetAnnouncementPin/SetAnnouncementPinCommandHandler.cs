using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Commands.SetAnnouncementPin
{
    public sealed class SetAnnouncementPinCommandHandler : IRequestHandler<SetAnnouncementPinCommand, Result>
    {
        private readonly ICourseQuery _courseQuery;
        private readonly IRepository<CourseAnnouncement> _announcementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetAnnouncementPinCommandHandler(ICourseQuery courseQuery, IRepository<CourseAnnouncement> announcementRepository, IUnitOfWork unitOfWork)
        {
            _courseQuery = courseQuery;
            _announcementRepository = announcementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SetAnnouncementPinCommand request, CancellationToken cancellationToken)
        {
            var accessData = await _courseQuery.GetCourseAccessData(request.CourseId, request.InstructorId, cancellationToken);
            if (!accessData.CourseExist)
                return Error.NotFound("course.not_found");
            if (!accessData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var announcement = await _announcementRepository.GetByIdAsync(request.AnnouncementId, cancellationToken);
            if (announcement == null || announcement.CourseId != request.CourseId)
                return Error.NotFound("course_announcement.not_found");

            var result = request.IsPinned ? announcement.Pin() : announcement.Unpin();
            if (result.IsFailure)
                return result.Error;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
