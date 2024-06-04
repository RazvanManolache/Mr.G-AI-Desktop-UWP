using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MrG.Desktop.Converters
{
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int count)
            {
                var cond = count > 0;
                
                if (parameter != null)
                {
                    cond = !cond;
                }
                return cond ? Visibility.Visible : Visibility.Collapsed; ; 
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
