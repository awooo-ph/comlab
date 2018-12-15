using System.Threading.Tasks;
using ComLab.Models;
using MaterialDesignThemes.Wpf;

namespace ComLab.Dialogs
{
    /// <summary>
    /// Interaction logic for AddStudentDialog.xaml
    /// </summary>
    public partial class AddStudentDialog
    {
        private AddStudentDialog()
        {
            InitializeComponent();
            Student = new Student();
            DataContext = this;
        }

        public static async Task<Student> Show()
        {
            var dlg = new AddStudentDialog();
            var res = await DialogHost.Show(dlg, "DialogHost") as bool?;
            if (!(res ?? false)) return null;
            return dlg.Student;
        }

        public Student Student { get; set; }

    }
}
