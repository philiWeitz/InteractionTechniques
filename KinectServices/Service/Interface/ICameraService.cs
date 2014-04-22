using Microsoft.Kinect;

namespace KinectServices.Service.Interface
{
    public interface ICameraService
    {
        void startCameraService(KinectSensor sensor);

        bool Enabled { get; set; }

        byte[] getImage();

        int getWidth();

        int getHeight();

        int getBytesPerPixel();
    }
}