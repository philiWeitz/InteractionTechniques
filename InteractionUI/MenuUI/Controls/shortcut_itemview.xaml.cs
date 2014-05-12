using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;

namespace InteractionUI.MenuUI.Controls
{
    /// <summary>
    /// Interaction logic for ItemView.xaml
    /// </summary>
    public partial class Shortcut_itemview : UserControl
    {
        private IShortcutReaderWriterService shortcutService;
        private List<TextBox> textBoxList = new List<TextBox>();

        public Shortcut_itemview()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            shortcutService = SpringUtil.getService<IShortcutReaderWriterService>();
        }

        private void updateView(ShortcutDefinition shortcutDef)
        {
            shortcutList.Items.Clear();

            if (null != shortcutDef)
            {
                foreach (KeyValuePair<InteractionGesture, ShortcutItem> item in shortcutDef.GestureMap)
                {
                    if (item.Key != InteractionGesture.None && item.Key != InteractionGesture.Wave)
                    {
                        shortcutList.Items.Add(item.Value);
                    }
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != Tag && Tag.GetType().Equals(typeof(ShortcutDefinition)))
            {
                textBoxName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                textBoxProcess.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                foreach (TextBox textBox in textBoxList)
                {
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }

                shortcutService.SaveOrUpdateShortcutDefinition((ShortcutDefinition)Tag);
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != Tag && Tag.GetType().Equals(typeof(ShortcutDefinition)))
            {
                textBoxName.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                textBoxProcess.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

                updateView((ShortcutDefinition)Tag);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ShortcutDefinition shortcutDef = (ShortcutDefinition)e.NewValue;
            updateView(shortcutDef);
        }

        private void TextBox_Initialized(object sender, EventArgs e)
        {
            textBoxList.Add((TextBox)sender);
        }
    }
}