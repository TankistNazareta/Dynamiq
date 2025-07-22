using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Enums;
using Dynamiq.Domain.Enums;
using MediatR;

namespace Dynamiq.Application.Commands.Products.Commands
{
    public record class UpdateProductCommand(Guid Id, string Name,
            string Description, int Price, IntervalEnum Interval) : IRequest;
}
