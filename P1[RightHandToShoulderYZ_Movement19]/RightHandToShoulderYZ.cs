using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;

public class RightHandToShoulderYZ
{
    private Skeleton skeleton;

    private detected180 = false;
    private detected90 = false;
    private detected0 = false;

    public RightHandToShoulderYZ()
	{
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
