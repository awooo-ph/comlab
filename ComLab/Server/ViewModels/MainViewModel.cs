using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using ComLab.Dialogs;
using ComLab.Models;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;

namespace ComLab.ViewModels
{
    class MainViewModel:ViewModelBase
    {
        

        private MainViewModel()
        {
            var ClassManager = new MenuItem
            {
                Title = "CLASS MANAGER",
                Command = new DelegateCommand(d => { PageContent = Students.Instance; }),
                IsSelected = true
            };

            MenuItems = new List<MenuItem>
            {
                ClassManager,
                new MenuItem
                {
                    Title = "ENROLL STUDENT",
                    Command = new DelegateCommand(d=>
                    {
                        if (Classes.Instance.Items.Count == 0)
                        {
                            Classes.Instance.AddClassCommand.Execute(null);
                            Classes.Instance.Items.MoveCurrentToFirst();
                        }

                        if (Classes.Instance.Items.CurrentItem == null) return;

                        AddNewStudent();
                        ClassManager.IsSelected = true;
                    }),
                    IsSelectable = false
                },
                new MenuItem
                {
                    Title = "CREATE NEW CLASS",
                    Command = new DelegateCommand(d=>
                        {
                            Classes.Instance.AddClassCommand.Execute(null);
                            ClassManager.IsSelected = true;
                        }),
                    IsSelectable = false
                },
                new MenuItem
                {
                    Title = "START CLASS",
                    Command = new DelegateCommand(d=>
                    {
                        StartClass();
                        ClassManager.IsSelected = true;
                    }),
                    IsSelectable = false
                },

                new MenuItem {Title = "ADMINISTRATION", IsHeader = true},
                new MenuItem
                {
                    Title = "INSTRUCTORS",
                    Command = new DelegateCommand(d =>
                    {
                        PageContent = Classes.Instance;
                    })
                },
                new MenuItem
                {
                    Title = "TERMINALS"
                },
                
            }; 
        }

        private void StartClass()
        {
            
        }

        private async void AddNewStudent()
        {
            var student = await AddStudentDialog.Show();
            if (student == null) return;
            student.Save();
            Students.Cache.Add(student);

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
                if (value != null)
                {
                    value.IsSelected = (bool)(value?.NavigationItemSelectedCallback(value));
                }
                 
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

        private ViewModelBase _RightDrawer;

        public ViewModelBase RightDrawer
        {
            get => _RightDrawer;
            set
            {
                if (value == _RightDrawer) return;
                _RightDrawer = value;
                OnPropertyChanged(nameof(RightDrawer));
            }
        }

        private bool _IsRightDrawerOpen;

        public bool IsRightDrawerOpen
        {
            get => _IsRightDrawerOpen;
            set
            {
                if (value == _IsRightDrawerOpen) return;
                _IsRightDrawerOpen = value;
                OnPropertyChanged(nameof(IsRightDrawerOpen));
            }
        }
        
        public List<MenuItem> MenuItems { get; }
        
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

        private bool _PinMenu = true;

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

            if (Classes.Instance.Items.Count == 0)
                Classes.Instance.AddClassCommand.Execute(null);
        }));

        public static void Notify(string message)
        {
            MessageQueue.Enqueue(message);
        }

        private static SnackbarMessageQueue _messageQueue;
        public static SnackbarMessageQueue MessageQueue => _messageQueue ?? (_messageQueue = new SnackbarMessageQueue());
    }
}
