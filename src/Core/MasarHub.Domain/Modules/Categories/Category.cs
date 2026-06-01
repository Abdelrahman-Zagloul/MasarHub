using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Categories
{
    public sealed class Category : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public string Slug { get; private set; } = null!;
        public int Level { get; private set; }
        public int DisplayOrder { get; private set; }

        public Guid? ParentCategoryId { get; private set; }

        private Category() { }

        private Category(string name, string? description, string slug, int level, int displayOrder, Guid? parentCategoryId)
        {
            Name = name;
            Description = description;
            Slug = slug;
            Level = level;
            DisplayOrder = displayOrder;
            ParentCategoryId = parentCategoryId;
        }
        public static DomainResult<Category> CreateRoot(string name, string? description, string slug, int displayOrder)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNullOrWhiteSpace(name, nameof(name)),
                Guard.AgainstNullOrWhiteSpace(name, nameof(name)),
                Guard.AgainstNullOrWhiteSpace(slug, nameof(slug)),
                Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder))
            );
            if (error is not null)
                return error;

            return new Category(name, description, slug, 1, displayOrder, null);
        }

        public static DomainResult<Category> CreateSubCategory(string name, string? description, string slug, int displayOrder, Category parent)
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
                return new DomainError("category.max_depth", "Level");


            return new Category(name, description, slug, parent.Level + 1, displayOrder, parent.Id);
        }

        public DomainResult Rename(string name)
        {
            var error = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            if (error is not null)
                return error;

            Name = name;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public void UpdateDescription(string? description)
        {
            Description = description;
            MarkAsUpdated();
        }
        public DomainResult ChangeParentCategory(Category parent)
        {
            var error = Guard.AgainstNull(parent, nameof(parent));
            if (error is not null)
                return error;

            if (parent.Id == Id)
                return new DomainError("category.cannot_be_own_parent", "ParentCategoryId");

            if (parent.Level >= 3)
                return new DomainError("category.max_depth", "Level");

            ParentCategoryId = parent.Id;
            Level = parent.Level + 1;

            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult MoveToRoot()
        {
            if (ParentCategoryId == null)
                return new DomainError("category.already_root", "ParentCategoryId");

            ParentCategoryId = null;
            Level = 1;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult ChangeDisplayOrder(int displayOrder)
        {
            var error = Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder));
            if (error is not null)
                return error;

            DisplayOrder = displayOrder;
            MarkAsUpdated();
            return DomainResult.Success();
        }

    }
}

