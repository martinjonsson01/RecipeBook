using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Core.Domain.Units;
using RecipeBook.Infrastructure.Persistence.Repositories;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public class IngredientsRepositoryTests
        : IntegerKeyRepositoryTestsBase<IngredientsRepository, Ingredient>
    {
        public IngredientsRepositoryTests(DatabaseFixture fixture) : base(fixture.Db)
        {
            Repo = new IngredientsRepository(MockLogger.Object, fixture.ConnectionString);
        }

        protected override string InsertResourceSql(int recipeId)
        {
            throw new MethodAccessException();
        }

        private static string InsertIngredientSql(
            bool   trueForMassFalseForVolume,
            int?   id,
            string name,
            int    recipeId,
            double value) => $@"
            WITH insert_ingredient AS (
               INSERT INTO ingredients (id, name, recipeid)
               VALUES ({id}, '{name}', {recipeId})
               RETURNING id AS ingredient_id, name AS ingredient_name
            ),
            insert_unit AS (
               INSERT INTO units (id, value)
               SELECT ingredient_id, {value.ToString(CultureInfo.InvariantCulture)} FROM insert_ingredient
               RETURNING id AS unit_id, value AS unit_value
            )
            INSERT INTO {(trueForMassFalseForVolume ? "masses" : "volumes")} (id)
            SELECT unit_id FROM insert_unit
            RETURNING
                (SELECT ingredient_id AS IngredientId FROM insert_ingredient),
                (SELECT ingredient_name as IngredientName FROM insert_ingredient),
                (SELECT unit_id AS {(trueForMassFalseForVolume ? "MassId" : "VolumeId")} FROM insert_unit),
                (SELECT NULL AS {(trueForMassFalseForVolume ? "VolumeId" : "MassId")} FROM insert_unit),
                (SELECT unit_value as value FROM insert_unit);
        ";

        protected override string ResourceExistsSql =>
            "SELECT EXISTS(SELECT 1 FROM ingredients WHERE id = :key)";

        protected override async Task<Ingredient> MockResource(int? key = default)
        {
            key ??= await MockKey();
            return new Ingredient
            {
                Id = key,
                Name = Faker.Lorem.Sentence(),
                Amount = Faker.PickRandom<Unit>(
                    new Mass { Id = key, Value = Faker.Random.Double(10) },
                    new Volume { Id = key, Value = Faker.Random.Double(10) }
                )
            };
        }

        protected override async Task<Ingredient> StoreInDatabase(string recipeName, Ingredient mockedResource)
        {
            var recipeId = await Db.QuerySingleAsync<int>(InsertOrGetParentRecipeSql, new { recipeName });
            string sql = InsertIngredientSql(mockedResource.Amount is Mass, mockedResource.Id, mockedResource.Name,
                recipeId,
                mockedResource.Amount.Value);
            
            IEnumerable<dynamic> results = await Db.QueryAsync(sql);
            return results.Select(Ingredient.MapFromRow)
                   .First();
        }
    }
}