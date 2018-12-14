using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLab.ViewModels
{
    class Students:ViewModelBase
    {
        private Students() { }

        private static Students _instance;
        public static Students Instance => _instance ?? (_instance = new Students());
    }
}
