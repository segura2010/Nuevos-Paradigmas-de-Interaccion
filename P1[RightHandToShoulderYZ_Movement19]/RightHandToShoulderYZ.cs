using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{

    public class RightHandToShoulderYZ
    {
        private Skeleton skeleton;

        private bool detected180;
        private bool detected90;
        private bool detected0;

        public RightHandToShoulderYZ(Skeleton s)
	    {
            detected180 = false;
            detected90 = false;
            detected0 = false;

            skeleton = s;
	    }

        public bool detection()
        { // Idea: to search for elbow's angle .
            // 1º-> 180º (recto)
            // 2º -> 90º (L)
            // 3º -> 0º -> Movimiento completado.
            // Using points to get vectors and vectors to get angles

            Joint shoulder = skeleton.Joints[JointType.ShoulderRight];
            Joint elbow = skeleton.Joints[JointType.ElbowRight];
            Joint wrist = sekeleton.Joints[JointType.WristRight];
            Joint hand = sekeleton.Joints[JointType.HandRight];

            return false;
        }
    }

}