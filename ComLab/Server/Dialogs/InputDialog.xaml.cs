using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace ComLab.Dialogs
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : UserControl
    {
        public InputDialog()
        {
            InitializeComponent();
        }

        public async Task<string> Show()
        {
            var res = await DialogHost.Show(this, "DialogHost").ConfigureAwait(false);
            if (res == null) return null;
            if (res is bool b && b)
            {
                return Message.Text;
            }
            return null;
        }

        public static async Task<string> Show(string title, string positive, PackIconKind icon = PackIconKind.Alert)
        {
            return await Show(title, "", positive);
        }

        public static async Task<string> Show(string message, string positive, string negative = null, PackIconKind icon = PackIconKind.Alert)
        {
            var dlg = new InputDialog();
            dlg.Title.Text = message;
            dlg.Positive.Content = positive;
            dlg.Icon.Kind = icon;
            if (!string.IsNullOrEmpty(negative))
            {
                dlg.Negative.Content = negative;
            }
            else
            {
                dlg.Negative.Visibility = Visibility.Collapsed;
            }

            return await dlg.Show().ConfigureAwait(false);
        }
    }
}
