using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ComLab.Models;
using MaterialDesignThemes.Wpf;

namespace ComLab.ViewModels
{
    class MainViewModel:ViewModelBase
    {
        private bool _IsAuthenticated;

        public bool IsAuthenticated
        {
            get => _IsAuthenticated;
            set
            {
                if (value == _IsAuthenticated) return;
                _IsAuthenticated = value;
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }

        private Instructor _CurrentUser;
        public Instructor CurrentUser
        {
            get => _CurrentUser;
            set
            {
                if (value == _CurrentUser) return;
                _CurrentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        private MainViewModel() { }
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());

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
        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand<PasswordBox>(pwd =>
        {
            IsAuthenticated = false;
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(pwd.Password))
            {
                Notify("Authentication failed!");
                return;
            }

            Instructor instructor;
            if (Instructor.GetRows() == 0)
            {
                instructor = new Instructor()
                {
                    Username = Username,
                    Password = pwd.Password,
                    IsAdmin = true,
                    Firstname = "Administrator"
                };
                instructor.Save();
            }
            else
            {
                instructor = Instructor.GetByUsername(Username);
            }

            if (instructor?.Password != pwd.Password)
            {
                Notify("Authentication failed!");
                return;
            }

            IsAuthenticated = true;
        }));

        public void Notify(string message)
        {
            MessageQueue.Enqueue(message);
        }

        private SnackbarMessageQueue _messageQueue;
        public SnackbarMessageQueue MessageQueue => _messageQueue ?? (_messageQueue = new SnackbarMessageQueue());
    }
}
