using System.Threading;
using System.Windows;

namespace ComLab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Core.IsRunning = true;
            Core.Context = SynchronizationContext.Current;
        }
    }
}
