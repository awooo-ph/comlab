using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ComLab.Models;

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
                return _items;
            }
        }
    }
}
