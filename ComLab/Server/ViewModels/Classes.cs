using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLab.ViewModels
{
    class Classes:ViewModelBase
    {
        private Classes() { }
        private static Classes _instance;
        public static Classes Instance => _instance ?? (_instance = new Classes());


    }
}
