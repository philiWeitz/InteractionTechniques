using System;
using Microsoft.Kinect;

namespace KinectServices.Common
{
    public class KinectDataPoint
    {
        public DateTime TimeStamp { get; private set; } 
        public ColorImagePoint ColorPoint { get; private set; }
        public DepthImagePoint DepthPoint { get; private set; }


        public KinectDataPoint(ColorImagePoint colorPoint, DepthImagePoint depthPoint)
        {
            this.ColorPoint = colorPoint;
            this.DepthPoint = depthPoint;
            this.TimeStamp = DateTime.Now;
        }
    }
}
