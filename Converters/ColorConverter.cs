using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MrG.Desktop.Converters
{
    internal class ColorConverter : IValueConverter
    {
        private object ConvertColor(object value)
        {
            if (value is System.Drawing.Color color)
            {
                return Windows.UI.ColorHelper.FromArgb(color.A, color.R, color.G, color.B);
            }
            if (value is Windows.UI.Color color2)
            {
                return System.Drawing.Color.FromArgb(color2.A, color2.R, color2.G, color2.B);
            }
            return null;
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ConvertColor(value);
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ConvertColor(value);
        }
    }
}
