using InteractionUtil.Common;
using Microsoft.Kinect;

namespace KinectServices.Service.Interface
{
    public interface IInteractionService
    {
        void enableInteractionService(KinectSensor sensor);
        void setGestureTimeOut(int gestureTimeOut);
        bool checkGesture(InteractionGesture gesture);
    }
}
