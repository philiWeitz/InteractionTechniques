using System.Windows;

namespace InteractionUI.MenuUI
{
    /// <summary>
    /// Interaction logic for NotificationView.xaml
    /// </summary>
    public partial class HighlightView : Window
    {
        public HighlightView()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            Background = null;

            Left = SystemParameters.WorkArea.Left;
            Top = SystemParameters.WorkArea.Top;
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
        }
    }
}