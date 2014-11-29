
namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.Collections.Generic;
    using System.Timers;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 520.0f;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.GhostWhite;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used to draw points helper
        /// </summary>
        private readonly Brush helperPointBrush = Brushes.Cyan;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 4);

        /// <summary>
        /// Pen used for drawing bones when an error move
        /// </summary>
        private readonly Pen errorBonePen = new Pen(Brushes.Red, 4);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 1;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 4;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;



        // Move detection objects!!
        private RightHandToShoulderYZ RightHandToShoulderYZDetector = new RightHandToShoulderYZ();
        private LeftHandToShoulderYZ LeftHandToShoulderYZDetector = new LeftHandToShoulderYZ();
        private RightHandToShoulderXY RightHandToShoulderXYDetector = new RightHandToShoulderXY();
        private LeftHandToShoulderXY LeftHandToShoulderXYDetector = new LeftHandToShoulderXY();


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            puntuacion.Visibility = Visibility.Hidden;

            // Texto inicio del juego
            resultStats.Text = "Bienvenido al juego de ejercicio de Barcenas.\n";
            resultStats.Text += "En este juego deberás realizar 2 ejercicios\n";
            resultStats.Text += "Uno detrás de otro, durante 1 minuto. \n";
            resultStats.Text += "Los movimientos consisten en llevar tus manos hasta los hombros en el plano XY e YZ \n";
            resultStats.Text += "La imagen del esqueleto te ayuda a hacerte una idea de cómo debes hacerlo. \n";
            resultStats.Text += "\nEl objetivo del juego es recoger el mayor número de monedas. \n";
            resultStats.Text += "Las monedas indican por donde debes pasar tus manos para realizar el movimiento, pero ten cuidado! \n";
            resultStats.Text += "Si no lo haces bien se te caerán las monedas y tendrás que volver a empezar el ejercicio. \n";
            resultStats.Text += "Al final del juego podrás ver cuantas monedas has conseguido, y además, el señor Barcenas\n";
            resultStats.Text += "te dará algo más de dinero dependiendo de lo que le hayas ganado. \n";
            resultStats.Text += "VAMOS! NO DEJES TU SOBRE VACIO! \n";
            resultStats.Text += "\nPara empezar a jugar lleva tu mano DERECHA a tu bolsillo, y entenderemos que quieres llenarlo..";
           
            //Gif del esqueleto, muestra los dos movimientos a realizar
            Guia_Esqueleto.Source = new System.Uri("pack://siteoforigin:,,,/Images/gif_guia_esqueleto_fast.gif", UriKind.Absolute);

        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image2.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.Image.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {

            ejercicio(skeleton, drawingContext);

            /*
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);
            
            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
             * */
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }


            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }





        // Vars and functions needed to check exercise.
        int rightArmSuccess = 0;
        int leftArmSuccess = 0;
        int bothArms = 0;

        Timer timer = null;

        int nEjercicio = 0;

        int timeBetweenMoves = 0;

        bool gameHasStarted = false;
        int diff = 0;
        DateTime startTime = DateTime.Now;
        DateTime finishTime = DateTime.Now;
        int startTimeMin = -1;
        int startTimeSec = -1;

        // Coins
        FormattedText oneEuro = new FormattedText(
                                    "1€",
                                    CultureInfo.GetCultureInfo("es-ES"),
                                    FlowDirection.LeftToRight,
                                    new Typeface("Arial Black"),
                                    32,
                                    Brushes.Gray);

        FormattedText twoEuro = new FormattedText(
                                    "2€",
                                    CultureInfo.GetCultureInfo("es-ES"),
                                    FlowDirection.LeftToRight,
                                    new Typeface("Arial Black"),
                                    32,
                                    Brushes.Gray);

        FormattedText fiveEuro = new FormattedText(
                                    "5€",
                                    CultureInfo.GetCultureInfo("es-ES"),
                                    FlowDirection.LeftToRight,
                                    new Typeface("Arial Black"),
                                    32,
                                    Brushes.Gray);


        private void drawHelpPoints(Brush bru, DrawingContext dc, SkeletonPoint skP, FormattedText text)
        {   // Draw Help Points (Coins)
            Point p = SkeletonPointToScreen(skP);
            dc.DrawEllipse(bru, null, p, 10, 10);
            dc.DrawText(text, p);
        }

        /*
          Función que detecta y procesa el segundo ejercicio en YZ
        */
        private void ejercicioYZ(Skeleton skeleton, DrawingContext drawingContext)
        {
            // This function will draw points and information about this exercise if the user must do it.
            // It will draw help points in the screen
            // it will detect moves
            SkeletonPoint head = skeleton.Joints[JointType.Head].Position;
            SkeletonPoint rightShoulder = skeleton.Joints[JointType.ShoulderRight].Position;
            SkeletonPoint leftShoulder = skeleton.Joints[JointType.ShoulderLeft].Position;
            SkeletonPoint leftWrist = skeleton.Joints[JointType.WristLeft].Position;
            SkeletonPoint rightWrist = skeleton.Joints[JointType.WristRight].Position;

            FormattedText bothCountText = new FormattedText(
                                        "Ambos: " + bothArms,
                                        CultureInfo.GetCultureInfo("es-ES"),
                                        FlowDirection.LeftToRight,
                                        new Typeface("Arial Black"),
                                        32,
                                        Brushes.Blue);

            FormattedText bothOkText = new FormattedText(
                                        "SIGUIENTE EJERCICIO!",
                                        CultureInfo.GetCultureInfo("es-ES"),
                                        FlowDirection.LeftToRight,
                                        new Typeface("Arial Black"),
                                        40,
                                        Brushes.Pink);

            FormattedText oneOkText = new FormattedText(
                                        "Perfecto! +1",
                                        CultureInfo.GetCultureInfo("es-ES"),
                                        FlowDirection.LeftToRight,
                                        new Typeface("Arial Black"),
                                        32,
                                        Brushes.Green);

            FormattedText infoExercise = new FormattedText(
                                        "Ejercicio 2. Eje YZ. Mano hacia arriba hasta hombros.",
                                        CultureInfo.GetCultureInfo("es-ES"),
                                        FlowDirection.LeftToRight,
                                        new Typeface("Arial Black"),
                                        32,
                                        Brushes.Green);

            Point textPoint = new Point(2, 2);
            //drawingContext.DrawText(infoExercise, textPoint);
            puntuacion.Text = infoExercise.Text;

            textPoint = new Point(240, 50);
            drawingContext.DrawText(bothCountText, textPoint);


            // Right Hand to Shoulder Exercise
            bool rightOk = RightHandToShoulderYZDetector.detection(skeleton);
            bool leftOk = LeftHandToShoulderYZDetector.detection(skeleton);
            if (rightOk && leftOk)
            {
                drawingContext.DrawText(bothOkText, SkeletonPointToScreen(head));
                bothArms++;
                rightArmSuccess++;
                leftArmSuccess++;
                nEjercicio = 0;
                LeftHandToShoulderYZDetector.resetDetecttion();
                RightHandToShoulderYZDetector.resetDetecttion();
            }
            else if (rightOk)
            {
                drawingContext.DrawText(oneOkText, SkeletonPointToScreen(rightShoulder));
                rightArmSuccess++;
            }
            else if (leftOk)
            {
                drawingContext.DrawText(oneOkText, SkeletonPointToScreen(leftShoulder));
                leftArmSuccess++;
            }

            // More info in the screen :)
            List<SkeletonPoint> goals = RightHandToShoulderYZDetector.updateGoals(skeleton);
            Brush red = new SolidColorBrush(Colors.Red);
            Brush green = new SolidColorBrush(Colors.Green);
            if (RightHandToShoulderYZDetector.status0())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, rightWrist, oneEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[0], oneEuro);
            }
            if (RightHandToShoulderYZDetector.status90())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, rightWrist, twoEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[1], twoEuro);
            }
            if (RightHandToShoulderYZDetector.status180())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, rightWrist, fiveEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[2], fiveEuro);
            }

            // Left Hand to Shoulder Exercise
            // More info in the screen :)
            goals = LeftHandToShoulderYZDetector.updateGoals(skeleton);
            if (LeftHandToShoulderYZDetector.status0())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, leftWrist, oneEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[0], oneEuro);
            }
            if (LeftHandToShoulderYZDetector.status90())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, leftWrist, oneEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[1], oneEuro);
            }
            if (LeftHandToShoulderYZDetector.status180())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, leftWrist, oneEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[2], oneEuro);
            }
        }

        /*
         Función que detecta y procesa el primer ejercicio en XY
         */
        private void ejercicioXY(Skeleton skeleton, DrawingContext drawingContext)
        {
            // This function will draw points and information about this exercise if the user must do it.
            // It will draw help points in the screen
            // it will detect moves
            SkeletonPoint head = skeleton.Joints[JointType.Head].Position;
            SkeletonPoint rightShoulder = skeleton.Joints[JointType.ShoulderRight].Position;
            SkeletonPoint leftShoulder = skeleton.Joints[JointType.ShoulderLeft].Position;
            SkeletonPoint leftWrist = skeleton.Joints[JointType.WristLeft].Position;
            SkeletonPoint rightWrist = skeleton.Joints[JointType.WristRight].Position;

            FormattedText bothCountText = new FormattedText(
                                        "Ambos: " + bothArms,
                                        CultureInfo.GetCultureInfo("es-ES"),
                                        FlowDirection.LeftToRight,
                                        new Typeface("Arial Black"),
                                        32,
                                        Brushes.Blue);

            FormattedText bothOkText = new FormattedText(
                                        "SIGUIENTE EJERCICIO!",
                                        CultureInfo.GetCultureInfo("es-ES"),
                                        FlowDirection.LeftToRight,
                                        new Typeface("Arial Black"),
                                        40,
                                        Brushes.Pink);

            FormattedText oneOkText = new FormattedText(
                                        "Perfecto! +1",
                                        CultureInfo.GetCultureInfo("es-ES"),
                                        FlowDirection.LeftToRight,
                                        new Typeface("Arial Black"),
                                        32,
                                        Brushes.Green);

            FormattedText infoExercise = new FormattedText(
                                        "Ejercicio 1. Eje XY. Mano hacia delante hasta hombros.",
                                        CultureInfo.GetCultureInfo("es-ES"),
                                        FlowDirection.LeftToRight,
                                        new Typeface("Arial Black"),
                                        32,
                                        Brushes.Green);

            Point textPoint = new Point(2, 2);
            //drawingContext.DrawText(infoExercise, textPoint);
            puntuacion.Text = infoExercise.Text;

            textPoint = new Point(240, 50);
            drawingContext.DrawText(bothCountText, textPoint);


            // Right Hand to Shoulder Exercise
            bool rightOk = RightHandToShoulderXYDetector.detection(skeleton);
            bool leftOk = LeftHandToShoulderXYDetector.detection(skeleton);
            if (rightOk && leftOk)
            {
                drawingContext.DrawText(bothOkText, SkeletonPointToScreen(head));
                bothArms++;
                rightArmSuccess++;
                leftArmSuccess++;
                nEjercicio = 1;
                LeftHandToShoulderXYDetector.resetDetecttion();
                RightHandToShoulderXYDetector.resetDetecttion();
            }
            else if (rightOk)
            {
                drawingContext.DrawText(oneOkText, SkeletonPointToScreen(rightShoulder));
                rightArmSuccess++;
            }
            else if (leftOk)
            {
                drawingContext.DrawText(oneOkText, SkeletonPointToScreen(leftShoulder));
                leftArmSuccess++;
            }

            // More info in the screen :)
            List<SkeletonPoint> goals = RightHandToShoulderXYDetector.updateGoals(skeleton);
            Brush red = new SolidColorBrush(Colors.Red);
            Brush green = new SolidColorBrush(Colors.Green);
            if (RightHandToShoulderXYDetector.status0())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, rightWrist, oneEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[0], oneEuro);
            }
            if (RightHandToShoulderXYDetector.status90())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, rightWrist, twoEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[1], twoEuro);
            }
            if (RightHandToShoulderXYDetector.status180())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, rightWrist, fiveEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[2], fiveEuro);
            }

            // Left Hand to Shoulder Exercise
            // More info in the screen :)
            goals = LeftHandToShoulderXYDetector.updateGoals(skeleton);
            if (LeftHandToShoulderXYDetector.status0())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, leftWrist, oneEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[0], oneEuro);
            }
            if (LeftHandToShoulderXYDetector.status90())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, leftWrist, oneEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[1], oneEuro);
            }
            if (LeftHandToShoulderXYDetector.status180())
            {
                drawHelpPoints(Brushes.Yellow, drawingContext, leftWrist, oneEuro);
            }
            else
            {
                drawHelpPoints(red, drawingContext, goals[2], oneEuro);
            }
        }

        /*
           Función que controla los sucesos del juego llamando a las
           funciones correspondientes:
           -Cuando el juego ha comenzado y ha pasado 1 min -> finisGame() 
           -Cuando el juego ha comenzado, por alguno de los dos eventos
           (pulsar el botón de inicio o posición con mano cerca de la cadera)
           -> detectStart(skeleton, drawingContext)
           -Cuando el nEjercicio es igual a 0 (1º mov) ->  ejercicioXY(skeleton, drawingContext)
           -Cuando el nEjercicio es igual a 1 (2º mov) -> ejercicioYZ(skeleton, drawingContext)
         */
        private void ejercicio(Skeleton skeleton, DrawingContext drawingContext)
        {
            // This function will proccess each skeleton call to show in the screen points wich depends on exercise.
            // if i have played 1 minute, it will finish the game.
            TimeSpan diff = DateTime.Now - startTime;
            statusBarText.Text = "Tiempo consumido: " + diff.Seconds;

            if (gameHasStarted && (diff.Minutes == 1))
                finishGame();
            else if (!gameHasStarted)
            {
                detectStart(skeleton, drawingContext);
            }
            if (nEjercicio == 0)
            {
                ejercicioXY(skeleton, drawingContext);
            }
            else if (nEjercicio == 1)
            {
                ejercicioYZ(skeleton, drawingContext);
            }
            
        }

        /*
           Función que detecta la posición inicial y llama a la función
           que da comienzo al juego -> startGame()
         */
        private void detectStart(Skeleton skeleton, DrawingContext drawingContext)
        {
            
            //GuiaEsqueleto.Stretch = Stretch.None;
            //GuiaEsqueleto.Margin = new Thickness(20);
            // This function will draw points and information about this exercise if the user must do it.
            // It will draw help points in the screen
            // it will detect moves
            //SkeletonPoint rightHip = skeleton.Joints[JointType.Head].Position;
            SkeletonPoint rightHip = skeleton.Joints[JointType.HipRight].Position;
            SkeletonPoint rightWrist = skeleton.Joints[JointType.WristRight].Position;

            bool detectS = ((rightHip.Z < (rightWrist.Z + 0.10)) && (rightHip.Z > (rightWrist.Z - 0.10))) &&
                            ((rightHip.Y < (rightWrist.Y + 0.10)) && (rightHip.Y > (rightWrist.Y - 0.10))) &&
                            ((rightHip.X < (rightWrist.X + 0.10)) && (rightHip.X > (rightWrist.X - 0.10)));

            if (detectS)
            {
                startGame(null, null);
            }
        }

        /*
           Función que actualiza los controles/feedback de la pantalla del juego.
         */
        private void startGame(object sender, RoutedEventArgs e)
        {
            // This function prepare all to start the game.
            // It shows the camera
            // It start the timer
            ImageV.Visibility = Visibility.Visible;
            Image2V.Visibility = Visibility.Visible;

            puntuacion.Visibility = Visibility.Visible;

            startButton.Visibility = Visibility.Hidden;

            medal.Visibility = Visibility.Hidden;
            imagen_Resultado.Visibility = Visibility.Hidden;
            Guia_Esqueleto.Visibility = Visibility.Hidden;
            resultStats.Visibility = Visibility.Hidden;

            startTime = DateTime.Now;
            gameHasStarted = true;

        }

        /*
           Función de fin de juego. Muestra los resultados por el 
           jugador.
         */
        private void finishGame()
        {
            // This function will finish the game.
            // It will hide camera's image
            // it will show user's result
            ImageV.Visibility = Visibility.Hidden;
            Image2V.Visibility = Visibility.Hidden;
            startButton.Visibility = Visibility.Hidden;

            resultStats.Visibility = Visibility.Visible;
            resultStats.Text = "Has realizado " + bothArms + " ejercicios correctamente!";
            resultStats.Text += "\nHas ganado " + bothArms * 8 * 2 + " Euros en total! ";
            resultStats.Text += "Y ahora.. Barcenas dará su veredicto y te llenará el\nsobre de acuerdo a tu habilidad.";

            if(bothArms<1)
            {
                resultStats.Text += "\nBarcenas te da: Un puñado de caramelos!!";
                medal.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Images/caramelos2.jpg", UriKind.Absolute));
                imagen_Resultado.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Images/bad.jpg", UriKind.Absolute));
            }
            else if (bothArms < 4)
            {
                resultStats.Text += "\nBarcenas te da: 100 €";
                medal.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Images/100.jpg", UriKind.Absolute));
                imagen_Resultado.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Images/good.jpg", UriKind.Absolute));
            }
            else if (bothArms >= 4 && bothArms < 8)
            {
                resultStats.Text += "\nBarcenas te da: 200 €";
                medal.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Images/200.jpg", UriKind.Absolute));
                imagen_Resultado.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Images/good.jpg", UriKind.Absolute));
            }
            else if (bothArms >= 8)
            {
                resultStats.Text += "\nBarcenas te da: 500 €!!";
                medal.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Images/500.jpg", UriKind.Absolute));
                imagen_Resultado.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Images/good.jpg", UriKind.Absolute));
            }

            medal.Visibility = Visibility.Visible;
            imagen_Resultado.Visibility = Visibility.Visible;
            //gameHasStarted = false;
        }
    }
}