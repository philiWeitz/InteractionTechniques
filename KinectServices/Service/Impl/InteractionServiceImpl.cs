using System;
using System.Collections.Generic;
using InteractionUtil.Common;
using KinectServices.Gesture;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Impl
{
    internal class InteractionServiceImpl : IInteractionService
    {
        private GestureDetector gestureDetector;
        private HashSet<KinectSensor> sensorSet = new HashSet<KinectSensor>();


        public void enableInteractionService(KinectSensor sensor)
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

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            gestureDetector.Update();
        }
    }
}
