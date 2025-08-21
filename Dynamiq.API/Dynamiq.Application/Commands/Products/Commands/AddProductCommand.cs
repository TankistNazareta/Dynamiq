using Dynamiq.Domain.Enums;
using MediatR;

namespace Dynamiq.Application.Commands.Products.Commands
{
    public record class AddProductCommand(
        string Name,
        string Description, 
        int Price,
        IntervalEnum Interval, 
        Guid CategoryId,
        List<string> ImgUrls
    ) : IRequest;
}
