using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kinect_Wrapper.frame;
using SharedLibJG.models;
using System.Drawing;

namespace Kinect_Wrapper.gestures
{
    public partial class GesturesDetector
    {

        public void triggerResizeGesture(float percentage = -1)
        {
            var res = (percentage == -1) ? calculateResizeGesture() : percentage;
            if (percentage >= 0 && percentage <= 1)
            {
                onResizeGesture?.Invoke(this, res);
            }
            else
            {
                Console.WriteLine("bad input gesture resize");
            }

        }


        private float calculateResizeGesture()
        {
            var head = Skeleton[SkeletonDataType.Head];
            var shoudlerCenter = Skeleton[SkeletonDataType.ShoulderCenter];
            var shoulderLeft = Skeleton[SkeletonDataType.ShoulderLeft];
            var shoulderRight = Skeleton[SkeletonDataType.ShoulderRight];
            var handLeft = Skeleton[SkeletonDataType.HandLeft];
            var handRight = Skeleton[SkeletonDataType.HandRight];
            var wristLeft = Skeleton[SkeletonDataType.WristLeft];
            var wristRight = Skeleton[SkeletonDataType.WristRight];
            var elbowLeft = Skeleton[SkeletonDataType.ElbowLeft];
            var elbowRight = Skeleton[SkeletonDataType.ElbowRight];
            return 0.0F;
        }

    }
}






