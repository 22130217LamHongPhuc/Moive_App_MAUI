using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPhimLo.Models
{
    public class BoolToHeartFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool b && b) ? "heart_fill.png" : "heart_outline.png";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

}
