using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;

namespace InteractionUI
{
    /// <summary>
    /// Interaction logic for bubble_settingsControl.xaml
    /// </summary>
    public partial class bubble_settingsControl : UserControl
    {
        private IConfigService confService;

        private Dictionary<Control, DependencyProperty> itemPropertyMap =
            new Dictionary<Control, DependencyProperty>();

        private Page MainPage { get; set; }

        private Storyboard ExitStoryBoard { get; set; }

        public bubble_settingsControl()
        {
            this.InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            confService = SpringUtil.getService<IConfigService>();
            this.DataContext = confService;

            itemPropertyMap[gestureTimeOut] = Slider.ValueProperty;
            itemPropertyMap[volumeEnabled] = CheckBox.IsCheckedProperty;
            itemPropertyMap[activeUserFeedbackEnabled] = CheckBox.IsCheckedProperty;
            itemPropertyMap[noUserInRangeFeedbackEnabled] = CheckBox.IsCheckedProperty;
        }

        public void InitCloseAnimation(Storyboard exitStoryBoard, Page mainPage)
        {
            MainPage = mainPage;
            ExitStoryBoard = exitStoryBoard;
        }

        private void settingsCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            closeView();
        }

        private void settingsSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (KeyValuePair<Control, DependencyProperty> item in itemPropertyMap)
            {
                item.Key.GetBindingExpression(item.Value).UpdateSource();
            }

            confService.WriteConfigToFile();
            closeView();
        }

        private void UserControl_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            foreach (KeyValuePair<Control, DependencyProperty> item in itemPropertyMap)
            {
                item.Key.GetBindingExpression(item.Value).UpdateTarget();
            }
        }

        private void gestureTimeOut_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (null != gestureTimeOutText)
            {
                gestureTimeOutText.Text = Math.Round(e.NewValue, 1).ToString();

                if (gestureTimeOutText.Text.Equals("1"))
                {
                    gestureTimeOutText.Text += " Second";
                }
                else
                {
                    gestureTimeOutText.Text += " Seconds";
                }
            }
        }

        private void closeView()
        {
            if (null != MainPage && null != ExitStoryBoard)
            {
                MainPage.BeginStoryboard(ExitStoryBoard);
            }
        }
    }
}