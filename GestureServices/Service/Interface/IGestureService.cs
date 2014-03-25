﻿using System.Collections.Generic;
using InteractionUtil.Common;
using KinectServices.Common;
using Microsoft.Kinect;

namespace GestureServices.Service.Interface
{
    public interface IGestureService
    {
        void enableGestureService(KinectSensor sensor);

        void setGestureTimeOut(int gestureTimeOut);

        bool checkGesture(InteractionGesture gesture);

        List<KinectDataPoint> getDataPointQueue(JointType joint);
    }
}