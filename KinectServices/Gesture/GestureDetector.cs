using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Gesture
{
    class GestureDetector
    {
        private PointQueue dataPointQueue;
        private SwipeDetector swipeDetector;
        private PushPullGestureDetector pushPullDetector;
        private ISkeletonService skeletonService = null;


        public GestureDetector()
        {
            initialize();
        }

        public void Update()
        {
            addDataPoint(JointType.HandLeft);
            addDataPoint(JointType.HandRight);
        }

        public void ClearGestureQueue()
        {
            dataPointQueue.ClearQueue();
        }

        public bool CheckGesture(InteractionGesture gesture)
        {
            bool result = false;

            switch (gesture)
            {
                case InteractionGesture.PullOneHanded:
                    result = pushPullDetector.CheckPullGuesture(dataPointQueue.GetQueue(JointType.HandLeft))
                        || pushPullDetector.CheckPullGuesture(dataPointQueue.GetQueue(JointType.HandRight));
                    break;
                case InteractionGesture.PullTwoHanded:
                    result = pushPullDetector.CheckPullGuesture(dataPointQueue.GetQueue(JointType.HandLeft))
                        && pushPullDetector.CheckPullGuesture(dataPointQueue.GetQueue(JointType.HandRight));
                    break;
                case InteractionGesture.PushOneHanded:
                    result = pushPullDetector.CheckPushGuesture(dataPointQueue.GetQueue(JointType.HandLeft))
                        || pushPullDetector.CheckPushGuesture(dataPointQueue.GetQueue(JointType.HandRight));
                    break;
                case InteractionGesture.PushTwoHanded:
                    result = pushPullDetector.CheckPushGuesture(dataPointQueue.GetQueue(JointType.HandLeft))
                        && pushPullDetector.CheckPushGuesture(dataPointQueue.GetQueue(JointType.HandRight));
                    break;
                case InteractionGesture.SwipeToLeft:
                    result = swipeDetector.CheckToLeftSwipeGuesture(dataPointQueue.GetQueue(JointType.HandLeft))
                        || swipeDetector.CheckToLeftSwipeGuesture(dataPointQueue.GetQueue(JointType.HandRight));
                    break;
                case InteractionGesture.SwipeToRight:
                    result = swipeDetector.CheckToRightSwipeGuesture(dataPointQueue.GetQueue(JointType.HandLeft))
                        || swipeDetector.CheckToRightSwipeGuesture(dataPointQueue.GetQueue(JointType.HandRight));
                    break;
                case InteractionGesture.SwipeUp:
                    result = swipeDetector.CheckUpSwipeGuesture(dataPointQueue.GetQueue(JointType.HandLeft))
                        || swipeDetector.CheckUpSwipeGuesture(dataPointQueue.GetQueue(JointType.HandRight));
                    break;
                case InteractionGesture.SwipeDown:
                    result = swipeDetector.CheckDownSwipeGuesture(dataPointQueue.GetQueue(JointType.HandLeft))
                        || swipeDetector.CheckDownSwipeGuesture(dataPointQueue.GetQueue(JointType.HandRight));
                    break;
                default:
                    break;
            }

            return result;
        }

        private void initialize()
        {
            dataPointQueue = new PointQueue(500);

            swipeDetector = new SwipeDetector();
            pushPullDetector = new PushPullGestureDetector();
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
            }
            return skeletonService;
        }
    }
}
