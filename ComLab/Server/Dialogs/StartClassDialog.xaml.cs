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
    /// Interaction logic for StartClassDialog.xaml
    /// </summary>
    public partial class StartClassDialog : UserControl
    {
        public StartClassDialog()
        {
            InitializeComponent();
        }

        public async Task<string> Show()
        {
            var res = await DialogHost.Show(this, "DialogHost");
            if (res == null) return null;
            if (res is bool b && b)
            {
                return Message.Text;
            }
            return null;
        }
        
        public static async Task<string> Show(string className)
        {
            var dlg = new StartClassDialog();
            Core.Post(()=>{
                dlg.Title.Text = $"Begin {className} class session";
            });
            return await dlg.Show();
        }
    }
}
