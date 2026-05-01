using MasarHub.Domain.Modules.Categories.Events;
using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

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
            Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Slug = Guard.AgainstNullOrWhiteSpace(slug, nameof(slug));
            Level = Guard.AgainstNegativeOrZero(level, nameof(level));
            DisplayOrder = Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder));

            ParentCategoryId = parentCategoryId;

            RaiseDomainEvent(new CategoryCreatedEvent(Id));
        }
        public static Category CreateRoot(string name, string slug, int displayOrder)
        {
            return new Category(name, slug, 1, displayOrder, null);
        }

        public static Category CreateSubCategory(string name, string slug, int displayOrder, Category parent)
        {
            parent = Guard.AgainstNull(parent, nameof(parent));

            if (parent.Level >= 3)
                throw new DomainException(ErrorCodes.Category.MaxDepth);

            return new Category(name, slug, parent.Level + 1, displayOrder, parent.Id);
        }

        public void Rename(string name)
        {
            Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));

            MarkAsUpdated();
        }

        public void ChangeDisplayOrder(int displayOrder)
        {
            DisplayOrder = Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder));

            MarkAsUpdated();
        }
    }
}