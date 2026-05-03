using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Categories.Events;

namespace MasarHub.Domain.Modules.Categories
{
    public sealed class Category : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public int Level { get; private set; }
        public int DisplayOrder { get; private set; }

        public Guid? ParentCategoryId { get; private set; }

        private Category() { }

        private Category(string name, string slug, int level, int displayOrder, Guid? parentCategoryId)
        {
            Name = name;
            Slug = slug;
            Level = level;
            DisplayOrder = displayOrder;
            ParentCategoryId = parentCategoryId;

            RaiseDomainEvent(new CategoryCreatedEvent(Id));
        }
        public static Result<Category> CreateRoot(string name, string slug, int displayOrder)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNullOrWhiteSpace(name, nameof(name)),
                Guard.AgainstNullOrWhiteSpace(slug, nameof(slug)),
                Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder))
            );
            if (error is not null)
                return error;

            return new Category(name, slug, 1, displayOrder, null);
        }

        public static Result<Category> CreateSubCategory(string name, string slug, int displayOrder, Category parent)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNull(parent, nameof(parent)),
                Guard.AgainstNullOrWhiteSpace(name, nameof(name)),
                Guard.AgainstNullOrWhiteSpace(slug, nameof(slug)),
                Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder))
            );
            if (error is not null)
                return error;

            if (parent.Level >= 3)
                return CategoryErrors.MaxDepth;


            return new Category(name, slug, parent.Level + 1, displayOrder, parent.Id);
        }

        public Result Rename(string name)
        {
            var error = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            if (error is not null)
                return error;

            Name = name;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result ChangeDisplayOrder(int displayOrder)
        {
            var error = Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder));
            if (error is not null)
                return error;

            DisplayOrder = displayOrder;
            MarkAsUpdated();
            return Result.Success();
        }

    }
}

