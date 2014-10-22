//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;



        // Move detection object!!
        private RightHandToShoulderYZ RightHandToShoulderYZDetector = new RightHandToShoulderYZ();



        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
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
            Image.Source = this.imageSource;

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
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

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
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
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

            // Move detection usage!
            // I create new "Pen" in green or red to show if move is correct or not.
            Pen correctMove = new Pen(Brushes.Green, 5);
            Pen incorrectMove = new Pen(Brushes.Red, 5);
            if (RightHandToShoulderYZDetector.myZones(jointType0, jointType1))
            {   // if this zones are involved in my deteccion, I will draw it with 
                // special colors.
                if (RightHandToShoulderYZDetector.detection(skeleton))
                {
                    drawPen = correctMove;
                    Console.WriteLine("DETECTED!!");
                }
                else
                {
                    drawPen = incorrectMove;
                    Console.WriteLine("not detected..");
                }
                // More info
                if(RightHandToShoulderYZDetector.status0())
                    status0.Foreground = new SolidColorBrush(Colors.Green);
                else
                    status0.Foreground = new SolidColorBrush(Colors.Red);
                if (RightHandToShoulderYZDetector.status180())
                    status180.Foreground = new SolidColorBrush(Colors.Green);
                else
                    status180.Foreground = new SolidColorBrush(Colors.Red);
                if (RightHandToShoulderYZDetector.status90())
                    status90.Foreground = new SolidColorBrush(Colors.Green);
                else
                    status90.Foreground = new SolidColorBrush(Colors.Red);
                if (RightHandToShoulderYZDetector.statusWeight())
                    statusWeight.Foreground = new SolidColorBrush(Colors.Green);
                else
                    statusWeight.Foreground = new SolidColorBrush(Colors.Red);
                keyAngle.Text = "KeyAngle: "+ RightHandToShoulderYZDetector.getKeyAngle()+"º";
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// <summary>
        /// Handles the checking or unchecking of the seated mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != this.sensor)
            {
                if (this.checkBoxSeatedMode.IsChecked.GetValueOrDefault())
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                }
            }
        }
    }







    public class RightHandToShoulderYZ
    {
        private Skeleton skeleton;

        private bool detected180;
        private bool detected90;
        private bool detected0;
        private bool similarPos;
        private double keyAngle;

        public RightHandToShoulderYZ()
        {
            detected180 = false;
            detected90 = false;
            detected0 = false;
            similarPos = false;

            //skeleton = s;
        }

        public bool detection(Skeleton s)
        { // Idea: to search for elbow's angle .
            // 1º-> 180º (recto)
            // 2º -> 90º (L)
            // 3º -> 0º -> Movimiento completado.
            // Using points to get vectors and vectors to get angles
            skeleton = s;

            Joint shoulder = skeleton.Joints[JointType.ShoulderRight];
            Joint elbow = skeleton.Joints[JointType.ElbowRight];
            Joint wrist = skeleton.Joints[JointType.WristRight];
            Joint hand = skeleton.Joints[JointType.HandRight];

            myPoint vShoulderElbow = new myPoint();
            myPoint vElbowWrist = new myPoint();
            vShoulderElbow = pointsToVector(shoulder, elbow);
            vElbowWrist = pointsToVector(elbow, wrist);
            keyAngle = calcAngleYZ(vShoulderElbow, vElbowWrist);

            similarPos = similarY(shoulder, elbow); // I need shoulder and elbow in the same height (more or less)

            if (similarPos)
            {
                detected180 = (similarAngle(keyAngle, 180) || detected180);
                detected90 = (similarAngle(keyAngle, 90) || detected90);
                detected0 = (similarAngle(keyAngle, 0) || detected0); // Be careful! I cant detect 180, 0 and 90 at the same time! So I must "remember" older detections! -> Using "|| detected0"
            }
            else
                detected0 = detected90 = detected180 = false;

            return (detected180 && detected90 && detected0);
        }

        private myPoint pointsToVector(Joint p1, Joint p2)
        {
            myPoint v = new myPoint();
            v.x = p1.Position.X - p2.Position.X;
            v.y = p1.Position.Y - p2.Position.Y;
            v.z = p1.Position.Z - p2.Position.Z;

            return v;
        }

        private double calcAngleYZ(myPoint v1, myPoint v2)
        {
            double cosin = (v1.y * v2.y) + (v1.z * v2.z);
            double sum1 = Math.Sqrt((v1.y * v1.y) + (v1.z * v1.z));
            double sum2 = Math.Sqrt((v2.y * v2.y) + (v2.z * v2.z));
            cosin = cosin / (sum1*sum2);
            double angle = Math.Acos(cosin);
            angle = angle * (180 / Math.PI); // Convert to degrees!
            return angle;
        }

        private bool similarY(Joint p1, Joint p2)
        {
            return ((p1.Position.Y < (p2.Position.Y + 2)) && (p1.Position.Y > (p2.Position.Y - 2)));
        }

        private bool similarAngle(double alpha, double beta)
        {
            return ((alpha < (beta + 10)) && (alpha > (beta - 10)));
        }

        public bool myZones(JointType p1, JointType p2)
        {   // This function return true if joints types are both zones that this detection's class must detect
            // You could use it to know if you should draw pens with special colors..
            bool p1IsZone = (p1.Equals(JointType.ShoulderRight) || p1.Equals(JointType.ElbowRight) || p1.Equals(JointType.WristRight) || p1.Equals(JointType.HandRight));
            bool p2IsZone = (p2.Equals(JointType.ShoulderRight) || p2.Equals(JointType.ElbowRight) || p2.Equals(JointType.WristRight) || p2.Equals(JointType.HandRight));

            return (p1IsZone && p2IsZone);
        }

        public bool status180()
        {
            return detected180;
        }
        public bool status0()
        {
            return detected0;
        }
        public bool status90()
        {
            return detected90;
        }
        public bool statusWeight()
        {
            return similarPos;
        }
        public double getKeyAngle()
        {
            return keyAngle;
        }
    }

    public struct myPoint
    {
        public double x;
        public double y;
        public double z;
    }

}