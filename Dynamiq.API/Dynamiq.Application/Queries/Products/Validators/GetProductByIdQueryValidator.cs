using Dynamiq.Application.Queries.Products.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamiq.Application.Queries.Products.Validators
{
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required.");
        }
    }

}
