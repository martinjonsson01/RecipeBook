using System;

using RecipeBook.Core.Domain.Units;

namespace RecipeBook.Core.Domain.Recipes
{
    public class Ingredient : BaseEntity, IShallowCloneable<Ingredient>, IEquatable<Ingredient>
    {
        public string Name   { get; set; } = string.Empty;
        public Unit   Amount { get; set; } = new Mass(0);

        public static Ingredient MapFromRow(dynamic row)
        {
            var ingredient = new Ingredient
            {
                Id = row.ingredientid,
                Name = row.ingredientname,
                Amount = row.massid is not null
                    ? new Mass { Id = row.massid, Value = row.value }
                    : new Volume { Id = row.volumeid, Value = row.value }
            };
            return ingredient;
        }

        public Ingredient ShallowClone()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                Amount = Amount
            };
        }

        public bool Equals(Ingredient? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Amount.Equals(other.Amount);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Ingredient) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode() * 397) ^ Amount.GetHashCode();
            }
        }
    }
}