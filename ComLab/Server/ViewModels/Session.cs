using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ComLab.Dialogs;
using ComLab.Models;
using ComLab.Network;
using MaterialDesignThemes.Wpf;

namespace ComLab.ViewModels
{
    class Session:ViewModelBase
    {
        private Session() { }
        private static Session _instance;
        public static Session Instance => _instance ?? (_instance = new Session());

        private ListCollectionView _terminals;

        public ListCollectionView Terminals
        {
            get
            {
                if (_terminals != null) return _terminals;
                _terminals = new ListCollectionView(Server.Clients);
                _terminals.Filter = FilterTerminals;
                return _terminals;
            }
        }

        private bool FilterTerminals(object obj)
        {
            if (!(obj is Terminal t)) return false;
            return t.Enabled;
        }

        private ICommand _assignCommand;
        public ICommand AssignCommand => _assignCommand ?? (_assignCommand = new DelegateCommand<Terminal>(async d =>
        {
            var stud = await StudentSelector.Show();
            if (stud == null) return;
            foreach (var terminal in Server.Clients)
            {
                if (terminal.Student?.Id == stud.Id)
                {
                    var res = await MessageDialog.Show($"Transfer {stud.Fullname}?",
                        $"{stud.Fullname} is currently assigned to {terminal.Name}. " +
                        $"Do you want to transfer him or her to {d.Name}?","TRANSFER","CANCEL",PackIconKind.TransferWithinAStation);
                    if(!res) return;
                    terminal.Student = null;
                    await new LockClient().Send(terminal.IpEndPoint);
                }
            }
            d.Student = stud;
            var assign = new StudentInfo
            {
                Firstname = stud.Firstname,
                Lastname = stud.Lastname,
                Course = stud.Course
            };
            
            await assign.Send(d.LogonEndPoint);
        }));
    }
}
