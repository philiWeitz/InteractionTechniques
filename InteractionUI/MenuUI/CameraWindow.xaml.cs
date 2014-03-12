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
        public KinectCamera KinectCamera { get; private set; }
        

        public CameraWindow(int sensorIdx)
        {
            InitializeComponent();
            initialize(sensorIdx);
        }

        public void Start()
        {
            KinectCamera.Start();
        }

        public void Stop()
        {
            KinectCamera.Stop();
        }

        private void initialize(int sensorIdx)
        {
            isClosed = false;
            Closing += new System.ComponentModel.CancelEventHandler(CameraView_Closing);

            KinectCamera = new KinectCamera(sensorIdx);
            KinectCamera.ScreenImage = cameraImage;
        }

        void CameraView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isClosed = true;
            KinectCamera.Stop();
        }
    }
}
