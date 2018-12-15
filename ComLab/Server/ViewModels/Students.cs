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
using MaterialDesignThemes.Wpf;

namespace ComLab.ViewModels
{
    class Students:ViewModelBase
    {
        private Students() { }

        private static Students _instance;
        public static Students Instance => _instance ?? (_instance = new Students());

        private ListCollectionView _items;
        private static ObservableCollection<Student> _cache;
        public static ObservableCollection<Student> Cache => _cache??(_cache =new ObservableCollection<Student>(Student.GetAll()?.Take(74)??new List<Student>()));
        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = (ListCollectionView) CollectionViewSource.GetDefaultView(Cache);
                _items.Filter = Filter;
                Classes.Instance.Items.CurrentChanged += (sender, args) =>
                {
                    _items.Refresh();
                };
                return _items;
            }
        }

        private bool Filter(object obj)
        {
            if (!(obj is Student s)) return false;
            if (!(Classes.Instance.Items.CurrentItem is Class c)) return true;
            return c.IsEnrolled(s.Id);
        }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand<Student>(async d =>
        {
            var res = await MessageDialog.Show("CONFIRM DELETE",
                $"Are you sure you want to delete {d.Fullname}?", "DELETE STUDENT", "CANCEL",
                PackIconKind.AccountRemove);
            if (!res) return;
            d.Delete();
            Cache.Remove(d);
            MainViewModel.Notify($"{d.Fullname} deleted!");
        }));

        
    }
}
