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
            RuleForEach(recipe => recipe.UsedOccasions)
                .SetValidator(new UsedOccasionValidator());
            RuleFor(recipe => recipe.Steps)
                .SetValidator(new StepsValidator());
            RuleForEach(recipe => recipe.Ingredients)
                .SetValidator(new IngredientValidator());
        }

        private const int MaxNameLength = 100;
        private const int MinRating     = 1;
        private const int MaxRating     = 10;
    }
}