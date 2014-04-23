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

        private bool _enabled = true;
        private KinectUser? activeUser = null;

        public Image ScreenImage { get; set; }

        public KinectCameraControl(int sensorIdx)
        {
            initialize(sensorIdx);
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                if (null != cameraService)
                {
                    cameraService.Enabled = value;
                }
                _enabled = value;
            }
        }

        private void initialize(int sensorIdx)
        {
            Enabled = true;

            cameraService = SpringUtil.getService<ICameraService>();
            sensorService = SpringUtil.getService<ISensorService>();
            skeletonService = SpringUtil.getService<ISkeletonService>();
            gestureService = SpringUtil.getService<IGestureService>();

            sensorService.startSensor(sensorIdx);
            cameraService.startCameraService(sensorService.getSensor(sensorIdx));
            skeletonService.enableSkeleton(sensorService.getSensor(sensorIdx));
            gestureService.enableGestureService(sensorService.getSensor(sensorIdx));
        }

        public bool UpdateCamera()
        {
            if (null != ScreenImage)
            {
                if (Enabled)
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
                            drawingContext.Close();
                        }

                        bitmap.Render(drawingVisual);
                        ScreenImage.Source = bitmap;

                        return true;
                    }
                }
                else
                {
                    ScreenImage.Source = null;
                    return true;
                }
            }
            return false;
        }

        public void CheckActiveUserChange(symbol_hand_animationControl handAnimation)
        {
            if (Enabled && skeletonService.userInRange().Count > 0)
            {
                KinectUser? curr = gestureService.getActiveKinectUser();

                if (null != curr && activeUser != curr)
                {
                    KinectDataPoint handLeft = skeletonService.getDataPoint(JointType.HandLeft, curr.Value);
                    KinectDataPoint handRight = skeletonService.getDataPoint(JointType.HandRight, curr.Value);

                    if (null != handLeft && null != handRight)
                    {
                        KinectDataPoint point;

                        if (handLeft.Y < handRight.Y)
                            point = handLeft;
                        else
                            point = handRight;

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
            //drawJointDataQueue(drawingContext);
            drawJointDataQueueAsPoints(drawingContext);

            foreach (KinectUser user in users)
            {
                KinectDataPoint handLeft = skeletonService.getDataPoint(JointType.HandLeft, user);
                KinectDataPoint handRight = skeletonService.getDataPoint(JointType.HandRight, user);

                if (null != handLeft)
                {
                    drawingContext.DrawRectangle(Brushes.Green, null, new Rect(handLeft.ScreenX - 10, handLeft.ScreenY - 10, 20, 20));
                }
                if (null != handRight)
                {
                    drawingContext.DrawRectangle(Brushes.Green, null, new Rect(handRight.ScreenX - 10, handRight.ScreenY - 10, 20, 20));
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

        private void drawJointDataQueueAsPoints(DrawingContext drawingContext)
        {
            List<KinectDataPoint> queue = gestureService.getActiveUserDataPointQueue();

            foreach (KinectDataPoint dataPoint in queue)
            {
                drawingContext.DrawRectangle(
                    Brushes.Red, null, new Rect(dataPoint.ScreenX - 5, dataPoint.ScreenY - 5, 10, 10));
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