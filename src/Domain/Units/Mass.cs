﻿namespace RecipeBook.Core.Domain.Units
{
    public record Mass(double Kilograms) : Unit(Kilograms)
    {
        private const string Kilogram         = "kg";
        private const string Gram             = "g";
        private const double GramsInAKilogram = 1000.0d; // 1000 g = 1 kg

        public static Mass FromGrams(int grams)
        {
            return new(grams / GramsInAKilogram); 
        }

        public override string ToString()
        {
            return Value switch
            {
                < 1.0d => $"{Value * GramsInAKilogram} {Gram}",
                _      => $"{Value} {Kilogram}"
            };
        }
    }
}