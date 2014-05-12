using System.Windows;
using InteractionUI.MenuUI;

namespace InteractionUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            initialize();
        }

        public void initialize()
        {
            A_MainView view = new A_MainView();
            _mainFrame.Navigate(view);

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}