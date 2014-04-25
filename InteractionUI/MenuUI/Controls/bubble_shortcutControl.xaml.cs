using System.Windows.Controls;

namespace InteractionUI
{
    /// <summary>
    /// Interaction logic for bubble_shortcutControl.xaml
    /// </summary>
    public partial class bubble_shortcutControl : UserControl
    {
        //private ShortCutEditView editView;
        //private IShortcutReaderWriterService shortCutReaderWriter;

        public bubble_shortcutControl()
        {
            this.InitializeComponent();
            //initialize();
            //fillListView();
        }

        //private void initialize()
        //{
        //    shortCutReaderWriter = SpringUtil.getService<IShortcutReaderWriterService>();

        //    //editView = new ShortCutEditView();
        //    //editView.OnSaveEvent += new EventHandler(editView_OnSaveEvent);
        //}
        //private void fillListView()
        //{
        //    shortcutDefinitionList.Items.Clear();
        //    foreach (ShortcutDefinition item in shortCutReaderWriter.ReadDefinitionsFromDirectory())
        //    {
        //        shortcutDefinitionList.Items.Add(item);
        //    }
        //}
        //private void removeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    MessageBoxResult result = MessageBox.Show("Do you really want to delete this itme?",
        //        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

        //    if (result == MessageBoxResult.Yes)
        //    {
        //        ShortcutDefinition item = (ShortcutDefinition)((Button)sender).Tag;
        //        shortcutDefinitionList.Items.Remove(item);
        //        shortCutReaderWriter.RemoveShortcutDefinition(item);

        //        //int idx = 0;
        //        //foreach (ShortcutDefinition def in shortCutReaderWriter.GetShortCutList())
        //        //{
        //        //    if (idx >= item.Idx)
        //        //    {
        //        //        def.Idx = idx;
        //        //        shortCutReaderWriter.UpdateShortcutDefinition(item);
        //        //    }
        //        //    ++idx;
        //        //}

        //        //fillListView();
        //    }
        //}
        //private void checkBoxChanged(object sender, RoutedEventArgs e)
        //{
        //    ShortcutDefinition item = (ShortcutDefinition)((CheckBox)sender).Tag;
        //    shortCutReaderWriter.SaveOrUpdateShortcutDefinition(item);
        //}
        //private void editButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    //ShortcutDefinition item = (ShortcutDefinition)((Button)sender).Tag;
        //    //editView.EditDefinition(item);
        //    //NavigationService.Navigate(editView);
        //}
    }
}