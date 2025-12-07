
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace PL.Courier
{
    public class ModeToReadOnlyConverter : IValueConverter
    {
        //returns true if in update mode (not add mode)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isAddMode = (bool)value;


            return !isAddMode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //returns Collapsed if in add mode, Visible if in update mode
    public class ModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isAddMode = (bool)value;
            return isAddMode ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}