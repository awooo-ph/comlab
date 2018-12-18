using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ComLab.Network;

namespace ComLab.ViewModels
{
    class MainViewModel:ViewModelBase
    {
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
        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand<PasswordBox>(async d =>
                                            {
                                                var res = await Client.Instance.Login(Username, d.Password);
                                                if (res == null) return;
                                                MessageBox.Show(res.Message);
                                            }));

        private long _PageIndex;

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


    }
}
