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
            if (res >= 0 && res <= 1)
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
            var handLeft = Skeleton[SkeletonDataType.HandLeft];
            var handRight = Skeleton[SkeletonDataType.HandRight];
            var shoulderLeft = Skeleton[SkeletonDataType.ShoulderLeft];
            var shoulderRight = Skeleton[SkeletonDataType.ShoulderRight];
            var wristLeft = Skeleton[SkeletonDataType.WristLeft];
            var wristRight = Skeleton[SkeletonDataType.WristRight];

            var maxS = Math.Max(shoulderLeft.X, shoulderRight.X);
            var minS = Math.Min(shoulderLeft.X, shoulderRight.X);
            var s = Math.Abs(maxS - minS) * 2; /// QUICK_FIX


            var maxW = Math.Max(wristLeft.X, wristRight.X);
            var minW = Math.Min(wristLeft.X, wristRight.X);
            var w = Math.Abs(maxW - minW);

            var res = (w * 100) / s;

            if (res < 50) res = 50;
            if (res > 100) res = 100;
            res -= 50;
            res *= 2;
            //Console.WriteLine("res: " + res);

            return ((float)res / 100F);
        }

    }
}






