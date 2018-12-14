using System;
using System.Windows;

namespace ComLab.Converters
{
    class EqualityConverter : ConverterBase
    {

        public enum ReturnTypes
        {
            Visibility, Boolean, Long, Object
        }

        public enum Operations
        {
            Equals,
            GreaterThan,
            LessThan,
            NotEquals,
        }

        private ReturnTypes _returnType = ReturnTypes.Visibility;

        public ReturnTypes ReturnType
        {
            get => _returnType;
            set
            {
                _returnType = value;
                if (value == ReturnTypes.Boolean)
                {
                    TrueValue = true;
                    FalseValue = false;
                }
            }
        }

        public object TrueValue { get; set; } = Visibility.Visible;

        public object FalseValue { get; set; } = Visibility.Collapsed;

        public Operations Operation { get; set; } = Operations.Equals;
        public object Operand { get; set; } = 0;

        public EqualityConverter(object whenTrue, object whenFalse)
        {
            TrueValue = whenTrue;
            FalseValue = whenFalse;
        }

        public EqualityConverter()
        {

        }

        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (value == null)
                return FalseValue;

            if (parameter != null)
                return value.Equals(parameter) ? TrueValue : FalseValue;

            switch (Operation)
            {
                case Operations.GreaterThan:
                    return (double)value >= System.Convert.ToDouble(Operand) ? TrueValue : FalseValue;
                case Operations.LessThan:
                    return (double)value < System.Convert.ToDouble(Operand) ? TrueValue : FalseValue;
                case Operations.NotEquals when value is double d:
                    return d == System.Convert.ToDouble(Operand) ? FalseValue : TrueValue;
                case Operations.NotEquals:
                    return value.Equals(Operand) ? FalseValue : TrueValue;
            }

            if ((value is double dd))
                return dd == System.Convert.ToDouble(Operand) ? TrueValue : FalseValue;
            return value.Equals(Operand) ? TrueValue : FalseValue;
        }
    }
}
