using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application
{
    public class RecipeValidator : AbstractValidator<Recipe>
    {
        public RecipeValidator()
        {
            RuleFor(recipe => recipe.Name)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(recipe => recipe.Rating)
                .InclusiveBetween(1, 10);
        }
    }
}