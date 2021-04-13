using Bogus;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Core.Domain.Units;

namespace Tests.Shared
{
    public static class Fakers
    {
        public static readonly Faker<Recipe> Recipe =
            new Faker<Recipe>()
                .RuleFor(recipe => recipe.Id,            f => f.IndexFaker)
                .RuleFor(recipe => recipe.Name,          f => f.Lorem.Sentence())
                .RuleFor(recipe => recipe.Rating,        f => f.Random.Number(1, 10))
                .RuleFor(recipe => recipe.ImagePath,     f => f.Internet.Avatar())
                .RuleFor(recipe => recipe.UsedOccasions, _ => UsedOccasion.Generate(2))
                .RuleFor(recipe => recipe.Steps,         _ => Step.Generate(2))
                .RuleFor(recipe => recipe.Ingredients,   _ => Ingredient.Generate(2));

        public static readonly Faker<UsedOccasion> UsedOccasion =
            new Faker<UsedOccasion>()
                .RuleFor(usedOccasion => usedOccasion.Id, f => f.IndexFaker)
                .RuleFor(usedOccasion => usedOccasion.Comment, f => f.Lorem.Sentence())
                .RuleFor(usedOccasion => usedOccasion.Date, f => f.Date.Recent())
                .RuleFor(usedOccasion => usedOccasion.Duration, f => f.Date.Timespan());

        public static readonly Faker<Step> Step =
            new Faker<Step>()
                .CustomInstantiator(f => f.PickRandom(new Step(), new TimeStep()))
                .RuleFor(step => step.Id,          f => f.IndexFaker)
                .RuleFor(step => step.Number,      f => f.IndexFaker)
                .RuleFor(step => step.Instruction, f => f.Lorem.Sentences(2));

        public static readonly Faker<Ingredient> Ingredient =
            new Faker<Ingredient>()
                .Rules((f, ingredient) =>
                {
                    int key = f.IndexFaker;
                    ingredient.Id = key;
                    ingredient.Name = f.Lorem.Sentence();
                    ingredient.Amount = f.PickRandom<Unit>(
                        new Mass { Id = key, Value = f.Random.Double(10) },
                        new Volume { Id = key, Value = f.Random.Double(10) }
                    );
                });
    }
}