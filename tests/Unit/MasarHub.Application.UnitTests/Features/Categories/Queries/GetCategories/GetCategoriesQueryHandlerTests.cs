using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Categories.Queries.GetCategories;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Categories.Queries.GetCategories
{
    [Trait("UnitTests", "Feature.Categories.GetCategories.Handler")]
    public sealed class GetCategoriesQueryHandlerTests
    {
        private readonly Mock<ICategoryQuery> _categoryQueryMock;
        private readonly GetCategoriesQueryHandler _sut;

        public GetCategoriesQueryHandlerTests()
        {
            _categoryQueryMock = new Mock<ICategoryQuery>();
            _sut = new GetCategoriesQueryHandler(_categoryQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPaginatedResult()
        {
            var query = new GetCategoriesQuery(null, null, 1, 10);
            var categories = new List<CategoryResponse>
            {
                new(Guid.NewGuid(), "Cat1", null, "cat1", 1, 1, null, DateTimeOffset.UtcNow),
                new(Guid.NewGuid(), "Cat2", null, "cat2", 1, 2, null, DateTimeOffset.UtcNow)
            };

            _categoryQueryMock
                .Setup(x => x.GetAllAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<CategoryResponse>(categories, 2));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(2);
            result.Value.TotalCount.Should().Be(2);
            result.Value.PageNumber.Should().Be(1);
            result.Value.PageSize.Should().Be(10);
            result.Value.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
        {
            var query = new GetCategoriesQuery(null, null, 1, 10);

            _categoryQueryMock
                .Setup(x => x.GetAllAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<CategoryResponse>([], 0));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);
        }
    }
}
