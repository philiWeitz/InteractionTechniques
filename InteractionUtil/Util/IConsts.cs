﻿using System;
using InteractionUtil.Properties;

namespace InteractionUtil.Util
{
    public static class IConsts
    {
        public static readonly String IMAGE_DIRECTORY = @"Content\Images\";
        public static readonly String SHORT_CUT_DIRECTORY = @"Properties\ShortCuts\";

        public static readonly int KinectDPIX = 96;
        public static readonly int KinectMinDistance = int.Parse(Resource.KinectMinDistance);
        public static readonly int KinectStdDistance = int.Parse(Resource.KinectStdDistance);

        public static readonly int GCircleDiameter = int.Parse(Resource.GestureCycleDiameter);
        public static readonly int GCircleGitterXY = int.Parse(Resource.GestureCycleGitterXY);
        public static readonly int GPushPullGitterXY = int.Parse(Resource.GesturePushPullGitterXY);
        public static readonly int GPushPullMinDepth = int.Parse(Resource.GesturePushPullMinDepth);
        public static readonly int GestureQueueSize = int.Parse(Resource.GestureQueueSize);
        public static readonly int GestureQueueSizeCycle = int.Parse(Resource.GestureQueueSizeCycle);
        public static readonly int GestureQueueSizePush = int.Parse(Resource.GestureQueueSizePush);
        public static readonly int GestureQueueSizeSwipe = int.Parse(Resource.GestureQueueSizeSwipe);
        public static readonly int GSwipeGitterXY = int.Parse(Resource.GestureSwipeGitterXY);
        public static readonly int GSwipeGitterZ = int.Parse(Resource.GestureSwipeGitterZ);
        public static readonly int GSwipeMinLength = int.Parse(Resource.GestureSwipeMinLength);
        public static readonly int GestureTimeOut = int.Parse(Resource.GestureTimeOut);
        public static readonly int GestureTimeOutContinuous = int.Parse(Resource.GestureTimeOutContinuous);
    }
}