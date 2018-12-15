using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ComLab.Annotations;
using ComLab.Models;
using ComLab.ViewModels;
using MaterialDesignThemes.Wpf;

namespace ComLab.Dialogs
{
    /// <summary>
    /// Interaction logic for AddStudentDialog.xaml
    /// </summary>
    public partial class AddStudentDialog:INotifyPropertyChanged
    {
        private AddStudentDialog()
        {
            InitializeComponent();
            Student = new Student();
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
                Student = Students.Cache.FirstOrDefault(x => x.StudentId.ToLower() == value.ToLower()) ??
                    new Student();
                OnPropertyChanged(nameof(Student));
            }
        }
        
        public static async Task<Student> Show()
        {
            var dlg = new AddStudentDialog();
            var res = await DialogHost.Show(dlg, "DialogHost") as bool?;
            if (!(res ?? false)) return null;
            if (dlg.Student.IsNew)
                dlg.Student.StudentId = dlg.StudentId;
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
