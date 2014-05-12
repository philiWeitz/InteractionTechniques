using System;
using System.Windows.Controls;
using System.Windows.Data;
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

            Binding volumeBinding = new Binding("VolumeEnabled");
            volumeBinding.Source = confService;
            volumeBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            volumeEnabled.SetBinding(CheckBox.IsCheckedProperty, volumeBinding);

            Binding gestureTimeOutBinding = new Binding("GestureTimeOut");
            gestureTimeOutBinding.Source = confService;
            gestureTimeOutBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            gestureTimeOut.SetBinding(Slider.ValueProperty, gestureTimeOutBinding);

            Binding activeUserFeedbackEnabledBinding = new Binding("ActiveUserFeedbackEnabled");
            activeUserFeedbackEnabledBinding.Source = confService;
            activeUserFeedbackEnabledBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            activeUserFeedbackEnabled.SetBinding(CheckBox.IsCheckedProperty, activeUserFeedbackEnabledBinding);
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
            BindingExpression be = volumeEnabled.GetBindingExpression(CheckBox.IsCheckedProperty);
            be.UpdateSource();

            BindingExpression bf = gestureTimeOut.GetBindingExpression(Slider.ValueProperty);
            bf.UpdateSource();

            BindingExpression bg = activeUserFeedbackEnabled.GetBindingExpression(CheckBox.IsCheckedProperty);
            bg.UpdateSource();

            confService.WriteConfigToFile();
            closeView();
        }

        private void UserControl_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            //selbe Anweisung wie cancel button: in separate Methode auslagern und von hier und cancel aufrufen.

            if ((bool)e.NewValue == true)
            {
                BindingExpression be = volumeEnabled.GetBindingExpression(CheckBox.IsCheckedProperty);
                be.UpdateTarget();

                BindingExpression bf = gestureTimeOut.GetBindingExpression(Slider.ValueProperty);
                bf.UpdateTarget();

                BindingExpression bg = activeUserFeedbackEnabled.GetBindingExpression(CheckBox.IsCheckedProperty);
                bg.UpdateTarget();
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