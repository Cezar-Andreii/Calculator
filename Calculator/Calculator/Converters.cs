using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Calculator
{
    public class ModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CalculatorMode mode && parameter is string targetMode)
            {
                return mode.ToString() == targetMode ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NumberBase currentBase && parameter is string targetBase)
            {
                return currentBase.ToString() == targetBase;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value && parameter is string targetBase)
            {
                return Enum.Parse(typeof(NumberBase), targetBase);
            }
            return NumberBase.Decimal;
        }
    }
}