using System;
using System.Windows;
using System.Windows.Controls;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;

namespace InteractionUI.MenuUI
{
    /// <summary>
    /// Interaction logic for ShortCutsMainView.xaml
    /// </summary>
    public partial class ShortCutsMainView : Page
    {
        private Page parent;
        private ShortCutEditView editView;
        private IShortcutReaderWriterService shortCutReaderWriter;


        public ShortCutsMainView(Page parent)
        {
            InitializeComponent();
            initialize(parent);
            fillListView();
        }

        private void initialize(Page parent)
        {
            this.parent = parent;
            shortCutReaderWriter = SpringUtil.getService<IShortcutReaderWriterService>();
            
            editView = new ShortCutEditView();
            editView.OnSaveEvent += new EventHandler(editView_OnSaveEvent);
        }

        private void fillListView()
        {
            shortcutDefinitionList.Items.Clear();
            foreach (ShortcutDefinition item in shortCutReaderWriter.ReadDefinitionsFromDirectory())
            {
                shortcutDefinitionList.Items.Add(item);
            }
        }

        private void toMainViewButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            shortCutReaderWriter.ReadDefinitionsFromDirectory();
            NavigationService.Navigate(parent);
        }

        private void editButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ShortcutDefinition item = (ShortcutDefinition) ((Button)sender).Tag;
            editView.EditDefinition(item);
            NavigationService.Navigate(editView);
        }

        private void removeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to delete this itme?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                ShortcutDefinition item = (ShortcutDefinition)((Button)sender).Tag;
                shortcutDefinitionList.Items.Remove(item);
                shortCutReaderWriter.RemoveShortcutDefinition(item);   
            }
        }

        private void addDefinitionButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            editView.EditDefinition(new ShortcutDefinition());
            NavigationService.Navigate(editView);
        }

        private void checkBoxChanged(object sender, RoutedEventArgs e)
        {
            ShortcutDefinition item = (ShortcutDefinition)((CheckBox)sender).Tag;
            shortCutReaderWriter.UpdateShortcutDefinition(item);
        }

        private void editView_OnSaveEvent(object sender, EventArgs e)
        {
            fillListView();
        }  
    }
}
