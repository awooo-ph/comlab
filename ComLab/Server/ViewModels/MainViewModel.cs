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
using ComLab.Network;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;

namespace ComLab.ViewModels
{
    class MainViewModel:ViewModelBase
    {
        private MenuItem ClassManager => new MenuItem
        {
            Title = "CLASS MANAGER",
            Command = new DelegateCommand(d => { PageContent = Students.Instance; }),
            IsSelected = true
        };

        private MenuItem StartClassMenu => new MenuItem
        {
            Title = "START CLASS",
            Command = new DelegateCommand(d =>
            {
                Core.Post(()=>{
                    StartClass();
                    ClassManager.IsSelected = true;
                });
            }),
            IsSelectable = false
        };

        private MainViewModel()
        {
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
                StartClassMenu,
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
                    Title = "TERMINALS",
                    IsSelectable = false,
                    Command = new DelegateCommand(d =>
                    {
                        RightDrawer = Terminals.Instance;
                        IsRightDrawerOpen = true;
                    })
                },
                
            }; 
        }

        private ClassSession _ClassSession;

        public ClassSession ClassSession
        {
            get => _ClassSession;
            set
            {
                if (value == _ClassSession) return;
                _ClassSession = value;
                OnPropertyChanged(nameof(ClassSession));
                StartClassMenu.IsEnabled = value == null;
                OnPropertyChanged(nameof(ClassStarted));
            }
        }

        public bool ClassStarted => ClassSession != null;
        
        private async void StartClass()
        {
            var c = ((Class)Classes.Instance.Items.CurrentItem);
            if (ClassSession != null || c==null) return;
   
             var   message = await StartClassDialog.Show(c.Title);
   
            if (message == null) return;

            ClassSession = new ClassSession()
            {
                ClassId = c.Id,
                Started = DateTime.Now,
                Message = message
            };
            ClassSession.Save();

            var classInfo = new ClassInfo
            {
                ClassName = c.Title,
                Schedule = c.Schedule,
                Instructor = CurrentUser.Fullname,
                WelcomMessage = message,
                ClassStarted = true,
                HasInstructor = true,
            };

            Server.Broadcast(classInfo);

        }

        private async void AddNewStudent()
        {
            var student = await AddStudentDialog.Show();
            if (student == null) return;
            student.Save();
            student.Enroll(CurrentUser.Id, Classes.Instance.Items.CurrentItem as Class);
            if(!Students.Cache.Contains(student)) Students.Cache.Add(student);
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

            Server.Broadcast(new InstructorLogin
            {
                Fullname = instructor?.Fullname
            });

            Classes.Instance.Refresh();
            if (Classes.Instance.Items.Count == 0)
                Classes.Instance.AddClassCommand.Execute(null);
        }));

        public static void Notify(string message)
        {
            MessageQueue.Enqueue(message);
        }

        private static SnackbarMessageQueue _messageQueue;
        public static SnackbarMessageQueue MessageQueue => _messageQueue ?? (_messageQueue = new SnackbarMessageQueue());

        public static ClassInfo GetCurrentClass()
        {
            return new ClassInfo
            {
                HasInstructor = Instance.CurrentUser!=null,
                Instructor = Instance.CurrentUser?.Fullname,
                ClassStarted = Instance.ClassSession!=null,
                ClassName = Instance.ClassSession?.Class?.Title,
                Schedule = Instance.ClassSession?.Class?.Schedule,
                WelcomMessage = Instance.ClassSession?.Message
            };
        }
    }
}
