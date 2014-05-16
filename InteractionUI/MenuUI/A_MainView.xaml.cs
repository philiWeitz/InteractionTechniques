using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using GestureServices.Service.Interface;
using InteractionUI.BusinessLogic;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;
using KinectServices.Service.Interface;

namespace InteractionUI.MenuUI
{
    public partial class A_MainView : Page
    {
        private static readonly int INTERVAL = 20;
        private static readonly int SENSOR_IDX = 0;
        private static readonly int NO_USER_THRESHOLD_IN_MS = 500;

        private HighlightView highlightView;

        private bool kinectReady = false;
        private Thread kinectThread = null;
        private DispatcherTimer updateTimer;
        private DateTime? noUserDetectedTimer = null;

        private IConfigService confService;
        private ISensorService sensorService;
        private ISkeletonService skeletonService;
        private IGestureService gestureService;

        private KinectCameraControl cameraControl { get; set; }

        private KinectInteractionControl kinectControl { get; set; }

        public A_MainView()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            confService = SpringUtil.getService<IConfigService>();
            confService.ReadConfigFromFile();

            sensorService = SpringUtil.getService<ISensorService>();
            skeletonService = SpringUtil.getService<ISkeletonService>();
            gestureService = SpringUtil.getService<IGestureService>();

            updateTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL);
            updateTimer.Start();

            highlightView = new HighlightView(SENSOR_IDX);
            highlightView.Show();

            bubble_settingsControl.InitCloseAnimation(story_hide_settings_bubble_BeginStoryboard.Storyboard, this);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            symbol_nokinectControl.Visibility = Visibility.Hidden;
            symbol_nopersonControl.Visibility = Visibility.Hidden;

            if (sensorService.sensorAvailable(SENSOR_IDX))
            {
                if (kinectReady)
                {
                    cameraImage.Opacity = 1;
                    setKinectButtonVisibility(true);

                    kinectControl.checkGesture();
                    cameraControl.UpdateCamera();

                    if (!kinectControl.Enabled)
                    {
                        bubble_infobarControl.infotext.Text = "Press the \"Play\" button to start the interaction";
                        highlightView.WindowHighlight.Visibility = Visibility.Collapsed;
                    }
                    else if (skeletonService.userInRange().Count <= 0)
                    {
                        bubble_infobarControl.infotext.Text = "No user in range";
                        highlightView.UpdateWindow();

                        if (null == noUserDetectedTimer)
                        {
                            noUserDetectedTimer = DateTime.Now;
                        }

                        if (noUserDetectedTimer.Value.AddMilliseconds(NO_USER_THRESHOLD_IN_MS) < DateTime.Now)
                        {
                            cameraImage.Opacity = 0.5;
                            symbol_nopersonControl.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        noUserDetectedTimer = null;
                        bubble_infobarControl.infotext.Text = kinectControl.LastGesture;

                        highlightView.UpdateWindow();
                    }
                }
                else if (null == kinectThread || !kinectThread.IsAlive)
                {
                    bubble_infobarControl.infotext.Text = "Loading Kinect...";

                    kinectThread = new Thread(new ThreadStart(addKinectConnection));
                    kinectThread.IsBackground = true;
                    kinectThread.Start();
                }
            }
            else
            {
                bubble_infobarControl.infotext.Text = "No Kinect device detected!";

                kinectReady = false;
                cameraImage.Source = null;
                symbol_nokinectControl.Visibility = Visibility.Visible;

                setKinectButtonVisibility(false);
            }
        }

        private void setKinectButtonVisibility(bool visible)
        {
            button_cameraoffControl.IsEnabled = visible;
            button_cameraonControl.IsEnabled = visible;
            button_playControl.IsEnabled = visible;
            button_pauseControl.IsEnabled = visible;
        }

        private void addKinectConnection()
        {
            cameraControl = new KinectCameraControl(SENSOR_IDX);
            cameraControl.ScreenImage = cameraImage;

            kinectControl = new KinectInteractionControl(SENSOR_IDX);
            kinectControl.Enabled = false;

            sensorService.startSensor(SENSOR_IDX);
            skeletonService.enableSkeleton(sensorService.getSensor(SENSOR_IDX));
            gestureService.enableGestureService(sensorService.getSensor(SENSOR_IDX));

            kinectReady = true;
        }

        private void button_cameraoffControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cameraControl.Enabled = false;
        }

        private void button_cameraonControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cameraControl.Enabled = true;
        }

        private void button_playControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            kinectControl.Enabled = true;
        }

        private void button_pauseControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            kinectControl.Enabled = false;
            noUserDetectedTimer = DateTime.Now;
            kinectControl.LastGesture = String.Empty;

            symbol_nopersonControl.Visibility = Visibility.Hidden;
        }

        private void kinect_control_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ContentControl control = (ContentControl)sender;

            if (true == (bool)e.NewValue)
            {
                control.Opacity = 1;
            }
            else
            {
                control.Opacity = 0.5;
            }
        }
    }
}