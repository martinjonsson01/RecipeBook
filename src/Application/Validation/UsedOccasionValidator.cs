using System;

using FluentValidation;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application.Validation
{
    public class UsedOccasionValidator : AbstractValidator<UsedOccasion>
    {
        private static readonly TimeSpan MinDuration      = TimeSpan.Zero;
        private static readonly TimeSpan MaxDuration      = TimeSpan.FromDays(1);
        private const           int      MaxCommentLength = 500;
        public UsedOccasionValidator()
        {
            RuleFor(occasion => occasion.Duration)
                .ExclusiveBetween(MinDuration, MaxDuration)
                .WithMessage($"Varaktigheten måste vara mellan {MinDuration} och {MaxDuration}");
            RuleFor(occasion => occasion.Comment)
                .MaximumLength(MaxCommentLength)
                .WithMessage($"Kommentaren måste vara kortare än {MaxCommentLength} karaktärer.");
        }
    }
}