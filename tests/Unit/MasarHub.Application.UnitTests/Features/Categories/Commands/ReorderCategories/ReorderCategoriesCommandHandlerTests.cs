using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Categories.Commands.ReorderCategories;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Categories.Commands.ReorderCategories
{
    [Trait("UnitTests.Feature.Categories", "ReorderCategories")]
    public sealed class ReorderCategoriesCommandHandlerTests
    {
        private readonly Mock<ICategoryQuery> _categoryQueryMock;
        private readonly ReorderCategoriesCommandHandler _sut;

        public ReorderCategoriesCommandHandlerTests()
        {
            _categoryQueryMock = new Mock<ICategoryQuery>();
            _sut = new ReorderCategoriesCommandHandler(_categoryQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidReorder_ReturnsSuccess()
        {
            var categoryIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var command = new ReorderCategoriesCommand(null, categoryIds);

            _categoryQueryMock
                .Setup(x => x.GetCategoryIdsByParentIdAsync(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoryIds);
            _categoryQueryMock
                .Setup(x => x.BulkUpdateDisplayOrderAsync(null, command.OrderedCategoryIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_CategoryIdsMismatch_ReturnsBadRequest()
        {
            var existingIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var orderedIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var command = new ReorderCategoriesCommand(null, orderedIds);

            _categoryQueryMock
                .Setup(x => x.GetCategoryIdsByParentIdAsync(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingIds);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.reorder_items_mismatch");
            _categoryQueryMock.Verify(x => x.BulkUpdateDisplayOrderAsync(It.IsAny<Guid?>(), It.IsAny<IReadOnlyList<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CategoryNotFoundInExisting_ReturnsBadRequest()
        {
            var existingIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var orderedIds = new List<Guid> { existingIds[0], existingIds[1], Guid.NewGuid() };
            var command = new ReorderCategoriesCommand(null, orderedIds);

            _categoryQueryMock
                .Setup(x => x.GetCategoryIdsByParentIdAsync(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingIds);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.reorder_category_not_found");
            _categoryQueryMock.Verify(x => x.BulkUpdateDisplayOrderAsync(It.IsAny<Guid?>(), It.IsAny<IReadOnlyList<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_BulkUpdateFails_ReturnsFailure()
        {
            var categoryIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var command = new ReorderCategoriesCommand(null, categoryIds);

            _categoryQueryMock
                .Setup(x => x.GetCategoryIdsByParentIdAsync(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoryIds);
            _categoryQueryMock
                .Setup(x => x.BulkUpdateDisplayOrderAsync(null, command.OrderedCategoryIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.reorder_failed");
        }
    }
}
