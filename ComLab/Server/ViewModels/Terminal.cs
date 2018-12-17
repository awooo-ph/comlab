using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLab.ViewModels
{
    class Terminal:ViewModelBase
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set
            {
                if (value == _Name) return;
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _IP;

        public string IP
        {
            get => _IP;
            set
            {
                if (value == _IP) return;
                _IP = value;
                OnPropertyChanged(nameof(IP));
            }
        }

        private Students _User;

        public Students User
        {
            get => _User;
            set
            {
                if (value == _User) return;
                _User = value;
                OnPropertyChanged(nameof(User));
            }
        }


    }
}
