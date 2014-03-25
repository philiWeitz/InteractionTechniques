using System;
using System.Collections.Generic;
using System.Linq;
using KinectServices.Common;
using Microsoft.Kinect;

namespace GestureServices.Gesture
{
    internal enum InteractionCircle
    {
        CLOCK_WISE = 0,
        COUNTER_CLOCK_WISE = 1
    }

    internal class CircleDetector
    {
        private static readonly int MIN_DIAMETER = 75;
        private static readonly int MAX_X_Y_GITTER = 25;
        private int maxCircleTime;


        public CircleDetector(int maxCircleTime)
        {
            this.maxCircleTime = maxCircleTime;
        }

        public bool CheckCircleCounterClockGesture(Queue<KinectDataPoint> queue)
        {
            return checkCircleGesture(queue, InteractionCircle.COUNTER_CLOCK_WISE);
        }

        public bool CheckCircleClockGesture(Queue<KinectDataPoint> queue)
        {
            return checkCircleGesture(queue, InteractionCircle.CLOCK_WISE);
        }

        private bool checkCircleGesture(Queue<KinectDataPoint> queue, InteractionCircle direction)
        {
            for (int i = 0; i < queue.Count; ++i)
            {
                if (checkCircleGesture(queue, i, direction))
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkCircleGesture(Queue<KinectDataPoint> queue, int stepSize, InteractionCircle direction)
        {
            int minPoints = stepSize * 4 + 1;

            for (int i = 0; i < queue.Count - minPoints; ++i)
            {
                if (queue.ElementAt(i).TimeStamp.AddMilliseconds(maxCircleTime) < queue.ElementAt(i + (4 * stepSize)).TimeStamp)
                {
                    break;
                }

                ColorImagePoint p1 = queue.ElementAt(i).ColorPoint;
                ColorImagePoint p2 = queue.ElementAt(i + stepSize).ColorPoint;
                ColorImagePoint p3 = queue.ElementAt(i + (2 * stepSize)).ColorPoint;
                ColorImagePoint p4 = queue.ElementAt(i + (3 * stepSize)).ColorPoint;

                double diag1 = InteractionMath.CalcDistance(p1, p3);
                double diag2 = InteractionMath.CalcDistance(p2, p4);
                double dist1 = InteractionMath.CalcDistance(p1, p2);
                double dist2 = InteractionMath.CalcDistance(p2, p3);
                double dist3 = InteractionMath.CalcDistance(p3, p4);
                double dist4 = InteractionMath.CalcDistance(p4, p1);

                if (diag1 > MIN_DIAMETER && diag2 > MIN_DIAMETER)
                {
                    if (Math.Abs(dist1 - dist2) < MAX_X_Y_GITTER && Math.Abs(dist2 - dist3) < MAX_X_Y_GITTER &&
                        Math.Abs(dist3 - dist4) < MAX_X_Y_GITTER && Math.Abs(dist4 - dist1) < MAX_X_Y_GITTER)
                    {
                        if (getDirection(p1,p2) == direction)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private InteractionCircle getDirection(ColorImagePoint p1, ColorImagePoint p2)
        {
            int dirX = p2.X - p1.X;

            if (dirX >= 0)
            {
                return InteractionCircle.CLOCK_WISE;
            }
            return InteractionCircle.COUNTER_CLOCK_WISE;
        }
    }
}
