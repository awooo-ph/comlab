using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace ComLab.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog
    {
        public MessageDialog()
        {
            InitializeComponent();
        }

        public async Task<bool> Show()
        {
            var res = await DialogHost.Show(this, "DialogHost").ConfigureAwait(false);
            if (res == null) return false;
            return res is bool b && b;
        }

        public static async Task<bool> Show(string title, string positive, PackIconKind icon = PackIconKind.Alert)
        {
            return await Show(title, "", positive);
        }

        public static async Task<bool> Show(string title, string message, string positive, string negative=null, PackIconKind icon = PackIconKind.Alert)
        {
            var dlg = new MessageDialog();
            dlg.Title.Text = title;
            dlg.Message.Text = message;
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
