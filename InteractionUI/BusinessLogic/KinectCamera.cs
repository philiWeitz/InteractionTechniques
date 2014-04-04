using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GestureServices.Service.Interface;
using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace InteractionUI.BusinessLogic
{
    public class KinectCamera
    {
        private static readonly int INTERVAL = 100;

        private ISensorService sensorService;
        private ICameraService cameraService;
        private ISkeletonService skeletonService;
        private IGestureService gestureService;

        private DispatcherTimer cameraTimer;
        private String lastGesture = "";

        public Image ScreenImage { get; set; }

        public KinectCamera(int sensorIdx)
        {
            initialize(sensorIdx);
        }

        public void Start()
        {
            cameraTimer.Start();
        }

        public void Stop()
        {
            cameraTimer.Stop();
        }

        public void AddGestureTextEvent(KinectInteraction interaction)
        {
            interaction.GestureEvent += new EventHandler(interaction_GestureEvent);
        }

        private void initialize(int sensorIdx)
        {
            cameraService = SpringUtil.getService<ICameraService>();
            sensorService = SpringUtil.getService<ISensorService>();
            skeletonService = SpringUtil.getService<ISkeletonService>();
            gestureService = SpringUtil.getService<IGestureService>();

            sensorService.startSensor(sensorIdx);
            cameraService.enableCamera(sensorService.getSensor(sensorIdx));
            skeletonService.enableSkeleton(sensorService.getSensor(sensorIdx));
            gestureService.enableGestureService(sensorService.getSensor(sensorIdx));

            cameraTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            cameraTimer.Tick += new EventHandler(cameraTimer_Tick);
            cameraTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL);
        }

        private void interaction_GestureEvent(object sender, EventArgs e)
        {
            lastGesture = ((KinectInteractionArg)e).GestureName;
        }

        private void cameraTimer_Tick(object sender, EventArgs e)
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

                    // add draw context in here
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        // draw camera stream
                        drawingContext.DrawImage(bitmapSource, new Rect(0, 0, bitmap.Width, bitmap.Height));
                        // draw hands
                        drawJoints(drawingContext);
                        // draw upper and lower border for kinect elevation angle
                        drawKinectElevationUpperLowerBound(drawingContext);

                        if (!skeletonService.userInRange())
                        {
                            drawingContext.PushOpacity(0.2);
                            drawingContext.DrawRectangle(Brushes.Red, null, new Rect(0, 0, bitmap.Width, bitmap.Height));
                        }
                    }

                    bitmap.Render(drawingVisual);
                    ScreenImage.Source = bitmap;
                }
            }
        }

        private void drawJoints(DrawingContext drawingContext)
        {
            if (skeletonService.hasJoint(JointType.HandLeft))
            {
                KinectDataPoint point = skeletonService.getDataPoint(JointType.HandLeft);
                drawingContext.DrawRectangle(Brushes.Green, null, new Rect(point.X - 10, point.Y - 10, 20, 20));
                drawJointDataQueue(drawingContext, JointType.HandLeft);
            }
            if (skeletonService.hasJoint(JointType.HandRight))
            {
                KinectDataPoint point = skeletonService.getDataPoint(JointType.HandRight);
                drawingContext.DrawRectangle(Brushes.Green, null, new Rect(point.X - 10, point.Y - 10, 20, 20));
            }

            FormattedText formattedText = new FormattedText(lastGesture, CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.Black);

            drawingContext.DrawText(formattedText, new Point(10, 10));
        }

        private void drawJointDataQueue(DrawingContext drawingContext, JointType joint)
        {
            Pen pen = new Pen(Brushes.Black, 5);
            pen.Freeze();

            List<KinectDataPoint> queue = gestureService.getDataPointQueue(joint);

            Point oldPoint = default(Point);
            foreach (KinectDataPoint dataPoint in queue)
            {
                Point newPoint = new Point(dataPoint.X, dataPoint.Y);

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