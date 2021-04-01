using System;

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
            options.UseNpgsql("Host=localhost;Database=RecipeBookDB;Username=postgres;Password=password");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UsedOccasion>().HasKey(usedOccasion => usedOccasion.When);
            builder.Entity<Mass>().ToTable("Masses");
            builder.Entity<Volume>().ToTable("Volumes");
        }
    }
}