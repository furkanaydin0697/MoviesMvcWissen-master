using _036_MoviesMvcWissen.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _036_MoviesMvcWissen.Valudations.FluentValidation
{
    public class DirectorValidator : AbstractValidator<Director>
    {
        public DirectorValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("Director name can not be empty!").MaximumLength(50).WithMessage("Director name must be 50 characters!");
            RuleFor(e => e.Surname).NotEmpty().Length(3, 50);
        }
    }
}