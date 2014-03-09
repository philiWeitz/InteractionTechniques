using System;
using System.Collections.Generic;
using System.Linq;
using KinectServices.Common;
using Microsoft.Kinect;

namespace KinectServices.Gesture
{
    public enum InteractionDirection 
    {
        TO_LEFT = 0,
        TO_RIGHT = 1,
        UP = 2,
        DOWN = 3
    }


    class SwipeDetector
    {
        private static readonly int MAX_X_Y_DIFFERENCE = 50;
        private static readonly int MIN_SWIPE_LENGTH = 200;


        public bool CheckToLeftSwipeGuesture(Queue<KinectDataPoint> queue)
        {
            return checkSwipeGuesture(queue, InteractionDirection.TO_LEFT);
        }

        public bool CheckToRightSwipeGuesture(Queue<KinectDataPoint> queue)
        {
            return checkSwipeGuesture(queue, InteractionDirection.TO_RIGHT);
        }

        public bool CheckUpSwipeGuesture(Queue<KinectDataPoint> queue)
        {
            return checkSwipeGuesture(queue, InteractionDirection.UP);
        }

        public bool CheckDownSwipeGuesture(Queue<KinectDataPoint> queue)
        {
            return checkSwipeGuesture(queue, InteractionDirection.DOWN);
        }

        private bool checkSwipeGuesture(Queue<KinectDataPoint> queue, InteractionDirection direction)
        {
            for (int i = (queue.Count / 2); i <= queue.Count; ++i)
            {
                if (checkSwipeGuesture(queue,i,direction))
                {
                    return true;
                }
            }
            return false;
        }


        private bool checkSwipeGuesture(Queue<KinectDataPoint> queue, int stepSize, InteractionDirection direction)
        {
            for (int i = 0; i + stepSize < queue.Count; ++i)
            {
                ColorImagePoint p1 = queue.ElementAt(i).ColorPoint;
                ColorImagePoint p2 = queue.ElementAt(i + stepSize).ColorPoint; 

                double length = InteractionMath.CalcDistance(p1,p2);

                if (length >= MIN_SWIPE_LENGTH &&
                    (getLeftRightDirection(p1, p2) == direction || getUpDownDirection(p1, p2) == direction))
                {
                    if (maxXYDifference(p1,p2,direction))
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
