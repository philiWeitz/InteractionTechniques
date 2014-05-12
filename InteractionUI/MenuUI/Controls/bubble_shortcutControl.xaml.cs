using System.Windows;
using System.Windows.Controls;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;

namespace InteractionUI
{
    /// <summary>
    /// Interaction logic for bubble_shortcutControl.xaml
    /// </summary>
    public partial class bubble_shortcutControl : UserControl
    {
        private IShortcutReaderWriterService shortcutService;

        public bubble_shortcutControl()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            shortcutService = SpringUtil.getService<IShortcutReaderWriterService>();

            foreach (ShortcutDefinition item in shortcutService.ReadDefinitionsFromDirectory())
            {
                tabcontrol.Items.Add(item);
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (null != button.Tag)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to delete this shortcut definition?",
                    "Delete Shortcut Definition", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    tabcontrol.Items.Remove(button.Tag);
                    shortcutService.RemoveShortcutDefinition((ShortcutDefinition)button.Tag);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ShortcutDefinition item = new ShortcutDefinition();
            item.Name = "New Item";
            item.Idx = tabcontrol.Items.Count;

            shortcutService.SaveOrUpdateShortcutDefinition(item);
            tabcontrol.Items.Add(item);
            tabcontrol.SelectedItem = item;
        }
    }
}