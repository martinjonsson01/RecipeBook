﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RecipeBook.Core.Domain.Units
{
    public abstract class Unit : BaseEntity, IEquatable<Unit>
    {
        public double Value { get; set; }

        private const char Separator = ' ';

        public static bool TryParseString(string? value, out Unit result, out string? errorMessage)
        {
            result = new Mass();
            errorMessage = null;

            value = value?.Trim();
            if (string.IsNullOrEmpty(value))
                return Failure("Måste innehålla text", out errorMessage);

            string[] parts = value.Split(Separator);
            return TryParseParts(ref result, ref errorMessage, parts);
        }

        private static bool TryParseParts(ref Unit result, ref string? errorMessage, IReadOnlyList<string> parts)
        {
            double number = double.MinValue;
            if (parts.Count == 3)
                if (!TryParseFraction(parts[0], parts[1], ref number))
                    return Failure("Måste börja med ett tal. T.ex. '1 1/2 msk'", out errorMessage);

            if (Math.Abs(number - double.MinValue) < 0.000001f)
                if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.CurrentCulture, out number))
                    if (!TryParseFraction(parts.Count == 3 ? parts[1] : parts[0], ref number))
                        return Failure("Måste börja med ett tal. T.ex. '1/2 msk'", out errorMessage);

            if (parts.Count == 1)
                return TryParseAmount(out result, number);

            if (parts.Count != 2 && parts.Count != 3)
                return Failure("Måste vara i formatet '{tal} {enhet}'. T.ex. '10 g'", out errorMessage);

            string unitText = parts[^1].ToLowerInvariant();
            return TryParseUnitText(ref result, ref errorMessage, unitText, number);
        }

        private static bool TryParseFraction(string wholeText, string fractionText, ref double result)
        {
            if (!int.TryParse(wholeText, NumberStyles.Integer, CultureInfo.CurrentCulture, out int wholes))
                return false;
            if (!TryParseFraction(fractionText, ref result))
                return false;
            result += wholes;
            return true;
        }

        private static bool TryParseFraction(string text, ref double result)
        {
            string[] parts = text.Split("/");
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.CurrentCulture, out int numerator))
                return false;
            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.CurrentCulture, out int denominator))
                return false;
            if (denominator == 0) return false;
            result = (double) numerator / denominator;
            return true;
        }

        private static bool TryParseUnitText(ref Unit result, ref string? errorMessage, string unitText, double number)
        {
            if (unitText.Contains(Mass.Gram))
            {
                return TryParseGrams(ref result, ref errorMessage, unitText, number);
            }
            if (unitText.Contains(Volume.Liter))
            {
                return TryParseLiters(ref result, ref errorMessage, unitText, number);
            }
            if (unitText.Contains(Amount.Piece))
            {
                return TryParseAmount(out result, number);
            }
            return TryParseMisc(ref result, ref errorMessage, unitText, number);
        }

        private static bool TryParseGrams(ref Unit result, ref string? errorMessage, string unitText, double number)
        {
            return unitText switch
            {
                Mass.Kilogram => Success(new Mass(number),       out result),
                Mass.Gram     => Success(Mass.FromGrams(number), out result),
                _             => Failure("Kan inte känna igen gramprefix", out errorMessage)
            };
        }

        private static bool TryParseLiters(ref Unit result, ref string? errorMessage, string unitText, double number)
        {
            return unitText switch
            {
                Volume.Liter      => Success(new Volume(number),             out result),
                Volume.Deciliter  => Success(Volume.FromDeciliters(number),  out result),
                Volume.Centiliter => Success(Volume.FromCentiliters(number), out result),
                Volume.Milliliter => Success(Volume.FromMilliliters(number), out result),
                _                 => Failure("Kan inte känna igen literprefix", out errorMessage)
            };
        }

        private static bool TryParseAmount(out Unit result, double number)
        {
            return Success(new Amount((int) number), out result);
        }

        private static bool TryParseMisc(ref Unit result, ref string? errorMessage, string unitText, double number)
        {
            return unitText switch
            {
                Volume.TeaSpoon   => Success(Volume.FromTeaSpoons(number),   out result),
                Volume.TableSpoon => Success(Volume.FromTableSpoons(number), out result),
                Volume.Krm        => Success(Volume.FromKrm(number),         out result),
                _                 => Failure("Kan inte känna igen enhet", out errorMessage)
            };
        }

        private static bool Success(Unit unit, out Unit result)
        {
            result = unit;
            return true;
        }

        private static bool Failure(string message, out string? errorMessage)
        {
            errorMessage = message;
            return false;
        }

        public bool Equals(Unit? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.ToString().Equals(ToString());
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
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.CurrentCulture);
        }
    }
}