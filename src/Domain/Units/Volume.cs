namespace RecipeBook.Core.Domain.Units
{
    public record Volume(double Liters) : Unit(Liters)
    {
        private const string Liter      = "l";
        private const string Deciliter  = "dl";
        private const string Centiliter = "cl";
        private const string Milliliter = "ml";

        private const double LitersPerTeaSpoon   = 0.005; //  1 tsk = 5 ml
        private const double LitersPerTableSpoon = 0.015d; // 1 msk = 3 tsk = 15 ml
        private const double LitersPerLiter      = 1.0d;
        private const double LitersPerDeciliter  = 0.1d;
        private const double LitersPerCentiliter = 0.01d;
        private const double LitersPerMilliliter = 0.001d;

        private const double DecilitersPerLiter  = 1.0d / LitersPerDeciliter;
        private const double CentilitersPerLiter = 1.0d / LitersPerCentiliter;
        private const double MillilitersPerLiter = 1.0d / LitersPerMilliliter;
        
        public static Volume FromTableSpoons(int tableSpoons)
        {
            return new(tableSpoons * LitersPerTableSpoon);
        }

        public static Volume FromTeaSpoons(int teaSpoons)
        {
            return new(teaSpoons * LitersPerTeaSpoon);
        }

        public static Volume FromMilliliters(int milliliters)
        {
            return new(milliliters * LitersPerMilliliter);
        }

        public override string ToString()
        {
            return Value switch
            {
                < LitersPerCentiliter => $"{Value * MillilitersPerLiter} {Milliliter}",
                < LitersPerDeciliter  => $"{Value * CentilitersPerLiter} {Centiliter}",
                < LitersPerLiter      => $"{Value * DecilitersPerLiter} {Deciliter}",
                _                     => $"{Value} {Liter}"
            };
        }
    }
}