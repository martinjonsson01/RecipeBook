using FluentValidation;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application.Validation
{
    public class IngredientValidator : AbstractValidator<Ingredient>
    {
        public IngredientValidator()
        {
            RuleFor(ingredient => ingredient.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(MaxNameLength)
                .WithMessage($"Namnet kan inte vara längre än {MaxNameLength} karaktärer.");
            RuleFor(ingredient => ingredient.Amount.Value)
                .GreaterThan(MinIngredientAmount)
                .WithMessage($"Mängden av ingrediensen måste vara över {MinIngredientAmount}.");
        }

        private const int MaxNameLength       = 100;
        private const int MinIngredientAmount = 0;
    }
}