using System;

namespace ComLab.Converters
{
    class IsNullConverter : ConverterBase
    {
        public bool Invert { get; set; }
        
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if(Invert) return value != null;
            return value == null;
        }
    }
}
