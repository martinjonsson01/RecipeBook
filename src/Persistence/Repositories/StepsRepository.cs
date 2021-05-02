using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Extensions.Logging;

using Npgsql;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Infrastructure.Persistence.Repositories
{
    public class StepsRepository : RepositoryBase<StepsRepository, Step, int?>
    {
        public StepsRepository(
            ILogger<StepsRepository> logger,
            string                   connectionString = "")
            : base(logger, connectionString: connectionString)
        {
        }

        protected override string GetAllSql => @"
                    SELECT * 
                      FROM steps
                 LEFT JOIN timesteps on steps.id = timesteps.id
                     WHERE recipeid = :recipeId
                  ORDER BY steps.id; 
            ";

        protected override string GetSql => @"
                    SELECT * 
                      FROM steps
                 LEFT JOIN timesteps on steps.id = timesteps.id
                     WHERE steps.id = :key
                       AND recipeid = :recipeId;
            ";

        public override async Task<IEnumerable<Step>> GetAllAsync(string recipeName)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            return await db.QueryAsync<Step, TimeStep, Step>(
                sql: GetAllSql,
                map: (step, timeStep) =>
                {
                    if (timeStep is null) return step;
                    timeStep.Number = step.Number;
                    timeStep.Instruction = step.Instruction;
                    return timeStep;
                },
                param: new { recipeId });
        }

        public override async Task<Step?> GetAsync(string recipeName, int? key)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            IEnumerable<Step> result = await db.QueryAsync<Step, TimeStep, Step>(
                sql: GetSql,
                map: (step, timeStep) =>
                {
                    if (timeStep is null) return step;
                    timeStep.Number = step.Number;
                    timeStep.Instruction = step.Instruction;
                    return timeStep;
                },
                param: new { key, recipeId });

            return result.FirstOrDefault();
        }
    }
}