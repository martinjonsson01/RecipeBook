using System;

namespace RecipeBook.Core.Domain.Recipes
{
    public class Step : BaseEntity, IShallowCloneable<Step>, IEquatable<Step>
    {
        public int    Number      { get; set; } = 1;
        public string Instruction { get; set; } = string.Empty;
        
        public Step   ShallowClone()
        {
            return new()
            {
                Id = Id,
                Number = Number,
                Instruction = Instruction
            };
        }

        public bool Equals(Step? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Number == other.Number && Instruction == other.Instruction;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Step) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Number * 397) ^ Instruction.GetHashCode();
            }
        }
    }
}