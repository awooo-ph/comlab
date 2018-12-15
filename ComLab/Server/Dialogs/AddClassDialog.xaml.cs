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
using ComLab.Models;
using MaterialDesignThemes.Wpf;

namespace ComLab.Dialogs
{
    /// <summary>
    /// Interaction logic for AddClassDialog.xaml
    /// </summary>
    public partial class AddClassDialog : UserControl
    {
        public AddClassDialog()
        {
            InitializeComponent();
            Class = new Class();
            DataContext = this;
        }

        public static async Task<Class> Show()
        {
            var dlg = new AddClassDialog();
            var res = await DialogHost.Show(dlg, "DialogHost") as bool?;
            if (!(res ?? false)) return null;
            return dlg.Class;
        }

        public Class Class { get; set; }
    }
}
