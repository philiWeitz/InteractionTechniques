using System;
using Microsoft.Kinect;

namespace KinectServices.Common
{
    public class KinectDataPoint
    {
        public int ScreenX { get; private set; }

        public int ScreenY { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Z { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public KinectDataPoint(ColorImagePoint colorPoint, SkeletonPoint skeletonPoint, SkeletonPoint shoulderCenter)
        {
            // coordinates are saved in cm
            initialize(skeletonPoint.X * 100, skeletonPoint.Y * 100,
                (shoulderCenter.Z - skeletonPoint.Z) * 100, colorPoint.X, colorPoint.Y);
        }

        public KinectDataPoint(ColorImagePoint colorPoint, SkeletonPoint skeletonPoint)
        {
            // coordinates are saved in cm
            initialize(skeletonPoint.X * 100, skeletonPoint.Y * 100,
                skeletonPoint.Z * 100, colorPoint.X, colorPoint.Y);
        }

        public KinectDataPoint(float x, float y, float z, int screenX, int screenY)
        {
            initialize(x, y, z, screenX, screenY);
        }

        public void initialize(float x, float y, float z, int screenX, int screenY)
        {
            // coordinates are saved in cm
            X = (int)x;
            Y = (int)y;
            Z = (int)z;

            ScreenX = screenX;
            ScreenY = screenY;

            this.TimeStamp = DateTime.Now;
        }

        public double CalcScreenDistance(KinectDataPoint point)
        {
            if (null != point)
            {
                int x = this.ScreenX - point.ScreenX;
                int y = this.ScreenY - point.ScreenY;
                return Math.Sqrt((x * x) + (y * y));
            }

            return 0;
        }

        public double CalcDistance3D(KinectDataPoint point)
        {
            if (null != point)
            {
                int x = this.X - point.X;
                int y = this.Y - point.Y;
                int z = this.Z - point.Z;

                return Math.Sqrt((x * x) + (y * y) + (z * z));
            }

            return 0;
        }

        public int CalcDepthAngle(KinectDataPoint point)
        {
            return calcAngle(point, new KinectDataPoint(this.X, this.Y, point.Z, 0, 0));
        }

        public int CalcHorizontalAngle(KinectDataPoint point)
        {
            return calcAngle(point, new KinectDataPoint(X, point.Y, Z, 0, 0));
        }

        private int calcAngle(KinectDataPoint point, KinectDataPoint refPoint)
        {
            double b = point.CalcDistance3D(refPoint);
            double c = this.CalcDistance3D(point);

            return (int)(Math.Acos(b / c) * 180 / Math.PI);
        }
    }
}