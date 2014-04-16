using System.Collections.Generic;
using GestureServices.Service.Interface;
using InteractionUtil.Common;
using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace GestureServices.Service.Stub
{
    internal class GestureServiceStub : IGestureService
    {
        private ISkeletonService service;

        public void enableGestureService(KinectSensor sensor)
        {
            if (null == service)
            {
                service = SpringUtil.getService<ISkeletonService>();
            }
        }

        public void setGestureTimeOut(int gestureTimeOut)
        {
        }

        public InteractionGesture checkAllGestures()
        {
            return InteractionGesture.None;
        }

        public void focuseCurrentUser()
        {
        }

        public List<KinectDataPoint> getActiveUserDataPointQueue()
        {
            return new List<KinectDataPoint>();
        }

        public KinectUser? getActiveKinectUser()
        {
            if (service.hasJoint(JointType.ShoulderCenter, KinectUser.User1))
            {
                return KinectUser.User1;
            }
            return null;
        }
    }
}