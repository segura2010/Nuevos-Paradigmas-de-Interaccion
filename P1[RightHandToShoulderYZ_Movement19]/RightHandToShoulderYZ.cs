using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// IMPORTANT! If you are not using "Microsoft.Samples.Kinect.SkeletonBasics" namespace, you must chage it!
namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    public class RightHandToShoulderYZ
    {
        // Class usage example in "drawBone" function!
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
            // 1º-> 180º (lined/right)
            //      O
            //   -- | --
            // 2º -> 90º (L)
            //      O
            //     L|--
            //
            // 3º -> 0º -> Movement completed!
            // Using points to get vectors and vectors to get angles
            // IMPORTANT: 
            //      X -> width
            //      Y -> height
            //      Z -> depth
            skeleton = s;

            // Get all important points..
            Joint shoulder = skeleton.Joints[JointType.ShoulderRight];
            Joint elbow = skeleton.Joints[JointType.ElbowRight];
            Joint wrist = skeleton.Joints[JointType.WristRight];
            Joint hand = skeleton.Joints[JointType.HandRight];

            // Preparing vectors to get key angle
            myPoint vShoulderElbow = new myPoint();
            myPoint vElbowWrist = new myPoint();
            vShoulderElbow = pointsToVector(shoulder, elbow);
            vElbowWrist = pointsToVector(elbow, wrist);
            keyAngle = calcAngleXY(vShoulderElbow, vElbowWrist); // Calculate key angle between vectors..

            // I need shoulder and elbow in the same height (more or less)
            // and same depth between wrist and elbow! Dont cheat!!
            similarPos = similarY(shoulder, elbow) && similarZ(elbow, wrist); 

            if (similarPos)
            {   // Only if i have shoulder and elbow in the same height i am doing the correct movement
                detected180 = (similarAngle(keyAngle, 180) || detected180);
                detected90 = (similarAngle(keyAngle, 90) || detected90);
                detected0 = (similarAngle(keyAngle, 0) || detected0); // Be careful! I cannot detect 180º, 0º and 90º at the same time! So, I must "remember" older detections! -> Using "|| detected0"
            }
            else
                detected0 = detected90 = detected180 = false;

            return (detected180 && detected90 && detected0);
        }

        private myPoint pointsToVector(Joint p1, Joint p2)
        {   // This function convert two points to a vector
            myPoint v = new myPoint();
            v.x = p1.Position.X - p2.Position.X;
            v.y = p1.Position.Y - p2.Position.Y;
            v.z = p1.Position.Z - p2.Position.Z;

            return v;
        }

        private double calcAngleXY(myPoint v1, myPoint v2)
        {   // This functions calculates the angle between two vectors.
            double cosin = (v1.x * v2.x) + (v1.y * v2.y);
            double sum1 = Math.Sqrt((v1.x * v1.x) + (v1.y * v1.y));
            double sum2 = Math.Sqrt((v2.x * v2.x) + (v2.y * v2.y));
            cosin = cosin / (sum1 * sum2);
            double angle = Math.Acos(cosin);
            angle = angle * (180 / Math.PI); // To convert to degrees!
            return angle;
        }

        private bool similarY(Joint p1, Joint p2)
        {   // I want to know if two points are at the same height
            return ((p1.Position.Y < (p2.Position.Y + 0.05)) && (p1.Position.Y > (p2.Position.Y - 0.05)));
        }

        private bool similarZ(Joint p1, Joint p2)
        {   // I want to know if two points are at the same depth. I need a little more error rate..
            return ((p1.Position.Z < (p2.Position.Z + 0.10)) && (p1.Position.Z > (p2.Position.Z - 0.10)));
        }

        private bool similarAngle(double alpha, double beta)
        {   // This function sais if alpha is between beta+20 and beta-20 ->  beta-20 < alpha < beta+20
            // it is used to detect 0º, 90º and 180º angles.
            return ((alpha < (beta + 30)) && (alpha > (beta - 30)));
        }

        public bool myZones(JointType p1, JointType p2)
        {   // This function return true if joints types are both zones that this detection's class must detect
            // You could use it to know if you should draw pens with special colors..
            bool p1IsZone = (p1.Equals(JointType.ShoulderRight) || p1.Equals(JointType.ElbowRight) || p1.Equals(JointType.WristRight) || p1.Equals(JointType.HandRight));
            bool p2IsZone = (p2.Equals(JointType.ShoulderRight) || p2.Equals(JointType.ElbowRight) || p2.Equals(JointType.WristRight) || p2.Equals(JointType.HandRight));

            return (p1IsZone && p2IsZone);
        }

        // Some useful consultors.. :)
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
        public bool statusHeight()
        {
            return similarPos;
        }
        public double getKeyAngle()
        {
            return keyAngle;
        }

    }

    // Useful struct to work with 3D points
    public struct myPoint
    {
        public double x;
        public double y;
        public double z;
    }
}
