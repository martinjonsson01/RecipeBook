using System;

using FluentValidation;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application.Validation
{
    public class TimeStepValidator : AbstractValidator<TimeStep>
    {
        private static readonly TimeSpan MinDuration = TimeSpan.Zero;
        private static readonly TimeSpan MaxDuration = TimeSpan.FromDays(10);

        private readonly StepValidator _stepValidator = new();

        public TimeStepValidator()
        {
            RuleFor(timeStep => timeStep).SetValidator(_stepValidator);
            RuleFor(timeStep => timeStep.Duration)
                .ExclusiveBetween(MinDuration, MaxDuration)
                .WithMessage($"Varaktigheten måste vara mellan {MinDuration} och {MaxDuration}");
        }
    }
}