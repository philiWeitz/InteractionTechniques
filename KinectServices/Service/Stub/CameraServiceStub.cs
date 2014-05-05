using InteractionUtil.Util;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Stub
{
    internal class CameraServiceStub : ICameraService
    {
        private byte[] image;

        public bool Enabled { get; set; }

        public CameraServiceStub()
        {
            image = new byte[IConsts.KinectResolutionWidth * IConsts.KinectResolutionHeight * 256];

            for (int i = 0; i < image.Length; ++i)
            {
                image[i] = 200;
            }
        }

        public void startCameraService(KinectSensor sensor)
        {
        }

        public byte[] getImage()
        {
            return image;
        }

        public int getWidth()
        {
            return IConsts.KinectResolutionWidth;
        }

        public int getHeight()
        {
            return IConsts.KinectResolutionHeight;
        }

        public int getBytesPerPixel()
        {
            return 8;
        }
    }
}