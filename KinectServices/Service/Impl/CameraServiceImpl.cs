using System;
using System.Collections.Generic;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Impl
{
    internal class CameraServiceImpl : ICameraService
    {
        private int height;
        private int width;
        private int bytesPerPixel;

        private byte[] imageBuffer;
        private HashSet<KinectSensor> sensorSet = new HashSet<KinectSensor>();

        public bool Enabled { get; set; }

        public CameraServiceImpl()
        {
            Enabled = true;
        }

        public void startCameraService(KinectSensor sensor)
        {
            if (null != sensor && !sensorSet.Contains(sensor))
            {
                sensorSet.Add(sensor);
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
            if (Enabled)
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

                    // TODO: does this remove the need to allocate the same space over and over again?
                    if (null == imageBuffer || imageBuffer.Length != colorFrame.PixelDataLength)
                    {
                        this.imageBuffer = new byte[colorFrame.PixelDataLength];
                    }

                    colorFrame.CopyPixelDataTo(this.imageBuffer);
                }
            }
        }
    }
}