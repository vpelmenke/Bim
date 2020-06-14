using System.Windows;

namespace Bim
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*public MainViewModel ViewModel
        {
            get => DataContext as MainViewModel;
            set => DataContext = value;
        }*/
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            //ViewModel = new MainViewModel();
        }

    }
}
