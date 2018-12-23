using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ComLab.Network;
using MaterialDesignThemes.Wpf;

namespace ComLab.ViewModels
{
    class LogonViewModel : ViewModelBase
    {
        private const long CONNECTING_INDEX = 2;
        private const long INSTRUCTOR_INDEX = 1;
        private const long LOGIN_INDEX = 0;
        private const long NO_CLASS_INDEX = 3;

        private LogonViewModel()
        {
            Messenger.Default.AddListener<InstructorLogin>(Messages.InstructorLogin, login =>
            {
                Instructor = login.Fullname;
                PageIndex = NO_CLASS_INDEX;
            });
            Messenger.Default.AddListener(Messages.ServerDiscovered, () =>
            {
                if (PageIndex == CONNECTING_INDEX)
                    PageIndex = INSTRUCTOR_INDEX;
            });
            Messenger.Default.AddListener<ClientInfo>(Messages.ClientInfoReceived, info =>
            {
                ComputerName = info.ComputerName;
                if (PageIndex == CONNECTING_INDEX)
                    PageIndex = INSTRUCTOR_INDEX;
            });
            Messenger.Default.AddListener<ClassInfo>(Messages.ClassInfo, UpdateClassInfo);
        }

        private static LogonViewModel _instance;
        public static LogonViewModel Instance => _instance ?? (_instance = new LogonViewModel());

        private string _ComputerName = Environment.MachineName;

        public string ComputerName
        {
            get => _ComputerName;
            set
            {
                if (value == _ComputerName) return;
                _ComputerName = value;
                OnPropertyChanged(nameof(ComputerName));
            }
        }

        private string _WelcomeMessage;

        public string WelcomeMessage
        {
            get => _WelcomeMessage;
            set
            {
                if (value == _WelcomeMessage) return;
                _WelcomeMessage = value;
                OnPropertyChanged(nameof(WelcomeMessage));
            }
        }



        private string _ClassTitle;

        public string ClassTitle
        {
            get => _ClassTitle;
            set
            {
                if (value == _ClassTitle) return;
                _ClassTitle = value;
                OnPropertyChanged(nameof(ClassTitle));
            }
        }

        private string _Schedule;

        public string Schedule
        {
            get => _Schedule;
            set
            {
                if (value == _Schedule) return;
                _Schedule = value;
                OnPropertyChanged(nameof(Schedule));
            }
        }

        private string _Instructor;

        public string Instructor
        {
            get => _Instructor?.ToUpper();
            set
            {
                if (value == _Instructor) return;
                _Instructor = value;
                OnPropertyChanged(nameof(Instructor));
            }
        }



        private string _Username;

        public string Username
        {
            get => _Username;
            set
            {
                if (value == _Username) return;
                _Username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private ICommand _loginCommand;

        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand<PasswordBox>(async d =>
        {
            IsLoggingIn = true;
            var res = await Client.Instance.Login(Username, d.Password);
            if (res?.Success??false)
            {
               Application.Current.Shutdown();
            }
            else
            {
                MessageQueue.Enqueue(res?.Message??"REQUEST TIMEOUT");
            }

            IsLoggingIn = false;
        }));

        private bool _IsLoggingIn;

        public bool IsLoggingIn
        {
            get => _IsLoggingIn;
            set
            {
                if (value == _IsLoggingIn) return;
                _IsLoggingIn = value;
                OnPropertyChanged(nameof(IsLoggingIn));
            }
        }
        
        private SnackbarMessageQueue _messageQueue;
        public SnackbarMessageQueue MessageQueue => _messageQueue ?? (_messageQueue = new SnackbarMessageQueue());

        private long _PageIndex = CONNECTING_INDEX;

        public long PageIndex
        {
            get => _PageIndex;
            set
            {
                if (value == _PageIndex) return;
                _PageIndex = value;
                OnPropertyChanged(nameof(PageIndex));
            }
        }

        public void UpdateInfo(ClientInfo info)
        {
            ComputerName = info.ComputerName;
        }

    public void UpdateClassInfo(ClassInfo info)
        {
            if (!info.HasInstructor)
                PageIndex = INSTRUCTOR_INDEX;
            else if (!info.ClassStarted)
                PageIndex = NO_CLASS_INDEX;
            else
            {
                ClassTitle = info.ClassName;
                Schedule = info.Schedule;
                Instructor = info.Instructor;
                PageIndex = LOGIN_INDEX;
            }

            WelcomeMessage = info.WelcomMessage;
        }
    }
}
