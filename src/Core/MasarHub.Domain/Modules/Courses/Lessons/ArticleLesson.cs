using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class ArticleLesson : Lesson
    {
        public string Content { get; private set; } = null!;

        private ArticleLesson() { }

        private ArticleLesson(Guid moduleId, string title, int order, string? description, string content)
            : base(moduleId, title, order, description)
        {
            Content = content;
        }

        public static Result<ArticleLesson> Create(Guid moduleId, string title, int order, string? description, string content)
        {
            var error = GuardExtensions.FirstError(
                ValidateLesson(moduleId, title, order),
                Guard.AgainstNullOrWhiteSpace(content, nameof(content))
            );

            if (error is not null)
                return error;

            return new ArticleLesson(moduleId, title, order, description, content);
        }

        public Result UpdateContent(string content)
        {
            var error = Guard.AgainstNullOrWhiteSpace(content, nameof(content));
            if (error is not null)
                return error;

            Content = content;
            MarkAsUpdated();
            return Result.Success();
        }
    }
}
