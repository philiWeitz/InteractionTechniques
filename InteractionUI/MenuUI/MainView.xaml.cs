using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using InteractionUI.BusinessLogic;
using InteractionUtil.Util;
using KinectServices.Service.Interface;

namespace InteractionUI.MenuUI
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        private static readonly int SENSOR_CHECK_INTERVAL = 1000;
        private static readonly int SENSOR_IDX = 0;

        private ShortCutsMainView shortCutsMainView;
        private DispatcherTimer sensorCheckTimer;
        private KinectInteractionControl interaction;
        private CameraWindow cameraView;

        public MainView()
        {
            InitializeComponent();

            initialize();
        }

        private void initialize()
        {
            uiUpdate_Tick(null, null);

            sensorCheckTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            sensorCheckTimer.Tick += new EventHandler(uiUpdate_Tick);
            sensorCheckTimer.Interval = TimeSpan.FromMilliseconds(SENSOR_CHECK_INTERVAL);
            sensorCheckTimer.Start();
        }

        private void uiUpdate_Tick(object sender, EventArgs e)
        {
            ISensorService sensorService = SpringUtil.getService<ISensorService>();

            // checks that the kinect is connected
            if (sensorService.sensorAvailable(SENSOR_IDX))
            {
                startCameraButton.IsEnabled = true;
                startStopButton.IsEnabled = true;
            }
            else
            {
                startCameraButton.IsEnabled = false;
                startStopButton.IsEnabled = false;

                // stop interaction service
                if (null != interaction)
                {
                    interaction = null;
                }

                // close camera window
                if (null != cameraView)
                {
                    cameraView.Stop();
                    cameraView.Close();
                    cameraView = null;
                }
            }
        }

        private void StartCameraButton_Click(object sender, RoutedEventArgs e)
        {
            getCameraView().Show();
            getCameraView().Start();
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private CameraWindow getCameraView()
        {
            if (null == cameraView || cameraView.isClosed)
            {
                Cursor = Cursors.Wait;
                cameraView = new CameraWindow(SENSOR_IDX);
                Cursor = Cursors.Arrow;
            }
            return cameraView;
        }

        private KinectInteractionControl getKinectInteraction()
        {
            if (null == interaction)
            {
                Cursor = Cursors.Wait;
                interaction = new KinectInteractionControl(SENSOR_IDX);
                Cursor = Cursors.Arrow;
            }
            return interaction;
        }

        private void shortcutsButton_Click(object sender, RoutedEventArgs e)
        {
            if (null == shortCutsMainView)
            {
                shortCutsMainView = new ShortCutsMainView(this);
            }
            NavigationService.Navigate(shortCutsMainView);
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
        }
    }
}