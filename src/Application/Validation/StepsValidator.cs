using System;
using System.Collections.Generic;

using FluentValidation;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application.Validation
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

    internal class StepValidator : AbstractValidator<Step>
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

    internal class TimeStepValidator : AbstractValidator<TimeStep>
    {
        private static readonly TimeSpan      MinDuration    = TimeSpan.Zero;
        private static readonly TimeSpan      MaxDuration    = TimeSpan.FromDays(10);
        
        private readonly        StepValidator _stepValidator = new();
        
        public TimeStepValidator()
        {
            RuleFor(timeStep => timeStep).SetValidator(_stepValidator);
            RuleFor(timeStep => timeStep.Duration)
                .ExclusiveBetween(MinDuration, MaxDuration)
                .WithMessage($"Varaktigheten måste vara mellan {MinDuration} och {MaxDuration}");
        }
    }
}