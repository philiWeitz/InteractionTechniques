using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Stub
{
    public class SensorServiceStub : ISensorService
    {
        public bool sensorAvailable(int idx)
        {
            return false;
        }

        public void startSensor(int idx)
        {
            // nothing to do
        }

        public void stopSensor(int idx)
        {
            // nothing to do
        }

        public KinectSensor getSensor(int idx)
        {
            KinectSensor stub = null;
            return stub;
        }
    }
}
