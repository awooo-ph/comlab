using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ComLab.Network;

namespace ComLab
{
    public static class Core
    {
        private static SynchronizationContext _synchronizationContext;

        public static SynchronizationContext Context
        {
            get { return _synchronizationContext; }
            set
            {
                if (_synchronizationContext != null || value == null) return;
                _synchronizationContext = value;
            }
        }

        public static void Post(Action action)
        {
            if (action == null) return;
            if (Context != null)
                Context.Post(d => action(), null);
            else
                action();
        }

        public static async void Log(this Exception exception, string source = "", bool includeStackTrace = false)
        {
            if (exception == null) return;
            if (!string.IsNullOrEmpty(source))
                source = $" [{source}]";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    File.AppendAllText(Path.Combine(".", "Error.log"), $@"{DateTime.Now:g}       {source}: {exception?.Message}{Environment.NewLine}");
                    if (includeStackTrace)
                    {
                        File.AppendAllText(Path.Combine(".", "Error.log"), $@"{exception.StackTrace} {Environment.NewLine}");
                    }
                }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
                catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
                {
                    //
                }
            });
        }

        [STAThreadAttribute]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            //if (CLocker.IsLocked())
            //{
            App app = new App();
            app.InitializeComponent();
            app.Run();
            //}
            //else
            //{
            //    Context = SynchronizationContext.Current;
            //    SetupListeners();
            //    Client.Start();
            //    //CLocker.Lock();
            //    while (true) Thread.Sleep(1);
            //}

        }

        private static void SetupListeners()
        {
            Messenger.Default.AddListener(Messages.ShutdownClient,Shutdown);
            Messenger.Default.AddListener(Messages.RestartClient,Restart);
            Messenger.Default.AddListener(Messages.LockClient, CLocker.Lock);
            Messenger.Default.AddListener(Messages.ServerDiscovered, () => { });
        }

        private static void Restart()
        {
            var psi = new ProcessStartInfo("shutdown", "/r /f /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        private static void Shutdown()
        {
            var psi = new ProcessStartInfo("shutdown", "/s /f /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }
    }
}
