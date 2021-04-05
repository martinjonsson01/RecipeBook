using FluentValidation;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application.Validation
{
    public class RecipeValidator : AbstractValidator<Recipe>
    {
        public RecipeValidator()
        {
            RuleFor(recipe => recipe.Name)
                .NotEmpty()
                .MaximumLength(MaxNameLength)
                .WithMessage($"Namnet måste vara under {MaxNameLength} karaktärer långt.");
            RuleFor(recipe => recipe.Rating)
                .InclusiveBetween(MinRating, MaxRating)
                .WithMessage($"Betyget måste vara från {MinRating}-{MaxRating}.");
        }

        private const int MaxNameLength = 100;
        private const int MinRating     = 1;
        private const int MaxRating     = 10;
    }
}