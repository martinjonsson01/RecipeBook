using System.Collections.Generic;

using FluentValidation;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application.Validators
{
    public class StepsValidator : AbstractValidator<IList<Step>>
    {
        public StepsValidator()
        {
            RuleFor(steps => steps).Must(StartAtStepOne).WithMessage("Stegen måste börja på 1.");
            RuleFor(steps => steps).Must(IncreaseByOne).WithMessage("Stegen måste öka med 1.");
            RuleForEach(steps => steps).SetValidator(new StepValidator());
            RuleForEach(steps => steps).SetInheritanceValidator(validator =>
            {
                validator.Add(new TimeStepValidator());
            });
        }

        private static bool IncreaseByOne(IList<Step> steps)
        {
            for (var i = 1; i < steps.Count; i++)
            {
                Step previous = steps[i - 1];
                Step current  = steps[i];
                if (current.Number - previous.Number != 1)
                    return false;
            }
            return true;
        }

        private static bool StartAtStepOne(IList<Step> steps)
        {
            if (steps.Count == 0) return false;
            return steps[0].Number == 1;
        }
    }
}