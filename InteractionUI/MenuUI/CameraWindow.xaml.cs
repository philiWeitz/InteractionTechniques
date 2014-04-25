using System.Windows;
using InteractionUI.BusinessLogic;

namespace InteractionUI.MenuUI
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraWindow : Window
    {
        public bool isClosed { get; private set; }

        public KinectCameraControl KinectCamera { get; private set; }

        public CameraWindow(int sensorIdx)
        {
            InitializeComponent();
            initialize(sensorIdx);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        private void initialize(int sensorIdx)
        {
            isClosed = false;
            Closing += new System.ComponentModel.CancelEventHandler(CameraView_Closing);

            KinectCamera = new KinectCameraControl(sensorIdx);
            KinectCamera.ScreenImage = cameraImage;
        }

        private void CameraView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isClosed = true;
        }
    }
}