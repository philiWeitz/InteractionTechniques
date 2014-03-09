using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Stub
{
    public class CameraServiceStub : ICameraService
    {
        public void enableCamera(KinectSensor sensor)
        {
            
        }

        public byte[] getImage()
        {
            return null;
        }

        public int getWidth()
        {
            return 0;
        }

        public int getHeight()
        {
            return 0;
        }

        public int getBytesPerPixel()
        {
            return 0;
        }
    }
}
