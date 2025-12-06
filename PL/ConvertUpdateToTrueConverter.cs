
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;


namespace PL;

public class ConvertUpdateToTrueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        if (value is string buttonText && buttonText == "Update")
            return true;

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

        throw new NotImplementedException();
    }
}

public class ConvertUpdateToVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        
        if (value is string buttonText && buttonText == "Update")
            return Visibility.Visible;

        
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}