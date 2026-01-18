
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BO;

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
    public class ModeToVisibilityOrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isAddMode = (bool)value;
            return isAddMode ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
    public class NullCurrentOrderToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value = CurrentOrderId
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class OrderStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals(BO.OrderStatus.Open) || value.Equals(BO.OrderStatus.InProgress))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ModeToVisibilityActiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool active = (bool)value;
            return active ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }

    public class ModeEnableSimulatorConverter : IValueConverter
    {
        //returns true if in update mode (not add mode)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool is_running = (bool)value;


            return !is_running;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class OrderStatusAndAddModeToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return Visibility.Collapsed;

            // ערך ראשון = OrderStatus
            var status = values[0] as OrderStatus?;

            // ערך שני = isAddMode
            bool isAddMode = false;
            if (values[1] is bool b)
                isAddMode = b;

            // אם במצב הוספה → נסתר
            if (isAddMode)
                return Visibility.Collapsed;

            // אם ההזמנה קיימת (סטטוס Open או InProgress) → מופיע
            if (status == OrderStatus.Open || status == OrderStatus.InProgress)
                return Visibility.Visible;

            // אחרת נסתר
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}