using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Stub
{
    internal class SensorServiceStub : ISensorService
    {
        public bool sensorAvailable(int idx)
        {
            return true;
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