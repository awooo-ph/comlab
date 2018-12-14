using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLab.Converters
{
    class Inverter : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var b = (bool) value;
            return !b;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = value as bool?;
            return !b;
        }
    }
}
