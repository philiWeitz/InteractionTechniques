using System;
using System.Collections.Generic;
using GestureServices.Gesture;
using GestureServices.Service.Interface;
using InteractionUtil.Common;
using KinectServices.Common;
using Microsoft.Kinect;

namespace GestureServices.Service.Impl
{
    internal class GestureServiceImpl : IGestureService
    {
        private GestureDetector gestureDetector;
        private HashSet<KinectSensor> sensorSet = new HashSet<KinectSensor>();

        public void enableGestureService(KinectSensor sensor)
        {
            if (null != sensor && !sensorSet.Contains(sensor))
            {
                gestureDetector = new GestureDetector(sensor);

                sensorSet.Add(sensor);
                sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            }
        }

        public void setGestureTimeOut(int gestureTimeOut)
        {
            gestureDetector.setGestureTimeOut(gestureTimeOut);
        }

        public bool checkGesture(InteractionGesture gesture)
        {
            return gestureDetector.CheckGesture(gesture);
        }

        public Queue<KinectDataPoint> getDataPointQueue(JointType joint)
        {
            return gestureDetector.getDataPointQueue(joint);
        }

        private void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            gestureDetector.Update();
        }
    }
}