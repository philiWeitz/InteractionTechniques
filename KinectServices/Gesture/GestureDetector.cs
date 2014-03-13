using System;
using InteractionUtil.Common;
using InteractionUtil.Util;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Gesture
{
    internal class GestureDetector
    {
        private static readonly int MAX_QUEUE_SIZE_IN_MS = 1000;
        private static readonly int MAX_SWIPE_TIME_IN_MS = 500;
        private static readonly int MAX_PUSH_PULL_TIME_IN_MS = 400;
        private static readonly int MAX_CIRCLE_TIME_IN_MS = 1000;

        private KinectSensor sensor;
        private DateTime gestureTime;
        private PointQueue dataPointQueue;
        private SwipeDetector swipeDetector;
        private CircleDetector circleDetector;
        private PushPullGestureDetector pushPullDetector;
        private ISkeletonService skeletonService = null;


        public GestureDetector(KinectSensor sensor)
        {
            initialize(sensor);
        }

        public void Update()
        {
            addDataPoint(JointType.HandLeft);
            addDataPoint(JointType.HandRight);
        }

        public void setGestureTimeOut(int gestureTimeOut)
        {
            gestureTime = DateTime.Now.AddMilliseconds(gestureTimeOut);
        }

        public bool CheckGesture(InteractionGesture gesture)
        {
            bool result = false;

            if (getSekeltonService().userInRange() && !isTimeOut())
            {
                switch (gesture)
                {
                    case InteractionGesture.CircleCounterClock:
                        result = circleDetector.CheckCircleCounterClockGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            || circleDetector.CheckCircleCounterClockGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    case InteractionGesture.CircleClock:
                        result = circleDetector.CheckCircleClockGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            || circleDetector.CheckCircleClockGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    case InteractionGesture.PullOneHanded:
                        result = pushPullDetector.CheckPullGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            || pushPullDetector.CheckPullGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    case InteractionGesture.PullTwoHanded:
                        result = pushPullDetector.CheckPullGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            && pushPullDetector.CheckPullGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    case InteractionGesture.PushOneHanded:
                        result = pushPullDetector.CheckPushGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            || pushPullDetector.CheckPushGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    case InteractionGesture.PushTwoHanded:
                        result = pushPullDetector.CheckPushGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            && pushPullDetector.CheckPushGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    case InteractionGesture.SwipeToLeft:
                        result = swipeDetector.CheckToLeftSwipeGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            || swipeDetector.CheckToLeftSwipeGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    case InteractionGesture.SwipeToRight:
                        result = swipeDetector.CheckToRightSwipeGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            || swipeDetector.CheckToRightSwipeGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    case InteractionGesture.SwipeUp:
                        result = swipeDetector.CheckUpSwipeGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            || swipeDetector.CheckUpSwipeGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    case InteractionGesture.SwipeDown:
                        result = swipeDetector.CheckDownSwipeGesture(dataPointQueue.GetQueue(JointType.HandLeft))
                            || swipeDetector.CheckDownSwipeGesture(dataPointQueue.GetQueue(JointType.HandRight));
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        private void initialize(KinectSensor sensor)
        {
            this.sensor = sensor;
            dataPointQueue = new PointQueue(MAX_QUEUE_SIZE_IN_MS);

            swipeDetector = new SwipeDetector(MAX_SWIPE_TIME_IN_MS);
            circleDetector = new CircleDetector(MAX_CIRCLE_TIME_IN_MS);
            pushPullDetector = new PushPullGestureDetector(MAX_PUSH_PULL_TIME_IN_MS);
        }

        private void addDataPoint(JointType joint)
        {
            if (getSekeltonService().hasJoint(joint))
            {
                dataPointQueue.AddPoint(getSekeltonService().getDataPoint(joint), joint);
            }
        }

        private ISkeletonService getSekeltonService()
        {
            if (null == skeletonService)
            {
                skeletonService = SpringUtil.getService<ISkeletonService>();
                skeletonService.enableSkeleton(sensor);
            }
            return skeletonService;
        }

        private bool isTimeOut()
        {
            if (gestureTime > DateTime.Now)
            {
                dataPointQueue.ClearQueue();
                return true;
            }
            return false;
        }
    }
}
