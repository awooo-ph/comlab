using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComLab
{
    static class ListBoxHelper
    {
        public static readonly DependencyProperty IsFirstItemProperty = DependencyProperty.RegisterAttached(
            "IsFirstItem", typeof(bool), typeof(ListBoxHelper), new PropertyMetadata(default(bool)));

        public static void SetIsFirstItem(DependencyObject element, bool value)
        {
            element.SetValue(IsFirstItemProperty, value);
        }

        public static bool GetIsFirstItem(DependencyObject element)
        {
            return (bool) element.GetValue(IsFirstItemProperty);
        }
    }
}
