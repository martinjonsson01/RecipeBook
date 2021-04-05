using System;

using FluentValidation;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application
{
    public class UsedOccasionValidator : AbstractValidator<UsedOccasion>
    {
        public UsedOccasionValidator()
        {
            RuleFor(occasion => occasion.Duration)
                .ExclusiveBetween(TimeSpan.Zero, TimeSpan.FromDays(1));
            RuleFor(occasion => occasion.Comment)
                .MaximumLength(500);
        }
    }
}