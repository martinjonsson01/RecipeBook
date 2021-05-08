using System;

namespace RecipeBook.Core.Domain.Units
{
    public class Amount : Unit
    {
        public const string Piece = "st";
        
        public Amount()
        {
        }

        public Amount(int count)
        {
            Value = count;
        }

        public override string ToString()
        {
            return $"{(int) Math.Round(Value, 0)} {Piece}";
        }
    }
}