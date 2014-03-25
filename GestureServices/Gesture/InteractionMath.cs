using System;
using Microsoft.Kinect;

namespace GestureServices.Gesture
{
    internal static class InteractionMath
    {
        public static double CalcDistanceLinePoint(ColorImagePoint v1, ColorImagePoint v2, ColorImagePoint point)
        {
            double i1 = v2.X * (point.Y - v1.Y) - v2.Y * (point.X - v1.X);
            double i2 = v2.Y * (point.X - v1.X) - v2.X * (point.Y - v1.Y);

            double result = Math.Sqrt(Math.Pow(i1, 2) + Math.Pow(i2, 2)) / 
                Math.Sqrt(Math.Pow(v2.X, 2) + Math.Pow(v2.Y, 2));

            return result;
        }

        public static double CalcDistance(ColorImagePoint p1, ColorImagePoint p2)
        {
            int a = p1.X - p2.X;
            int b = p1.Y - p2.Y;
            double c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

            return c;
        }


    }
}
