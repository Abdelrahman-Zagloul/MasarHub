using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Categories.Commands.CreateCategory;
using MasarHub.Domain.Modules.Categories;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Categories.Commands.CreateCategory
{
    [Trait("UnitTests.Feature.Categories", "CreateCategory")]
    public sealed class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICategoryQuery> _categoryQueryMock;
        private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
        private readonly CreateCategoryCommandHandler _sut;

        public CreateCategoryCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryQueryMock = new Mock<ICategoryQuery>();
            _categoryRepositoryMock = new Mock<IRepository<Category>>();
            _sut = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _categoryQueryMock.Object, _categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRootCategory_ReturnsSuccessResponse()
        {
            var command = new CreateCategoryCommand("Programming", "Development category", null);

            _categoryQueryMock
                .Setup(x => x.GetCreationDataAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CategoryCreationData(1, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Programming");
            result.Value.DisplayOrder.Should().Be(1);
            result.Value.Level.Should().Be(1);
            result.Value.ParentCategoryId.Should().BeNull();
            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidSubCategory_ReturnsSuccessResponse()
        {
            var parent = Category.CreateRoot("Parent", null, "parent", 1).Value;
            var command = new CreateCategoryCommand("C#", "C# language", parent.Id);

            _categoryQueryMock
                .Setup(x => x.GetCreationDataAsync(It.IsAny<string>(), parent.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CategoryCreationData(2, false));
            _categoryQueryMock
                .Setup(x => x.GetByIdAsync(parent.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(parent);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("C#");
            result.Value.Level.Should().Be(2);
            result.Value.DisplayOrder.Should().Be(2);
            result.Value.ParentCategoryId.Should().Be(parent.Id);
            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_SlugExists_ReturnsConflictError()
        {
            var command = new CreateCategoryCommand("Programming", null, null);

            _categoryQueryMock
                .Setup(x => x.GetCreationDataAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CategoryCreationData(1, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.slug_already_exists");
            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ParentCategoryNotFound_ReturnsNotFoundError()
        {
            var parentId = Guid.NewGuid();
            var command = new CreateCategoryCommand("Sub", null, parentId);

            _categoryQueryMock
                .Setup(x => x.GetCreationDataAsync(It.IsAny<string>(), parentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CategoryCreationData(1, false));
            _categoryQueryMock
                .Setup(x => x.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.not_found");
            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DomainFailure_ReturnsFailure()
        {
            var root = Category.CreateRoot("Root", null, "root", 1).Value;
            var l2 = Category.CreateSubCategory("L2", null, "l2", 1, root).Value;
            var l3 = Category.CreateSubCategory("L3", null, "l3", 1, l2).Value;
            var command = new CreateCategoryCommand("TooDeep", null, l3.Id);

            _categoryQueryMock
                .Setup(x => x.GetCreationDataAsync(It.IsAny<string>(), l3.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CategoryCreationData(1, false));
            _categoryQueryMock
                .Setup(x => x.GetByIdAsync(l3.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(l3);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
