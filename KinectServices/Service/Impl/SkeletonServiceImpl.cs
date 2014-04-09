using System;
using System.Collections.Generic;
using System.Linq;
using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Impl
{
    internal class SkeletonServiceImpl : ISkeletonService
    {
        private static readonly int MAX_SKELETON_COUNT = 6;

        private Skeleton[] skeletonArray = new Skeleton[MAX_SKELETON_COUNT];
        private HashSet<KinectSensor> sensorSet = new HashSet<KinectSensor>();

        private IDictionary<int, KinectUser> idToUserMap = new Dictionary<int, KinectUser>();

        private IDictionary<KinectUser, IDictionary<JointType, KinectDataPoint>> userDataPointMap =
            new Dictionary<KinectUser, IDictionary<JointType, KinectDataPoint>>();

        public SkeletonServiceImpl()
        {
            foreach (KinectUser user in Enum.GetValues(typeof(KinectUser)))
            {
                userDataPointMap[user] = new Dictionary<JointType, KinectDataPoint>();
            }
        }

        public void enableSkeleton(KinectSensor sensor)
        {
            if (null != sensor && !sensorSet.Contains(sensor))
            {
                sensorSet.Add(sensor);
                sensor.AllFramesReady +=
                    new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            }
        }

        public KinectDataPoint getDataPoint(JointType type, KinectUser user)
        {
            return userDataPointMap[user][type];
        }

        public bool hasJoint(JointType type, KinectUser user)
        {
            return userDataPointMap[user].ContainsKey(type);
        }

        public List<KinectUser> userInRange()
        {
            List<KinectUser> result = new List<KinectUser>();

            foreach (KinectUser user in Enum.GetValues(typeof(KinectUser)))
            {
                if (hasJoint(JointType.ShoulderCenter, user))
                {
                    KinectDataPoint shoulderCenter = getDataPoint(JointType.ShoulderCenter, user);
                    if (shoulderCenter.Z > IConsts.KinectMinDistance)
                    {
                        result.Add(user);
                    }
                }
            }
            return result;
        }

        public int getUserBodyAngle(KinectUser user)
        {
            KinectDataPoint sLeft = userDataPointMap[user][JointType.ShoulderLeft];
            KinectDataPoint sRight = userDataPointMap[user][JointType.ShoulderRight];

            return sRight.CalcDepthAngle(sLeft);
        }

        private void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                // can happen that we have to drop frames
                if (null == skeletonFrame)
                {
                    return;
                }

                skeletonFrame.CopySkeletonDataTo(skeletonArray);

                List<Skeleton> skeletons = (
                    from s in skeletonArray
                    where s != null && s.TrackingState == SkeletonTrackingState.Tracked
                    select s).ToList();

                // clear last joint points
                foreach (Dictionary<JointType, KinectDataPoint> map in userDataPointMap.Values)
                {
                    map.Clear();
                }

                updateUserToTrackingId(skeletons);

                KinectSensor sensor = (KinectSensor)sender;
                CoordinateMapper coordinateMapper = new CoordinateMapper(sensor);

                foreach (Skeleton skeleton in skeletons)
                {
                    if (!idToUserMap.ContainsKey(skeleton.TrackingId))
                    {
                        continue;
                    }

                    KinectUser user = idToUserMap[skeleton.TrackingId];

                    userDataPointMap[user][JointType.HandLeft] =
                        getDataPointRelativeToBody(skeleton.Joints, JointType.HandLeft, coordinateMapper);
                    userDataPointMap[user][JointType.HandRight] =
                        getDataPointRelativeToBody(skeleton.Joints, JointType.HandRight, coordinateMapper);

                    userDataPointMap[user][JointType.ShoulderCenter] =
                        getDataPointAbsolut(skeleton.Joints, JointType.ShoulderCenter, coordinateMapper);
                    userDataPointMap[user][JointType.ShoulderLeft] =
                        getDataPointAbsolut(skeleton.Joints, JointType.ShoulderLeft, coordinateMapper);
                    userDataPointMap[user][JointType.ShoulderRight] =
                        getDataPointAbsolut(skeleton.Joints, JointType.ShoulderRight, coordinateMapper);
                }
            }
        }

        private KinectDataPoint getDataPointAbsolut(JointCollection joints,
            JointType joint, CoordinateMapper coordinateMapper)
        {
            ColorImagePoint colorPoint = coordinateMapper.MapSkeletonPointToColorPoint(
                joints[joint].Position, ColorImageFormat.RgbResolution640x480Fps30);

            return new KinectDataPoint(colorPoint, joints[joint].Position);
        }

        private KinectDataPoint getDataPointRelativeToBody(JointCollection joints,
            JointType joint, CoordinateMapper coordinateMapper)
        {
            ColorImagePoint colorPoint = coordinateMapper.MapSkeletonPointToColorPoint(
                joints[joint].Position, ColorImageFormat.RgbResolution640x480Fps30);

            return new KinectDataPoint(colorPoint, joints[joint].Position, joints[JointType.ShoulderCenter].Position);
        }

        private void updateUserToTrackingId(List<Skeleton> skeletons)
        {
            if (skeletons.Count > 0)
            {
                // get all current skeleton ids
                HashSet<int> currSkeletonIds = new HashSet<int>();
                foreach (Skeleton s in skeletons)
                {
                    currSkeletonIds.Add(s.TrackingId);
                }

                // remove all old ids and get a list of all available users
                HashSet<KinectUser> availableUsers = new
                    HashSet<KinectUser>(Enum.GetValues(typeof(KinectUser)).Cast<KinectUser>());

                List<Skeleton> availableSkeletons = new List<Skeleton>();

                foreach (KeyValuePair<int, KinectUser> item in new Dictionary<int, KinectUser>(idToUserMap))
                {
                    if (currSkeletonIds.Contains(item.Key))
                    {
                        availableUsers.Remove(item.Value);
                    }
                    else
                    {
                        idToUserMap.Remove(item.Key);
                    }
                }

                // add new user - skeleton mapping
                foreach (Skeleton s in skeletons)
                {
                    if (availableUsers.Count <= 0)
                    {
                        break;
                    }

                    if (!idToUserMap.ContainsKey(s.TrackingId))
                    {
                        idToUserMap[s.TrackingId] = availableUsers.First();
                        availableUsers.Remove(availableUsers.First());
                    }
                }
            }
        }
    }
}