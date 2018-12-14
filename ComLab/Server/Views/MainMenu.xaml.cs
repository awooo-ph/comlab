using System.Windows;
using System.Windows.Controls;

namespace ComLab.Views
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.Register(
            "TitleVisibility", typeof(Visibility), typeof(MainMenu), new PropertyMetadata(Visibility.Visible));

        public Visibility TitleVisibility
        {
            get { return (Visibility) GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }
    }
}
