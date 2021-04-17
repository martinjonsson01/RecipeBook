using System;

namespace RecipeBook.Core.Domain.Units
{
    public class Volume : Unit
    {
        public const string Liter      = "l";
        public const string Deciliter  = "dl";
        public const string Centiliter = "cl";
        public const string Milliliter = "ml";
        public const string TeaSpoon   = "tsk";
        public const string TableSpoon = "msk";
        public const string Krm        = "krm";

        private const double LitersPerTeaSpoon   = 0.005; //  1 tsk = 5 ml
        private const double LitersPerTableSpoon = 0.015d; // 1 msk = 3 tsk = 15 ml
        private const double LitersPerLiter      = 1.0d;
        private const double LitersPerDeciliter  = 0.1d;
        private const double LitersPerCentiliter = 0.01d;
        private const double LitersPerMilliliter = 0.001d;

        private const double DecilitersPerLiter  = 1.0d / LitersPerDeciliter;
        private const double CentilitersPerLiter = 1.0d / LitersPerCentiliter;
        private const double MillilitersPerLiter = 1.0d / LitersPerMilliliter;

        public Volume()
        {
        }

        public Volume(double liters)
        {
            Value = liters;
        }

        public static Volume FromTableSpoons(double tableSpoons)
        {
            return new(tableSpoons * LitersPerTableSpoon);
        }

        public static Volume FromTeaSpoons(double teaSpoons)
        {
            return new(teaSpoons * LitersPerTeaSpoon);
        }

        public static Volume FromDeciliters(double deciliters)
        {
            return new(deciliters * LitersPerDeciliter);
        }

        public static Volume FromCentiliters(double centiliters)
        {
            return new(centiliters * LitersPerCentiliter);
        }

        public static Volume FromMilliliters(double milliliters)
        {
            return new(milliliters * LitersPerMilliliter);
        }

        // Kryddmått
        public static Volume FromKrm(double krm)
        {
            return FromMilliliters(krm);
        }

        public override string ToString()
        {
            double roundedValue = Math.Round(Value, 3, MidpointRounding.AwayFromZero);
            double ml           = roundedValue * 1000;
            if (ml % (MillilitersPerLiter / 1) == 0) return $"{roundedValue:#.##} {Liter}";
            if (ml % (MillilitersPerLiter / 10) == 0) return $"{(roundedValue * DecilitersPerLiter):#.##} {Deciliter}";
            if (ml % (MillilitersPerLiter / 100) == 0)
                return $"{(roundedValue * CentilitersPerLiter):#.##} {Centiliter}";
            return $"{roundedValue * MillilitersPerLiter} {Milliliter}";
        }
    }
}