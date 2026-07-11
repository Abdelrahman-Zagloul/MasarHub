using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourseAnnouncement
{
    public sealed class CreateCourseAnnouncementCommandHandler
        : IRequestHandler<CreateCourseAnnouncementCommand, Result<CreateCourseAnnouncementResponse>>
    {
        private readonly ICourseQuery _courseQuery;
        private readonly IRepository<CourseAnnouncement> _announcementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseAnnouncementCommandHandler(ICourseQuery courseQuery, IRepository<CourseAnnouncement> announcementRepository, IUnitOfWork unitOfWork)
        {
            _courseQuery = courseQuery;
            _announcementRepository = announcementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateCourseAnnouncementResponse>> Handle(CreateCourseAnnouncementCommand request, CancellationToken cancellationToken)
        {
            var accessData = await _courseQuery.GetCourseAccessData(request.CourseId, request.InstructorId, cancellationToken);
            if (!accessData.CourseExist)
                return Error.NotFound("course.not_found");
            if (!accessData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var announcementResult = CourseAnnouncement.Create(request.CourseId, request.InstructorId, request.Title, request.Content, request.Importance);
            if (announcementResult.IsFailure)
                return announcementResult.Error;

            var announcement = announcementResult.Value;

            await _announcementRepository.AddAsync(announcement, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateCourseAnnouncementResponse(
                announcement.Id,
                announcement.CourseId,
                announcement.Title,
                announcement.Content,
                announcement.Importance,
                announcement.CreatedAt);
        }
    }
}
