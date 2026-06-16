using MasarHub.Domain.Modules.Categories;

namespace MasarHub.Domain.UnitTests.Modules.Categories
{
    public sealed class CategoryTests
    {
        private const string Name = "Programming";
        private const string Slug = "programming";
        private const int DisplayOrder = 1;

        #region CreateRoot

        [Fact]
        public void CreateRoot_ValidInput_ReturnsSuccess()
        {
            var result = Category.CreateRoot(Name, null, Slug, DisplayOrder);

            Assert.True(result.IsSuccess);
            Assert.Equal(Name, result.Value.Name);
            Assert.Equal(Slug, result.Value.Slug);
            Assert.Equal(1, result.Value.Level);
            Assert.Equal(DisplayOrder, result.Value.DisplayOrder);
            Assert.Null(result.Value.ParentCategoryId);
            Assert.Null(result.Value.Description);
        }

        [Fact]
        public void CreateRoot_WithDescription_ReturnsSuccess()
        {
            var description = "All programming-related categories";

            var result = Category.CreateRoot(Name, description, Slug, DisplayOrder);

            Assert.True(result.IsSuccess);
            Assert.Equal(description, result.Value.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreateRoot_InvalidName_ReturnsError(string? name)
        {
            var result = Category.CreateRoot(name!, null, Slug, DisplayOrder);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreateRoot_InvalidSlug_ReturnsError(string? slug)
        {
            var result = Category.CreateRoot(Name, null, slug!, DisplayOrder);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void CreateRoot_InvalidDisplayOrder_ReturnsError(int order)
        {
            var result = Category.CreateRoot(Name, null, Slug, order);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region CreateSubCategory

        [Fact]
        public void CreateSubCategory_ValidParent_ReturnsSuccess()
        {
            var parent = CreateRootCategory();
            var subName = "C#";

            var result = Category.CreateSubCategory(subName, null, "csharp", 1, parent);

            Assert.True(result.IsSuccess);
            Assert.Equal(subName, result.Value.Name);
            Assert.Equal(2, result.Value.Level);
            Assert.Equal(parent.Id, result.Value.ParentCategoryId);
        }

        [Fact]
        public void CreateSubCategory_NullParent_ReturnsError()
        {
            var result = Category.CreateSubCategory(Name, null, Slug, DisplayOrder, null!);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void CreateSubCategory_InvalidName_ReturnsError()
        {
            var parent = CreateRootCategory();

            var result = Category.CreateSubCategory("", null, Slug, DisplayOrder, parent);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void CreateSubCategory_MaxDepthExceeded_ReturnsError()
        {
            var level1 = CreateRootCategory();
            var level2 = Category.CreateSubCategory("Sub1", null, "sub1", 1, level1).Value;
            var level3 = Category.CreateSubCategory("Sub2", null, "sub2", 1, level2).Value;

            var result = Category.CreateSubCategory("Sub3", null, "sub3", 1, level3);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region Rename

        [Fact]
        public void Rename_ValidInput_ReturnsSuccess()
        {
            var category = CreateRootCategory();
            var newName = "Advanced Programming";

            var result = category.Rename(newName);

            Assert.True(result.IsSuccess);
            Assert.Equal(newName, category.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Rename_InvalidInput_ReturnsError(string? name)
        {
            var category = CreateRootCategory();

            var result = category.Rename(name!);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region ChangeParentCategory

        [Fact]
        public void ChangeParentCategory_ValidParent_ReturnsSuccess()
        {
            var category = CreateRootCategory();
            var newParent = Category.CreateRoot("Design", null, "design", 2).Value;

            var result = category.ChangeParentCategory(newParent);

            Assert.True(result.IsSuccess);
            Assert.Equal(newParent.Id, category.ParentCategoryId);
            Assert.Equal(2, category.Level);
        }

        [Fact]
        public void ChangeParentCategory_NullParent_ReturnsError()
        {
            var category = CreateRootCategory();

            var result = category.ChangeParentCategory(null!);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void ChangeParentCategory_SelfAsParent_ReturnsError()
        {
            var category = CreateRootCategory();

            var result = category.ChangeParentCategory(category);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void ChangeParentCategory_ParentAtMaxDepth_ReturnsError()
        {
            var root = CreateRootCategory();
            var sub1 = Category.CreateSubCategory("Sub1", null, "sub1", 1, root).Value;
            var sub2 = Category.CreateSubCategory("Sub2", null, "sub2", 1, sub1).Value;
            var target = Category.CreateRoot("Other", null, "other", 1).Value;

            var result = target.ChangeParentCategory(sub2);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region MoveToRoot

        [Fact]
        public void MoveToRoot_SubCategory_ReturnsSuccess()
        {
            var root = CreateRootCategory();
            var sub = Category.CreateSubCategory("Sub", null, "sub", 1, root).Value;

            var result = sub.MoveToRoot();

            Assert.True(result.IsSuccess);
            Assert.Null(sub.ParentCategoryId);
            Assert.Equal(1, sub.Level);
        }

        [Fact]
        public void MoveToRoot_AlreadyRoot_ReturnsError()
        {
            var category = CreateRootCategory();

            var result = category.MoveToRoot();

            Assert.True(result.IsFailure);
        }

        #endregion

        #region UpdateDescription

        [Fact]
        public void UpdateDescription_ValidInput_SetsDescription()
        {
            var category = CreateRootCategory();
            var newDescription = "Updated description";

            category.UpdateDescription(newDescription);

            Assert.Equal(newDescription, category.Description);
        }

        [Fact]
        public void UpdateDescription_Null_ClearsDescription()
        {
            var category = CreateRootCategory();
            category.UpdateDescription("Original");

            category.UpdateDescription(null);

            Assert.Null(category.Description);
        }

        #endregion

        private static Category CreateRootCategory()
        {
            return Category.CreateRoot(Name, null, Slug, DisplayOrder).Value;
        }
    }
}
