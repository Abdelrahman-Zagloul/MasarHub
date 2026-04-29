using MasarHub.Domain.SharedKernel;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class ArticleLesson : Lesson
    {
        public string Content { get; private set; } = null!;
        private ArticleLesson() { }

        private ArticleLesson(Guid moduleId, string title, int order, string? description, string content)
            : base(moduleId, title, order, description)
        {
            Content = Guard.AgainstNullOrWhiteSpace(content, nameof(content));
        }

        public static ArticleLesson Create(Guid moduleId, string title, int order, string? description, string content)
        {
            return new ArticleLesson(moduleId, title, order, description, content);
        }

        public void UpdateContent(string content)
        {
            Content = Guard.AgainstNullOrWhiteSpace(content, nameof(content));
            MarkAsUpdated();
        }
    }
}
