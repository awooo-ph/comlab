using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ComLab.Dialogs;
using ComLab.Models;

namespace ComLab.ViewModels
{
    class Classes:ViewModelBase
    {
        private Classes() { }
        private static Classes _instance;
        public static Classes Instance => _instance ?? (_instance = new Classes());

        private static ObservableCollection<Class> _cache;

        public static ObservableCollection<Class> 
            Cache => _cache ?? (_cache = new ObservableCollection<Class>(
                         Class.GetAll()));

        private ListCollectionView _items;
        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = (ListCollectionView) CollectionViewSource.GetDefaultView(Cache);
                return _items;
            }
        }

        private ICommand _addClassCommand;
        public ICommand AddClassCommand => _addClassCommand ?? (_addClassCommand = new DelegateCommand(async d =>
        {
            var c = await AddClassDialog.Show();
            if (!(c?.IsValid??false)) return;
            c.InstructorId = MainViewModel.Instance.CurrentUser.Id;
            c.Save();
            Cache.Add(c);
        }));
    }
}
