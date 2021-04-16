using System;

namespace RecipeBook.Core.Domain.Units
{
    public abstract class Unit : BaseEntity, IEquatable<Unit>
    {
        public double Value { get; set; }

        public bool Equals(Unit? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Unit) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}