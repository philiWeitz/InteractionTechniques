using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using InteractionUI.BusinessLogic;
using InteractionUtil.Util;
using KinectServices.Service.Interface;

namespace InteractionUI.MenuUI
{
    public partial class A_MainView : Page
    {
        private static readonly int INTERVAL = 50;
        private static readonly int SENSOR_IDX = 0;

        private bool kinectReady = false;
        private Thread kinectThread = null;
        private DispatcherTimer updateTimer;
        private ISensorService sensorService;
        private ISkeletonService skeletonService;

        private KinectCameraControl cameraControl { get; set; }

        private KinectInteractionControl kinectControl { get; set; }

        public A_MainView()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            sensorService = SpringUtil.getService<ISensorService>();
            skeletonService = SpringUtil.getService<ISkeletonService>();

            updateTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL);
            updateTimer.Start();
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            symbol_nokinectControl.Visibility = Visibility.Hidden;
            symbol_nopersonControl.Visibility = Visibility.Hidden;

            if (sensorService.sensorAvailable(SENSOR_IDX))
            {
                if (kinectReady)
                {
                    kinectControl.checkGesture();

                    cameraControl.UpdateCamera();
                    cameraControl.CheckActiveUserChange(symbol_hand_animationControl);

                    if (kinectControl.Enabled && skeletonService.userInRange().Count <= 0)
                    {
                        symbol_nopersonControl.Visibility = Visibility.Visible;
                    }

                    bubble_infobarControl.infotext.Text = kinectControl.LastGesture;
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
                kinectReady = false;
                cameraImage.Source = null;
                symbol_nokinectControl.Visibility = Visibility.Visible;
            }
        }

        private void addKinectConnection()
        {
            cameraControl = new KinectCameraControl(SENSOR_IDX);
            cameraControl.ScreenImage = cameraImage;
            cameraControl.Enabled = true;

            kinectControl = new KinectInteractionControl(SENSOR_IDX);
            kinectControl.Enabled = false;

            sensorService.startSensor(SENSOR_IDX);
            skeletonService.enableSkeleton(sensorService.getSensor(SENSOR_IDX));

            kinectReady = true;
        }

        private void button_cameraoffControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cameraControl.Enabled = true;
        }

        private void button_cameraonControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cameraControl.Enabled = false;
        }

        private void button_playControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            kinectControl.Enabled = false;
            kinectControl.LastGesture = String.Empty;

            symbol_nopersonControl.Visibility = Visibility.Hidden;
        }

        private void button_pauseControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            kinectControl.Enabled = true;
        }
    }
}