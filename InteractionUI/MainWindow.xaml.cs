using System.Windows;
using InteractionUI.BusinessLogic;

namespace InteractionUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectInteraction interaction;


        public MainWindow()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            interaction = new KinectInteraction();
            interaction.ScreenImage = image1;
            interaction.Start();
        }
    }
}
