using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Carts.Models;
using MediatR;

namespace MasarHub.Application.Features.Carts.Commands.AddToCart
{
    public sealed class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result>
    {
        private readonly ICartService _cartService;
        private readonly ICourseQuery _courseQuery;

        public AddToCartCommandHandler(ICartService cartService, ICourseQuery courseQuery)
        {
            _cartService = cartService;
            _courseQuery = courseQuery;
        }

        public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var courseData = await _courseQuery.GetCourseCartDataAsync(request.CourseId, cancellationToken);
            if (courseData == null)
                return Error.NotFound("course.not_found");

            if (!courseData.IsPublished)
                return Error.BadRequest("course.not_published");

            var item = CartItem.Create(courseData.Id, courseData.Title, courseData.Price, courseData.ThumbnailPublicId);
            var result = await _cartService.AddItemAsync(request.UserId, item, cancellationToken);
            return result;
        }
    }
}