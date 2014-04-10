using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using InteractionUI.BusinessLogic;

namespace InteractionUI.MenuUI
{
    public partial class A_MainView : Page
    {
        private static readonly int INTERVAL = 50;
        private static readonly int SENSOR_IDX = 0;

        private DispatcherTimer updateTimer;

        private KinectCameraControl cameraControl { get; set; }

        private KinectInteractionControl kinectControl { get; set; }

        public A_MainView()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            cameraControl = new KinectCameraControl(SENSOR_IDX);
            cameraControl.ScreenImage = cameraImage;

            kinectControl = new KinectInteractionControl(SENSOR_IDX);

            updateTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL);
            updateTimer.Start();
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            kinectControl.checkGesture();

            cameraControl.LastGesture = kinectControl.LastGesture;
            cameraControl.UpdateCamera();
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
            kinectControl.Enabled = true;
        }

        private void button_pauseControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            kinectControl.Enabled = false;
        }
    }
}