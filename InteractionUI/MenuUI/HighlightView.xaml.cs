using System.Windows;
using System.Windows.Media;
using GestureServices.Service.Interface;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;
using KinectServices.Service.Interface;

namespace InteractionUI.MenuUI
{
    /// <summary>
    /// Interaction logic for NotificationView.xaml
    /// </summary>
    public partial class HighlightView : Window
    {
        private readonly int SENSOR_IDX;

        private IConfigService confService;
        private ISensorService sensorService;
        private ISkeletonService skeletonService;
        private IGestureService gestureService;

        public HighlightView(int sensorIdx)
        {
            InitializeComponent();
            initialize();

            SENSOR_IDX = sensorIdx;
        }

        private void initialize()
        {
            Background = null;

            Left = SystemParameters.WorkArea.Left;
            Top = SystemParameters.WorkArea.Top;
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;

            confService = SpringUtil.getService<IConfigService>();
        }

        public void UpdateWindow()
        {
            if (confService.NoUserInRangeFeedbackEnabled && getSkeletonService().userInRange().Count <= 0)
            {
                WindowHighlight.Stroke = Brushes.Gold;
                WindowHighlight.Visibility = Visibility.Visible;
            }
            else if (confService.ActiveUserFeedbackEnabled && getGestureService().getActiveUserDataPointQueue().Count > 0)
            {
                WindowHighlight.Stroke = Brushes.Lime;
                WindowHighlight.Visibility = Visibility.Visible;
            }
            else
            {
                WindowHighlight.Visibility = Visibility.Collapsed;
            }
        }

        private ISensorService getSensorService()
        {
            if (null == sensorService)
            {
                sensorService = SpringUtil.getService<ISensorService>();
                sensorService.startSensor(SENSOR_IDX);
            }
            return sensorService;
        }

        private ISkeletonService getSkeletonService()
        {
            if (null == skeletonService)
            {
                skeletonService = SpringUtil.getService<ISkeletonService>();
                skeletonService.enableSkeleton(getSensorService().getSensor(SENSOR_IDX));
            }
            return skeletonService;
        }

        private IGestureService getGestureService()
        {
            if (null == gestureService)
            {
                gestureService = SpringUtil.getService<IGestureService>();
                gestureService.enableGestureService(getSensorService().getSensor(SENSOR_IDX));
            }
            return gestureService;
        }
    }
}