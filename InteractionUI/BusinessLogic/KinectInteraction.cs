using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using InteractionUI.Util;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
using KinectServices.Util;
using Microsoft.Kinect;

namespace InteractionUI.BusinessLogic
{
    class KinectInteraction
    {
        private static readonly int INTERVAL = 100;
        private static readonly int SENSOR_IDX = 0;

        private ISensorService sensorService;
        private ICameraService cameraService;
        private IProcessService processService;
        private ISkeletonService skeletonService;
        private IInteractionService interactionService;

        private DispatcherTimer kinectTimer;
        public Image ScreenImage { get; set; }

        private string lastGesture = "";

        public KinectInteraction()
        {
            initialize();
        }

        public void Start()
        {
            kinectTimer.Start();
        }

        private void initialize()
        {
            processService = SpringUtil.getService<IProcessService>();
            cameraService = SpringUtil.getService<ICameraService>();
            sensorService = SpringUtil.getService<ISensorService>();
            skeletonService = SpringUtil.getService<ISkeletonService>();
            interactionService = SpringUtil.getService<IInteractionService>();

            sensorService.startSensor(SENSOR_IDX);
            cameraService.enableCamera(sensorService.getSensor(SENSOR_IDX));
            skeletonService.enableSkeleton(sensorService.getSensor(SENSOR_IDX));
            interactionService.enableInteractionService(sensorService.getSensor(SENSOR_IDX));

            kinectTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            kinectTimer.Tick += new EventHandler(kinectTimer_Tick);
            kinectTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL);
        }

        private void kinectTimer_Tick(object sender, EventArgs e)
        {
            if (null != ScreenImage)
            {
                byte[] byteArrayIn = cameraService.getImage();

                if (null != byteArrayIn)
                {
                    BitmapSource bitmapSource = BitmapSource.Create(
                        cameraService.getWidth(), cameraService.getHeight(), KinectConsts.DPIX, KinectConsts.DPIX,
                        PixelFormats.Bgr32, null, byteArrayIn, cameraService.getWidth() * cameraService.getBytesPerPixel());

                    RenderTargetBitmap bitmap = new RenderTargetBitmap(cameraService.getWidth(),
                        cameraService.getHeight(), KinectConsts.DPIX, KinectConsts.DPIX, PixelFormats.Default);

                    DrawingVisual drawingVisual = new DrawingVisual();

                    // add draw context in here
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        // draw camera stream
                        drawingContext.DrawImage(bitmapSource, new Rect(0, 0, bitmap.Width, bitmap.Height));
                        // draw hands
                        drawJoints(drawingContext);

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

            checkGesture();
        }

        private void checkGesture()
        {
            bool detected = false;

            foreach (InteractionGesture gesture in Enum.GetValues(typeof(InteractionGesture)))
            {
                if (interactionService.checkGesture(gesture))
                {
                    if (InteractionGesture.PushTwoHanded == gesture)
                    {
                        ApplicationList.NextProgram();
                    }
                    else if (InteractionGesture.PullTwoHanded == gesture)
                    {
                        ApplicationList.PreviousProgram();
                    }

                    String shortCut = ShortCutUtil.GetShortCut(gesture.ToString());
                    processService.SendKeyToProcess(ShortCutUtil.GetProcessName(), shortCut);

                    detected = true;
                    break;
                }
            }        

            if (detected)
            {
                System.Media.SystemSounds.Exclamation.Play();
                interactionService.setGestureTimeOut(800);
            }
        }

        // DEBUG
        private void drawJoints(DrawingContext drawingContext)
        {
            if (skeletonService.hasJoint(JointType.HandLeft))
            {
                ColorImagePoint point = skeletonService.getColorPointJoint(JointType.HandLeft);
                drawingContext.DrawRectangle(Brushes.Green, null, new Rect(point.X - 10, point.Y - 10, 20, 20));
            }
            if (skeletonService.hasJoint(JointType.HandRight))
            {
                ColorImagePoint point = skeletonService.getColorPointJoint(JointType.HandRight);
                drawingContext.DrawRectangle(Brushes.Green, null, new Rect(point.X - 10, point.Y - 10, 20, 20));
            }

            foreach (InteractionGesture gesture in Enum.GetValues(typeof(InteractionGesture)))
            {
                if (interactionService.checkGesture(gesture))
                {
                    lastGesture = gesture.ToString();
                    break;
                }
            }

            if (interactionService.checkGesture(InteractionGesture.PushTwoHanded))
            {
                lastGesture = InteractionGesture.PushTwoHanded.ToString();
            }
            else if (interactionService.checkGesture(InteractionGesture.PullTwoHanded))
            {
                lastGesture = InteractionGesture.PullTwoHanded.ToString();
            }

            FormattedText formattedText = new FormattedText(lastGesture, CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.Black);

            drawingContext.DrawText(formattedText, new Point(10, 10));
        }
    }
}
