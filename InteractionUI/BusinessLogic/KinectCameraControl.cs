﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private static readonly String IMAGE_HAND_LEFT = IConsts.IMAGE_DIRECTORY + "HandLeft.png";
        private static readonly String IMAGE_HAND_RIGHT = IConsts.IMAGE_DIRECTORY + "HandRight.png";
        private static readonly String IMAGE_HAND_LEFT_ACTIVE = IConsts.IMAGE_DIRECTORY + "HandLeftActive.png";
        private static readonly String IMAGE_HAND_RIGHT_ACTIVE = IConsts.IMAGE_DIRECTORY + "HandRightActive.png";

        private ISensorService sensorService;
        private ICameraService cameraService;
        private ISkeletonService skeletonService;
        private IGestureService gestureService;

        private bool _enabled = true;

        private ImageSource imgHandRight;
        private ImageSource imgHandLeft;
        private ImageSource imgHandRightActive;
        private ImageSource imgHandLeftActive;

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

            imgHandLeft = new BitmapImage(new Uri(Application.Current.StartupUri, IMAGE_HAND_LEFT));
            imgHandRight = new BitmapImage(new Uri(Application.Current.StartupUri, IMAGE_HAND_RIGHT));
            imgHandLeftActive = new BitmapImage(new Uri(Application.Current.StartupUri, IMAGE_HAND_LEFT_ACTIVE));
            imgHandRightActive = new BitmapImage(new Uri(Application.Current.StartupUri, IMAGE_HAND_RIGHT_ACTIVE));

            imgHandLeft.Freeze();
            imgHandRight.Freeze();
            imgHandLeftActive.Freeze();
            imgHandRightActive.Freeze();
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
                        bitmapSource.Freeze();

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

                            drawingContext.Close();
                        }

                        bitmap.Render(drawingVisual);
                        bitmap.Freeze();
                        ScreenImage.Source = bitmap;

                        // RenderTargetBitmap has a memory leak -> need to call GC (id = 489723)
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

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

        private void drawJoints(DrawingContext drawingContext, List<KinectUser> users)
        {
            drawJointDataQueue(drawingContext);
            //drawJointDataQueueAsPoints(drawingContext);

            KinectUser? activeUser = gestureService.getActiveKinectUser();

            foreach (KinectUser user in users)
            {
                KinectDataPoint handLeft = skeletonService.getDataPoint(JointType.HandLeft, user);
                KinectDataPoint handRight = skeletonService.getDataPoint(JointType.HandRight, user);

                if (null != handLeft)
                {
                    if (null != activeUser && activeUser.Value == user)
                    {
                        drawHandImage(imgHandLeftActive, handLeft, drawingContext);
                    }
                    else
                    {
                        drawHandImage(imgHandLeft, handLeft, drawingContext);
                    }
                }
                if (null != handRight)
                {
                    if (null != activeUser && activeUser.Value == user)
                    {
                        drawHandImage(imgHandRightActive, handRight, drawingContext);
                    }
                    else
                    {
                        drawHandImage(imgHandRight, handRight, drawingContext);
                    }
                }
            }
        }

        private void drawHandImage(ImageSource source, KinectDataPoint point, DrawingContext drawingContext)
        {
            drawingContext.DrawImage(source, new Rect(
            point.ScreenX - (source.Width / 2),
            point.ScreenY - (source.Height / 2),
            source.Width, source.Height));
        }

        private void drawJointDataQueue(DrawingContext drawingContext)
        {
            Pen pen = new Pen(Brushes.SkyBlue, 5);
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
            pen.Freeze();

            drawingContext.DrawLine(pen,
                new Point(0, IConsts.KinectCenterUpperLimit),
                new Point(IConsts.KinectResolutionWidth, IConsts.KinectCenterUpperLimit));
            drawingContext.DrawLine(pen,
                new Point(0, IConsts.KinectCenterLowerLimit),
                new Point(IConsts.KinectResolutionWidth, IConsts.KinectCenterLowerLimit));
        }
    }
}