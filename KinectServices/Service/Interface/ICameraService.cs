using Microsoft.Kinect;

namespace KinectServices.Service.Interface
{
    public interface ICameraService
    {
        void enableCamera(KinectSensor sensor);
        byte[] getImage();
        int getWidth();
        int getHeight();
        int getBytesPerPixel();
    }
}
