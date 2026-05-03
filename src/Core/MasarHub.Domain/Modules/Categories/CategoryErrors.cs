using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Categories
{
    public static class CategoryErrors
    {
        public static readonly DomainError MaxDepth = new DomainError("Category.MaxDepth", "Level");
    }
}
