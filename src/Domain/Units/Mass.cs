namespace RecipeBook.Core.Domain.Units
{
    public class Mass : Unit
    {
        public const string Kilogram = "kg";
        public const string Gram     = "g";

        private const double GramsInAKilogram = 1000.0d; // 1000 g = 1 kg

        public Mass()
        {
        }

        public Mass(double kilograms)
        {
            Value = kilograms;
        }

        public static Mass FromGrams(double grams)
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