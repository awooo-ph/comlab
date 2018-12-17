using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLab.ViewModels
{
    class Terminals:ViewModelBase
    {
        private Terminals() { }

        private static Terminals _instance;
        public static Terminals Instance => _instance ?? (_instance = new Terminals());
    }
}
