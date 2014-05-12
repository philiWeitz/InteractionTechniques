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

        private Dictionary<Control, DependencyProperty> itemPropertyMap =
            new Dictionary<Control, DependencyProperty>();

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
                checkBoxActive.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();

                foreach (KeyValuePair<Control, DependencyProperty> pair in itemPropertyMap)
                {
                    pair.Key.GetBindingExpression(pair.Value).UpdateSource();
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
                checkBoxActive.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateTarget();

                updateView((ShortcutDefinition)Tag);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ShortcutDefinition shortcutDef = (ShortcutDefinition)e.NewValue;
            updateView(shortcutDef);
        }

        private void Item_Initialized(object sender, EventArgs e)
        {
            if (sender.GetType().Equals(typeof(TextBox)))
            {
                itemPropertyMap.Add((Control)sender, TextBox.TextProperty);
            }
            else if (sender.GetType().Equals(typeof(Slider)))
            {
                itemPropertyMap.Add((Control)sender, Slider.ValueProperty);
            }
        }
    }
}