using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Categories.Commands.DeleteCategory;
using MasarHub.Domain.Modules.Categories;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Categories.Commands.DeleteCategory
{
    [Trait("UnitTests", "Feature.Categories.DeleteCategory.Handler")]
    public sealed class DeleteCategoryCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICategoryQuery> _categoryQueryMock;
        private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
        private readonly DeleteCategoryCommandHandler _sut;

        public DeleteCategoryCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryQueryMock = new Mock<ICategoryQuery>();
            _categoryRepositoryMock = new Mock<IRepository<Category>>();
            _sut = new DeleteCategoryCommandHandler(_unitOfWorkMock.Object, _categoryQueryMock.Object, _categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_CategoryNotFound_ReturnsNotFoundError()
        {
            var command = new DeleteCategoryCommand(Guid.NewGuid());

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CategoryHasChildren_ReturnsConflictError()
        {
            var category = Category.CreateRoot("Cat", null, "cat", 1).Value;
            var command = new DeleteCategoryCommand(category.Id);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _categoryQueryMock
                .Setup(x => x.CanDeleteAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CategoryDeletionCheckData(true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.has_children");
            _categoryRepositoryMock.Verify(x => x.Remove(It.IsAny<Category>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CategoryHasCourses_ReturnsConflictError()
        {
            var category = Category.CreateRoot("Cat", null, "cat", 1).Value;
            var command = new DeleteCategoryCommand(category.Id);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _categoryQueryMock
                .Setup(x => x.CanDeleteAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CategoryDeletionCheckData(false, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.has_courses");
            _categoryRepositoryMock.Verify(x => x.Remove(It.IsAny<Category>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CategoryDeletable_RemovesAndReturnsSuccess()
        {
            var category = Category.CreateRoot("Cat", null, "cat", 1).Value;
            var command = new DeleteCategoryCommand(category.Id);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _categoryQueryMock
                .Setup(x => x.CanDeleteAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CategoryDeletionCheckData(false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _categoryRepositoryMock.Verify(x => x.Remove(category), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
