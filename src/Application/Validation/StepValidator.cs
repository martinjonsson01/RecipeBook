using FluentValidation;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application.Validation
{
    public class StepValidator : AbstractValidator<Step>
    {
        private const int MaxInstructionLength = 500;

        public StepValidator()
        {
            RuleFor(step => step.Number)
                .GreaterThan(0)
                .WithMessage("Stegnumret måste vara över 0.");
            RuleFor(step => step.Instruction)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .WithMessage("Instruktionen måste innehålla text.");
            RuleFor(step => step.Instruction)
                .MaximumLength(MaxInstructionLength)
                .WithMessage($"Instruktionen måste vara kortare än {MaxInstructionLength} karaktärer.");
        }
    }
}