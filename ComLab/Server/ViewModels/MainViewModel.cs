using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using ComLab.Models;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;

namespace ComLab.ViewModels
{
    class MainViewModel:ViewModelBase
    {
        private MainViewModel()
        {
            MenuItems = new List<INavigationItem>
            {
                new DividerNavigationItem(),
                new SubheaderNavigationItem {Subheader = "STUDENTS"},
                new FirstLevelNavigationItem
                {
                    Label = "MANAGE STUDENTS",
                    NavigationItemSelectedCallback = d =>
                    {
                        PageContent = Students.Instance;
                        return true;
                    },
                    IsSelected = true
                },
                new FirstLevelNavigationItem
                {
                    Label = "ADD NEW STUDENT"
                },
                new DividerNavigationItem(),
                new SubheaderNavigationItem {Subheader = "CLASSES", IsSelectable = false},
                new FirstLevelNavigationItem {Label = "VIEW ALL CLASSES"},
                new FirstLevelNavigationItem {Label = "CREATE NEW CLASS"}
            }; 
        }

        private INavigationItem _SelectedMenu;

        public INavigationItem SelectedMenu
        {
            get => _SelectedMenu;
            set
            {
                if (value == _SelectedMenu) return;              
                IsMenuOpen = false;
                _SelectedMenu = value;
                value?.NavigationItemSelectedCallback(value);
                OnPropertyChanged(nameof(SelectedMenu));
            }
        }

        private ViewModelBase _PageContent = Students.Instance;

        public ViewModelBase PageContent
        {
            get => _PageContent;
            set
            {
                if (value == _PageContent) return;
                _PageContent = value;
                OnPropertyChanged(nameof(PageContent));
            }
        }
        
        public List<INavigationItem> MenuItems { get; }
        
        private bool _IsMenuOpen;

        public bool IsMenuOpen
        {
            get => _IsMenuOpen;
            set
            {
                if (value == _IsMenuOpen) return;
                _IsMenuOpen = value;
                OnPropertyChanged(nameof(IsMenuOpen));
            }
        }

        private bool _PinMenu;

        public bool PinMenu
        {
            get => _PinMenu;
            set
            {
                if (value == _PinMenu) return;
                _PinMenu = value;
                OnPropertyChanged(nameof(PinMenu));
                if (value)
                {
                    IsMenuOpen = false;
                }
            }
        }
        
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
                _CurrentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

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

            CurrentUser = instructor;
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
