﻿using System;
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
        #region group athmetic function
        enum Operator
        {
            LowerYThan,
            LowerXThan,
            GreaterYThan,
            GreaterXThan
        }


        bool check(int value, Operator op, params Point[] points)
        {

            foreach (var p in points)
            {
                switch (op)
                {
                    case Operator.LowerYThan:
                        if (value >= p.Y) return false;
                        break;
                    case Operator.LowerXThan:
                        if (value >= p.X) return false;
                        break;
                    case Operator.GreaterYThan:
                        if (value < p.Y) return false;
                        break;
                    case Operator.GreaterXThan:
                        if (value < p.X) return false;
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        #endregion

        bool checkSkeleton(PlayerGestures gesture)
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

            switch (gesture)
            {
                case PlayerGestures.BOTH_HAND_ABOVE_HEAD:
                    if (check(head.Y, Operator.GreaterYThan, handLeft, handRight, wristLeft, wristRight)) return true;
                    break;
                case PlayerGestures.LEFT_HAND_ABOVE_HEAD:
                    if (check(head.Y, Operator.GreaterYThan, handLeft, wristLeft) &&
                        check(head.Y, Operator.LowerYThan, handRight, wristRight)) return true;
                    break;
                case PlayerGestures.RIGHT_HAND_ABOVE_HEAD:
                    if (check(head.Y, Operator.LowerYThan, handLeft, wristLeft) &&
                        check(head.Y, Operator.GreaterYThan, handRight, wristRight)) return true;
                    break;
                case PlayerGestures.SWIPE_LEFT:
                    if (check(shoudlerCenter.X, Operator.GreaterXThan, handLeft, wristLeft)) return true;
                    break;
                case PlayerGestures.SWIPE_RIGHT:
                    if (check(shoudlerCenter.X, Operator.LowerXThan, handRight, wristRight)) return true;
                    break;
                case PlayerGestures.BOTH_HANDS_WIDE:
                    if (
                        check(head.Y, Operator.LowerYThan, handLeft, handRight, elbowLeft, elbowRight, wristLeft, wristRight) &&
                        check(shoulderLeft.Y, Operator.GreaterYThan, handLeft, handRight, elbowLeft, elbowRight, wristLeft, wristRight) &&
                        check(shoulderRight.Y, Operator.GreaterYThan, handLeft, handRight, elbowLeft, elbowRight, wristLeft, wristRight))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

    }
}






