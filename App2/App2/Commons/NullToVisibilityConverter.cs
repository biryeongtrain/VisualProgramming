using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace App2.Commons;

public class NullToVisibilityConverter : IValueConverter
{
    public Visibility NullValue { get; set; } = Visibility.Collapsed;
    public Visibility NotNullValue { get; set; } = Visibility.Visible;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (value == null) ? NullValue : NotNullValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}