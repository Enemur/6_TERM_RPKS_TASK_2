
using System.Windows;

using Lab2RPKS.ApplicationViewModel;

namespace Lab2RPKS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel_Encrypthion();
        }
    }
}
