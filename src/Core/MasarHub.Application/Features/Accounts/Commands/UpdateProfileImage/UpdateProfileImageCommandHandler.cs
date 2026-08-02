using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Commands.UpdateProfileImage
{
    public sealed class UpdateProfileImageCommandHandler : IRequestHandler<UpdateProfileImageCommand, Result<string>>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public UpdateProfileImageCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _usersRepository = usersRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<string>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
        {
            var uploadResult = await _fileStorageService.UploadAsync(request.File, FileType.Image, StorageFolders.Users.Avatars, cancellationToken);
            if (uploadResult.IsFailure)
                return uploadResult.Errors[0];

            var updateResult = await _usersRepository.UpdateProfileImageAsync(request.UserId, uploadResult.Value.FileKey, cancellationToken);
            if (updateResult.IsFailure)
            {
                await _fileStorageService.DeleteAsync(uploadResult.Value.FileKey, FileType.Image, cancellationToken);
                return updateResult.Errors[0];
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return uploadResult.Value.Url;
        }
    }
}
