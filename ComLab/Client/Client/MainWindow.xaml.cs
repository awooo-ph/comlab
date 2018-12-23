using System;
using System.Threading;
using System.Windows;

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
            Core.Context = SynchronizationContext.Current;
        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Core.Context = SynchronizationContext.Current;
        }
    }
}
