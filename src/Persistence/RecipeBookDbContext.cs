using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using RecipeBook.Core.Application;
using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Core.Domain.Units;

namespace RecipeBook.Infrastructure.Persistence
{
    public class RecipeBookDbContext : DbContext, IRecipeRepository
    {
        private DbSet<Recipe>       Recipes       { get; set; } = null!; // Will be set by EF.
        private DbSet<UsedOccasion> UsedOccasions { get; set; } = null!; // Will be set by EF.
        private DbSet<Step>         Steps         { get; set; } = null!; // Will be set by EF.
        private DbSet<Ingredient>   Ingredients   { get; set; } = null!; // Will be set by EF.
        private DbSet<Unit>         Units         { get; set; } = null!; // Will be set by EF.
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string connectionString = Environment.GetEnvironmentVariable("RecipeBookDBConnectionString") ?? throw
                new Exception("Could not locate RecipeBookDBConnectionString environment variable.");
            
            options.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UsedOccasion>().HasKey(usedOccasion => usedOccasion.When);
            builder.Entity<Mass>().ToTable("Masses");
            builder.Entity<Volume>().ToTable("Volumes");
        }

        public async Task<Recipe?> FetchRecipe(int id)
        {
            return await Recipes.FindAsync(id);
        }

        public async Task<IEnumerable<Recipe>> FetchAllRecipes()
        {
            return await Recipes.ToListAsync();
        }
    }
}