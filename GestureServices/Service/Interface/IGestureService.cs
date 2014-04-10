using System.Collections.Generic;
using InteractionUtil.Common;
using KinectServices.Common;
using Microsoft.Kinect;

namespace GestureServices.Service.Interface
{
    public interface IGestureService
    {
        void enableGestureService(KinectSensor sensor);

        void setGestureTimeOut(int gestureTimeOut);

        InteractionGesture checkAllGestures();

        void focuseCurrentUser();

        List<KinectDataPoint> getActiveUserDataPointQueue();
    }
}