﻿using System;
using InteractionUtil.Properties;

namespace InteractionUtil.Util
{
    public static class IConsts
    {
        public static readonly String IMAGE_DIRECTORY = @"Properties\Images\";
        public static readonly String SOUND_DIRECTORY = @"Properties\Sounds\";
        public static readonly String SHORT_CUT_DIRECTORY = @"Properties\ShortCuts\";

        public static readonly int KinectDPIX = 96;
        public static readonly int KinectMinDistance = int.Parse(Resource.KinectMinDistance);
        public static readonly int KinectResolutionWidth = int.Parse(Resource.KinectResolutionWidth);
        public static readonly int KinectResolutionHeight = int.Parse(Resource.KinectResolutionHeight);

        public static readonly int KinectCenterUpperLimit = (int)(KinectResolutionHeight * 0.1);
        public static readonly int KinectCenterLowerLimit = KinectResolutionHeight - (int)(KinectResolutionHeight * 0.3);
        public static readonly int KinectCenterUpperLimitInner = (int)(KinectResolutionHeight * 0.3);
        public static readonly int KinectCenterLowerLimitInner = KinectResolutionHeight - (int)(KinectResolutionHeight * 0.5);

        public static readonly int GCircleDiameter = int.Parse(Resource.GestureCycleDiameter);
        public static readonly int GCircleGitterXY = int.Parse(Resource.GestureCycleGitterXY);
        public static readonly int GesturePushPullAngle = int.Parse(Resource.GesturePushPullAngle);
        public static readonly int GPushPullMinDepth = int.Parse(Resource.GesturePushPullMinDepth);
        public static readonly int GestureQueueSize = int.Parse(Resource.GestureQueueSize);

        public static readonly int GestureQueueSizeCycle = int.Parse(Resource.GestureQueueSizeCycle);
        public static readonly int GestureQueueSizePush = int.Parse(Resource.GestureQueueSizePush);
        public static readonly int GestureQueueSizeSwipe = int.Parse(Resource.GestureQueueSizeSwipe);

        public static readonly int GSwipeDepthAngle = int.Parse(Resource.GestureSwipeDepthAngle);
        public static readonly int GSwipeHorizontalAngle = int.Parse(Resource.GestureSwipeHorizontalAngle);
        public static readonly int GSwipeMinLength = int.Parse(Resource.GestureSwipeMinLength);

        public static readonly int GestureQueueSizeWave = int.Parse(Resource.GestureQueueSizeWave);
        public static readonly int GWaveMinLength = int.Parse(Resource.GestureWaveMinLength);
        public static readonly int GWaveMaxGitter = int.Parse(Resource.GestureWaveMaxGitter);
    }
}