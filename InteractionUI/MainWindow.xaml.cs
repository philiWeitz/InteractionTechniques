using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using InteractionUI.MenuUI;

namespace InteractionUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly String BACKGROUND_IMG = "Content/Images/BackgroundMainWindow.png";

        public MainWindow()
        {
            InitializeComponent();
            initialize();
        }

        public void initialize()
        {
            MainView view = new MainView();
            _mainFrame.Navigate(view);

            Background = new ImageBrush(new BitmapImage(
                new Uri(BaseUriHelper.GetBaseUri(this), BACKGROUND_IMG)));
        }
    }
}
