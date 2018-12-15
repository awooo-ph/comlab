using System;
using System.Linq;
using System.Threading;
using System.Windows;
using ComLab.ViewModels;

namespace ComLab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel.Instance;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Core.Context = SynchronizationContext.Current;
            
        }

    }
}
