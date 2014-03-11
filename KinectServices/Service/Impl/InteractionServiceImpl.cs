using System;
using KinectServices.Common;
using KinectServices.Gesture;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Impl
{
    public class InteractionServiceImpl : IInteractionService
    {
        private GestureDetector gestureDetector;

        public InteractionServiceImpl()
        {
            initialize();
        }

        public void enableInteractionService(KinectSensor sensor)
        {
            if (null != sensor)
            {
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
        
        private void initialize()
        {
            gestureDetector = new GestureDetector();
        }

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            gestureDetector.Update();
        }
    }
}
