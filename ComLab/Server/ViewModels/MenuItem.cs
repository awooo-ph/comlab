using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ComLab.ViewModels
{
    class MenuItem:ViewModelBase
    {
        private string _Title;

        public string Title
        {
            get => _Title;
            set
            {
                if (value == _Title) return;
                _Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private ICommand _Command;

        public ICommand Command
        {
            get => _Command;
            set
            {
                if (value == _Command) return;
                _Command = value;
                OnPropertyChanged(nameof(Command));
            }
        }

        private bool _IsSelected;

        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (value == _IsSelected) return;
                _IsSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        private bool _IsSelectable = true;

        public bool IsSelectable
        {
            get => _IsSelectable;
            set
            {
                if (value == _IsSelectable) return;
                _IsSelectable = value;
                OnPropertyChanged(nameof(IsSelectable));
            }
        }

        private bool _IsHeader;

        public bool IsHeader
        {
            get => _IsHeader;
            set
            {
                if (value == _IsHeader) return;
                _IsHeader = value;
                OnPropertyChanged(nameof(IsHeader));
            }
        }
        
    }
}
