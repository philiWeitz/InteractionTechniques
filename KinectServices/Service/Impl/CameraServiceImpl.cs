using System;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Impl
{
    public class CameraServiceImpl : ICameraService
    {
        private int height;
        private int width;
        private int bytesPerPixel;

        private byte[] imageBuffer;

        public void enableCamera(KinectSensor sensor)
        {
            if (null != sensor)
            {
                sensor.ColorFrameReady +=
                    new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            }
        }


        public byte[] getImage()
        {
            return imageBuffer;
        }


        public int getWidth()
        {
            return width;
        }


        public int getHeight()
        {
            return height;
        }

        public int getBytesPerPixel()
        {
            return bytesPerPixel;
        }

        private void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                // can happen that we have to drop frames
                if (null == colorFrame)
                {
                    return;
                }

                height = colorFrame.Height;
                width = colorFrame.Width;
                bytesPerPixel = colorFrame.BytesPerPixel;

                this.imageBuffer = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(this.imageBuffer);
            }
        }
    }
}
