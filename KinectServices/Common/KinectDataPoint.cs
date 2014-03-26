using System;
using Microsoft.Kinect;

namespace KinectServices.Common
{
    public class KinectDataPoint
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }
        public DateTime TimeStamp { get; private set; } 


        public KinectDataPoint(ColorImagePoint colorPoint, DepthImagePoint depthPoint)
        {
            X = colorPoint.X;
            Y = colorPoint.Y;
            Z = depthPoint.Depth;
            this.TimeStamp = DateTime.Now;
        }

        public double CalcDistance(KinectDataPoint point)
        {
            if (null != point)
            {
                int a = this.X - point.X;
                int b = this.Y - point.Y;
                double c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

                return c;
            }

            return 0;
        }
    }
}
