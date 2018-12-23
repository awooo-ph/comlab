using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComLab
{
    public static class Core
    {
        private static SynchronizationContext _synchronizationContext;

        public static SynchronizationContext Context
        {
            get { return _synchronizationContext; }
            set
            {
                if (_synchronizationContext != null || value == null) return;
                _synchronizationContext = value;
            }
        }

        public static void Post(Action action)
        {
            if (action == null) return;
            if (Context != null)
                Context.Post(d => action(), null);
            else
                action();
        }

    }
}
