using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Categories.Queries.GetCategoryById
{
    [Trait("UnitTests.Feature.Categories", "GetCategoryById")]
    public sealed class GetCategoryByIdQueryHandlerTests
    {
        private readonly Mock<ICategoryQuery> _categoryQueryMock;
        private readonly GetCategoryByIdQueryHandler _sut;

        public GetCategoryByIdQueryHandlerTests()
        {
            _categoryQueryMock = new Mock<ICategoryQuery>();
            _sut = new GetCategoryByIdQueryHandler(_categoryQueryMock.Object);
        }

        [Fact]
        public async Task Handle_CategoryFound_ReturnsCategoryWithChildren()
        {
            var categoryId = Guid.NewGuid();
            var query = new GetCategoryByIdQuery(categoryId);
            var categoryResponse = new CategoryResponse(categoryId, "Programming", "Desc", "programming", 1, 1, null, DateTimeOffset.UtcNow);
            var response = new CategoryWithChildrenResponse(categoryResponse, []);

            _categoryQueryMock
                .Setup(x => x.GetWithChildrenByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(response);
        }

        [Fact]
        public async Task Handle_CategoryNotFound_ReturnsNotFoundError()
        {
            var query = new GetCategoryByIdQuery(Guid.NewGuid());

            _categoryQueryMock
                .Setup(x => x.GetWithChildrenByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryWithChildrenResponse?)null);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.not_found");
        }
    }
}
