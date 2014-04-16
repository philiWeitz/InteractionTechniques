using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using GestureServices.Service.Interface;
using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace InteractionUI.BusinessLogic
{
    public class KinectCameraControl
    {
        private ISensorService sensorService;
        private ICameraService cameraService;
        private ISkeletonService skeletonService;
        private IGestureService gestureService;

        private KinectUser? activeUser = null;

        public bool Enabled { get; set; }

        public Image ScreenImage { get; set; }

        public KinectCameraControl(int sensorIdx)
        {
            initialize(sensorIdx);
        }

        private void initialize(int sensorIdx)
        {
            Enabled = true;

            cameraService = SpringUtil.getService<ICameraService>();
            sensorService = SpringUtil.getService<ISensorService>();
            skeletonService = SpringUtil.getService<ISkeletonService>();
            gestureService = SpringUtil.getService<IGestureService>();

            sensorService.startSensor(sensorIdx);
            cameraService.enableCamera(sensorService.getSensor(sensorIdx));
            skeletonService.enableSkeleton(sensorService.getSensor(sensorIdx));
            gestureService.enableGestureService(sensorService.getSensor(sensorIdx));
        }

        public bool UpdateCamera()
        {
            if (null != ScreenImage)
            {
                byte[] byteArrayIn = cameraService.getImage();

                if (null != byteArrayIn)
                {
                    BitmapSource bitmapSource = BitmapSource.Create(
                        cameraService.getWidth(), cameraService.getHeight(), IConsts.KinectDPIX, IConsts.KinectDPIX,
                        PixelFormats.Bgr32, null, byteArrayIn, cameraService.getWidth() * cameraService.getBytesPerPixel());

                    RenderTargetBitmap bitmap = new RenderTargetBitmap(cameraService.getWidth(),
                        cameraService.getHeight(), IConsts.KinectDPIX, IConsts.KinectDPIX, PixelFormats.Default);

                    DrawingVisual drawingVisual = new DrawingVisual();

                    if (Enabled)
                    {
                        // add draw context in here
                        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                        {
                            List<KinectUser> users = skeletonService.userInRange();

                            // draw camera stream
                            drawingContext.DrawImage(bitmapSource, new Rect(0, 0, bitmap.Width, bitmap.Height));
                            // draw hands
                            drawJoints(drawingContext, users);
                            // draw upper and lower border for kinect elevation angle
                            drawKinectElevationUpperLowerBound(drawingContext);

                            if (users.Count <= 0)
                            {
                                drawingContext.PushOpacity(0.5);
                                drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0, 0, bitmap.Width, bitmap.Height));
                            }
                        }
                    }

                    bitmap.Render(drawingVisual);
                    ScreenImage.Source = bitmap;

                    return true;
                }
            }
            return false;
        }

        public void CheckActiveUserChange(symbol_hand_animationControl handAnimation)
        {
            if (Enabled)
            {
                KinectUser? curr = gestureService.getActiveKinectUser();

                if (null != curr && activeUser != curr)
                {
                    if (skeletonService.hasJoint(JointType.HandLeft, curr.Value)
                        && skeletonService.hasJoint(JointType.HandRight, curr.Value))
                    {
                        KinectDataPoint point;
                        KinectDataPoint handLeft = skeletonService.getDataPoint(JointType.HandLeft, curr.Value);
                        KinectDataPoint handRight = skeletonService.getDataPoint(JointType.HandLeft, curr.Value);

                        if (handLeft.Y < handRight.Y)
                            point = handRight;
                        else
                            point = handLeft;

                        Canvas.SetTop(handAnimation, point.ScreenY);
                        Canvas.SetLeft(handAnimation, point.ScreenX);

                        Storyboard board = (Storyboard)handAnimation.Resources["story_hand_expand"];
                        handAnimation.Visibility = Visibility.Visible;
                        handAnimation.BeginStoryboard(board);
                    }
                }

                activeUser = curr;
            }
        }

        private void drawJoints(DrawingContext drawingContext, List<KinectUser> users)
        {
            drawJointDataQueue(drawingContext);

            foreach (KinectUser user in users)
            {
                if (skeletonService.hasJoint(JointType.HandLeft, user))
                {
                    KinectDataPoint point = skeletonService.getDataPoint(JointType.HandLeft, user);
                    drawingContext.DrawRectangle(Brushes.Green, null, new Rect(point.ScreenX - 10, point.ScreenY - 10, 20, 20));
                }
                if (skeletonService.hasJoint(JointType.HandRight, user))
                {
                    KinectDataPoint point = skeletonService.getDataPoint(JointType.HandRight, user);
                    drawingContext.DrawRectangle(Brushes.Green, null, new Rect(point.ScreenX - 10, point.ScreenY - 10, 20, 20));
                }
            }
        }

        private void drawJointDataQueue(DrawingContext drawingContext)
        {
            Pen pen = new Pen(Brushes.Black, 5);
            pen.Freeze();

            List<KinectDataPoint> queue = gestureService.getActiveUserDataPointQueue();

            Point oldPoint = default(Point);
            foreach (KinectDataPoint dataPoint in queue)
            {
                Point newPoint = new Point(dataPoint.ScreenX, dataPoint.ScreenY);

                if (default(Point) != oldPoint)
                {
                    drawingContext.DrawLine(pen, oldPoint, newPoint);
                }
                oldPoint = newPoint;
            }
        }

        private void drawKinectElevationUpperLowerBound(DrawingContext drawingContext)
        {
            Pen pen = new Pen(Brushes.Black, 2);

            drawingContext.DrawLine(pen,
                new Point(0, IConsts.KinectCenterUpperLimit),
                new Point(IConsts.KinectResolutionWidth, IConsts.KinectCenterUpperLimit));
            drawingContext.DrawLine(pen,
                new Point(0, IConsts.KinectCenterLowerLimit),
                new Point(IConsts.KinectResolutionWidth, IConsts.KinectCenterLowerLimit));
        }
    }
}