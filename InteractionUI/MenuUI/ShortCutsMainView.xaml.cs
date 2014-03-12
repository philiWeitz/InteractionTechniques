using System.Windows.Controls;

namespace InteractionUI.MenuUI
{
    /// <summary>
    /// Interaction logic for ShortCutsMainView.xaml
    /// </summary>
    public partial class ShortCutsMainView : Page
    {
        private Page parent;

        public ShortCutsMainView(Page parent)
        {
            InitializeComponent();
            initialize(parent);
        }

        private void initialize(Page parent)
        {
            this.parent = parent;
        }

        private void toMainViewButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(parent);
        }
    }
}
