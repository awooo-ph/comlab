using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ComLab.Models;
using ComLab.Network;

namespace ComLab.ViewModels
{

    class Terminals:ViewModelBase
    {
      

        private Terminals()
        {
           
        }

        private static Terminals _instance;
        public static Terminals Instance => _instance ?? (_instance = new Terminals());

        private ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = (ListCollectionView) CollectionViewSource.GetDefaultView(Server.Clients);
                return _items;
            }
        }
        
    }
}
