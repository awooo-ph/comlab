using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComLab.Annotations;
using ComLab.Models;
using ComLab.ViewModels;
using MaterialDesignThemes.Wpf;

namespace ComLab.Dialogs
{
    /// <summary>
    /// Interaction logic for StudentSelector.xaml
    /// </summary>
    public partial class StudentSelector : INotifyPropertyChanged
    {
        private readonly Student _newStudent = new Student();
        private StudentSelector()
        {
            InitializeComponent();
            Student = _newStudent;
            DataContext = this;
        }

        private string _StudentId;

        public string StudentId
        {
            get => _StudentId;
            set
            {
                if (value == _StudentId) return;
                _StudentId = value;
                OnPropertyChanged(nameof(StudentId));
                Student = Students.Cache.FirstOrDefault(x => x.StudentId.ToLower() == value?.ToLower()) ?? _newStudent;
                _newStudent.StudentId = value;
                OnPropertyChanged(nameof(Student));
            }
        }

        public static async Task<Student> Show()
        {
            var dlg = new StudentSelector();
            var res = await DialogHost.Show(dlg, "DialogHost") as bool?;
            if (!(res ?? false)) return null;
            if (dlg.Student.IsNew) return null;
            return dlg.Student;
        }

        public Student Student { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
