using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ComLab.Network;

namespace ComLab.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public const long CONNECTING_INDEX = 2;
        public const long INSTRUCTOR_INDEX = 1;
        public const long LOGIN_INDEX = 0;
        public const long NO_CLASS_INDEX = 3;

        private MainViewModel()
        {
        }

        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());

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
            var res = await Client.Instance.Login(Username, d.Password);
            if (res == null) return;
            MessageBox.Show(res.Message);
        }));

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
