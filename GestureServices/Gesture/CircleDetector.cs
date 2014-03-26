using System;
using System.Collections.Generic;
using System.Linq;
using KinectServices.Common;

namespace GestureServices.Gesture
{
    internal enum InteractionCircle
    {
        CLOCK_WISE = 0,
        COUNTER_CLOCK_WISE = 1
    }

    internal class CircleDetector
    {
        private static readonly int MIN_DIAMETER = 50;
        private static readonly int MAX_X_Y_GITTER = 25;
        private int maxCircleTime;

        public CircleDetector(int maxCircleTime)
        {
            this.maxCircleTime = maxCircleTime;
        }

        public bool CheckCircleCounterClockGesture(List<KinectDataPoint> queue)
        {
            return checkCircleGesture(queue, InteractionCircle.COUNTER_CLOCK_WISE);
        }

        public bool CheckCircleClockGesture(List<KinectDataPoint> queue)
        {
            return checkCircleGesture(queue, InteractionCircle.CLOCK_WISE);
        }

        private bool checkCircleGesture(List<KinectDataPoint> queue, InteractionCircle direction)
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

        private bool checkCircleGesture(List<KinectDataPoint> queue, int stepSize, InteractionCircle direction)
        {
            int minPoints = stepSize * 4 + 1;

            for (int i = 0; i < queue.Count - minPoints; ++i)
            {
                KinectDataPoint p1 = queue.ElementAt(i);
                KinectDataPoint p4 = queue.ElementAt(i + (3 * stepSize));

                if (p1.TimeStamp.AddMilliseconds(maxCircleTime) < p4.TimeStamp)
                {
                    break;
                }

                KinectDataPoint p2 = queue.ElementAt(i + stepSize);
                KinectDataPoint p3 = queue.ElementAt(i + (2 * stepSize));

                double diag1 = p1.CalcDistance(p3);
                double diag2 = p2.CalcDistance(p4);
                double dist1 = p1.CalcDistance(p2);
                double dist2 = p2.CalcDistance(p3);
                double dist3 = p3.CalcDistance(p4);
                double dist4 = p4.CalcDistance(p1);

                if (diag1 > MIN_DIAMETER && diag2 > MIN_DIAMETER)
                {
                    if (Math.Abs(dist1 - dist2) < MAX_X_Y_GITTER && Math.Abs(dist2 - dist3) < MAX_X_Y_GITTER &&
                        Math.Abs(dist3 - dist4) < MAX_X_Y_GITTER && Math.Abs(dist4 - dist1) < MAX_X_Y_GITTER)
                    {
                        if (getDirection(p1, p2) == direction)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private InteractionCircle getDirection(KinectDataPoint p1, KinectDataPoint p2)
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