using MasarHub.Application.Common.Models;

namespace MasarHub.API.Extensions.Mappers
{
    public static class FormFileMappers
    {
        public static FileResource ToResource(this IFormFile formFile)
        {
            return new FileResource(
                FileName: formFile.FileName,
                ContentType: formFile.ContentType,
                Content: formFile.OpenReadStream(),
                FileSizeInByte: formFile.Length
            );
        }
    }
}
