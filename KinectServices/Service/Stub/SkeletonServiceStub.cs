﻿using System.Collections.Generic;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Stub
{
    internal class SkeletonServiceStub : ISkeletonService
    {
        private IDictionary<JointType, ColorImagePoint> jointMap =
                    new Dictionary<JointType, ColorImagePoint>();

        public void enableSkeleton(KinectSensor sensor)
        {
            System.Timers.Timer timer = new System.Timers.Timer(10);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        public KinectDataPoint getDataPoint(JointType type, KinectUser user)
        {
            return null;
        }

        public bool hasJoint(JointType type, KinectUser user)
        {
            return jointMap.ContainsKey(type);
        }

        public List<KinectUser> userInRange()
        {
            return new List<KinectUser>();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // TODO: not implemented
        }

        public int getUserBodyAngle(KinectUser user)
        {
            return 0;
        }
    }
}