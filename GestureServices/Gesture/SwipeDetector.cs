using System;
using System.Collections.Generic;
using System.Linq;
using KinectServices.Common;
using Microsoft.Kinect;

namespace GestureServices.Gesture
{
    internal enum InteractionDirection 
    {
        TO_LEFT = 0,
        TO_RIGHT = 1,
        UP = 2,
        DOWN = 3
    }


    internal class SwipeDetector
    {
        private static readonly int MAX_DEPTH_DIFFERENCE = 40;
        private static readonly int MAX_X_Y_DIFFERENCE = 50;
        private static readonly int MIN_SWIPE_LENGTH = 200;
        private int maxSwipeTime;

        public SwipeDetector(int maxSwipeTime)
        {
            this.maxSwipeTime = maxSwipeTime;
        }

        public bool CheckToLeftSwipeGesture(List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(queue, InteractionDirection.TO_LEFT);
        }

        public bool CheckToRightSwipeGesture(List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(queue, InteractionDirection.TO_RIGHT);
        }

        public bool CheckUpSwipeGesture(List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(queue, InteractionDirection.UP);
        }

        public bool CheckDownSwipeGesture(List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(queue, InteractionDirection.DOWN);
        }

        private bool checkSwipeGesture(List<KinectDataPoint> queue, InteractionDirection direction)
        {
            for (int i = 0; i < queue.Count; ++i)
            {
                if (checkSwipeGesture(queue,i,direction))
                {
                    return true;
                }
            }
            return false;
        }


        private bool checkSwipeGesture(List<KinectDataPoint> queue, int stepSize, InteractionDirection direction)
        {
            int minPoints = stepSize  + 1;

            for (int i = 0; i < queue.Count - minPoints; ++i)
            {
                if (queue.ElementAt(i).TimeStamp.AddMilliseconds(maxSwipeTime) < queue.ElementAt(i + stepSize).TimeStamp)
                {
                    break;
                }

                ColorImagePoint p1 = queue.ElementAt(i).ColorPoint;
                ColorImagePoint p2 = queue.ElementAt(i + stepSize).ColorPoint;

                double length = InteractionMath.CalcDistance(p1, p2);

                if (length >= MIN_SWIPE_LENGTH &&
                    (getLeftRightDirection(p1, p2) == direction || getUpDownDirection(p1, p2) == direction))
                {
                    int depthDiff = Math.Abs(
                        queue.ElementAt(i).DepthPoint.Depth - queue.ElementAt(i + stepSize).DepthPoint.Depth);

                    if (maxXYDifference(p1, p2, direction) && depthDiff < MAX_DEPTH_DIFFERENCE)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private InteractionDirection getLeftRightDirection(ColorImagePoint p1, ColorImagePoint p2) 
        {
            if(p1.X < p2.X) 
            {
                return InteractionDirection.TO_RIGHT;
            }
            return InteractionDirection.TO_LEFT;
        }

        private InteractionDirection getUpDownDirection(ColorImagePoint p1, ColorImagePoint p2)
        {
            if (p1.Y < p2.Y)
            {
                return InteractionDirection.DOWN;
            }
            return InteractionDirection.UP;
        }

        private bool maxXYDifference(ColorImagePoint p1, ColorImagePoint p2, InteractionDirection direction)
        {
            if (direction == InteractionDirection.TO_LEFT || direction == InteractionDirection.TO_RIGHT)
            {
                return (Math.Abs(p1.Y - p2.Y) < MAX_X_Y_DIFFERENCE);
            }
            else
            {
                return (Math.Abs(p1.X - p2.X) < MAX_X_Y_DIFFERENCE);
            }
        }
    }
}
