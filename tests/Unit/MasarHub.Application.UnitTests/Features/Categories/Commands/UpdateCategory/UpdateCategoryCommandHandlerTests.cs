using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Categories.Commands.UpdateCategory;
using MasarHub.Domain.Modules.Categories;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Categories.Commands.UpdateCategory
{
    [Trait("UnitTests", "Feature.Categories.UpdateCategory.Handler")]
    public sealed class UpdateCategoryCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
        private readonly Mock<ICategoryQuery> _categoryQueryMock;
        private readonly UpdateCategoryCommandHandler _sut;

        public UpdateCategoryCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryRepositoryMock = new Mock<IRepository<Category>>();
            _categoryQueryMock = new Mock<ICategoryQuery>();
            _sut = new UpdateCategoryCommandHandler(_unitOfWorkMock.Object, _categoryRepositoryMock.Object, _categoryQueryMock.Object);
        }

        [Fact]
        public async Task Handle_CategoryNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "NewName", null, null, false);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RenameCategory_UpdatesName()
        {
            var category = Category.CreateRoot("OldName", null, "slug", 1).Value;
            var command = new UpdateCategoryCommand(category.Id, "NewName", null, null, false);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            category.Name.Should().Be("NewName");
            _categoryRepositoryMock.Verify(x => x.Update(category), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateDescription_UpdatesDescription()
        {
            var category = Category.CreateRoot("Name", null, "slug", 1).Value;
            var command = new UpdateCategoryCommand(category.Id, null, "New description", null, false);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            category.Description.Should().Be("New description");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_MoveToRoot_MovesCategory()
        {
            var parent = Category.CreateRoot("Parent", null, "parent", 1).Value;
            var category = Category.CreateSubCategory("Child", null, "child", 1, parent).Value;
            var command = new UpdateCategoryCommand(category.Id, null, null, null, true);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _categoryQueryMock
                .Setup(x => x.HasChildrenAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            category.ParentCategoryId.Should().BeNull();
            category.Level.Should().Be(1);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ChangeParentCategory_ReturnsSuccess()
        {
            var category = Category.CreateRoot("Cat", null, "cat", 1).Value;
            var newParent = Category.CreateRoot("Parent", null, "parent", 1).Value;
            var command = new UpdateCategoryCommand(category.Id, null, null, newParent.Id, false);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _categoryQueryMock
                .Setup(x => x.HasChildrenAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryQueryMock
                .Setup(x => x.GetByIdAsync(newParent.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newParent);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            category.ParentCategoryId.Should().Be(newParent.Id);
            category.Level.Should().Be(2);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_MoveCategoryWithChildren_ReturnsConflictError()
        {
            var category = Category.CreateRoot("Cat", null, "cat", 1).Value;
            var command = new UpdateCategoryCommand(category.Id, null, null, null, true);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _categoryQueryMock
                .Setup(x => x.HasChildrenAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.cannot_move_parent_with_children");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ChangeParentCategoryNotFound_ReturnsNotFoundError()
        {
            var category = Category.CreateRoot("Cat", null, "cat", 1).Value;
            var parentId = Guid.NewGuid();
            var command = new UpdateCategoryCommand(category.Id, null, null, parentId, false);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _categoryQueryMock
                .Setup(x => x.HasChildrenAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _categoryQueryMock
                .Setup(x => x.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.parent_not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NoChanges_StillSaves()
        {
            var category = Category.CreateRoot("Name", "Desc", "slug", 1).Value;
            var command = new UpdateCategoryCommand(category.Id, null, null, null, false);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _categoryRepositoryMock.Verify(x => x.Update(category), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
