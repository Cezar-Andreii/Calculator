using System;
using System.Globalization;

namespace Calculator
{
    public class NumberBaseConverter
    {
        public string ConvertToBase(decimal value, NumberBase targetBase)
        {
            if (targetBase == NumberBase.Decimal)
                return value.ToString(CultureInfo.InvariantCulture);


            long integerPart = (long)Math.Floor(value);

            switch (targetBase)
            {
                case NumberBase.Binary:
                    return Convert.ToString(integerPart, 2).ToUpperInvariant();
                case NumberBase.Octal:
                    return Convert.ToString(integerPart, 8).ToUpperInvariant();
                case NumberBase.Hexadecimal:
                    return Convert.ToString(integerPart, 16).ToUpperInvariant();
                default:
                    throw new ArgumentException("Bază de numerație nesuportată", nameof(targetBase));
            }
        }

        public decimal ConvertFromBase(string value, NumberBase sourceBase)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            if (sourceBase == NumberBase.Decimal)
                return decimal.Parse(value, CultureInfo.InvariantCulture);

            try
            {
                return Convert.ToInt64(value, (int)sourceBase);
            }
            catch (Exception)
            {
                throw new ArgumentException("Valoare invalidă pentru baza specificată");
            }
        }

        public bool IsValidForBase(char c, NumberBase baseSystem)
        {
            switch (baseSystem)
            {
                case NumberBase.Binary:
                    return c == '0' || c == '1';
                case NumberBase.Octal:
                    return c >= '0' && c <= '7';
                case NumberBase.Decimal:
                    return char.IsDigit(c) || c == '.' || c == '-';
                case NumberBase.Hexadecimal:
                    return (c >= '0' && c <= '9') ||
                           (c >= 'A' && c <= 'F') ||
                           (c >= 'a' && c <= 'f');
                default:
                    return false;
            }
        }
    }
}