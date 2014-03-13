using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InteractionUI.BusinessLogic;
using KinectServices.Common;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;

namespace InteractionUI.MenuUI
{
    public class GestureShortcutPair
    {
        public String Shortcut { get; set; }
        public String GestureName { get; set; }
        public InteractionGesture Gesture { get; set; }
    }


    /// <summary>
    /// Interaction logic for ShortCutEditView.xaml
    /// </summary>
    public partial class ShortCutEditView : Page
    {
        private ShortcutDefinition item;
        private IShortcutReaderWriterService shortcutReaderWriter;
        public event EventHandler OnSaveEvent; 


        public ShortCutEditView()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            shortcutReaderWriter = SpringUtil.getService<IShortcutReaderWriterService>();
        }

        private void initializeList(ShortcutDefinition item)
        {
            shortcutList.Items.Clear();

            foreach (KeyValuePair<InteractionGesture, String> mapItem in item.GestureMap)
            {
                GestureShortcutPair pair = new GestureShortcutPair();

                pair.GestureName = mapItem.Key.ToString();
                pair.Gesture = mapItem.Key;
                pair.Shortcut = mapItem.Value;
                shortcutList.Items.Add(pair);
            }
        }

        public void EditDefinition(ShortcutDefinition item)
        {
            this.item = item;
            nameTextBox.Text = item.Name;
            processTextBox.Text = item.ProcessName;

            initializeList(item);
        }

        private void commit()
        {
            item.OldName = item.Name;
            item.Name = nameTextBox.Text;
            item.ProcessName = processTextBox.Text;

            foreach (Object obj in shortcutList.Items.SourceCollection)
            {
                GestureShortcutPair pair = (GestureShortcutPair)obj;
                item.GestureMap[pair.Gesture] = pair.Shortcut;
            }
        }

        private bool isValid()
        {
            return (nameTextBox.Text != String.Empty);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            item = null;
            this.NavigationService.GoBack();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (isValid())
            {
                commit();
                shortcutReaderWriter.UpdateShortcutDefinition(item);

                if (null != OnSaveEvent)
                {
                    OnSaveEvent.Invoke(this, null);
                }

                this.NavigationService.GoBack();
            }
        }
    }
}
