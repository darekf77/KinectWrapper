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

        bool isEmpty(params Point[] points)
        {
            foreach (var p in points)
            {
                if (p.IsEmpty) return true;
            }
            return false;
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
                case PlayerGestures.RESIZING:
                    if (
                        !isEmpty(shoudlerCenter, shoulderLeft, shoulderRight,
                        handLeft, handRight, elbowLeft, elbowRight, wristLeft, wristRight) &&

                        check(shoulderLeft.Y, Operator.GreaterYThan, elbowLeft, wristLeft, handLeft) &&
                        check(shoulderRight.Y, Operator.GreaterYThan, elbowRight, wristRight, handRight) &&

                        check(elbowLeft.X, Operator.LowerXThan, wristLeft, handLeft) &&
                        check(elbowRight.X, Operator.GreaterXThan, wristRight, handRight)
                        )
                    {
                        return true;
                    }
                    break;
                case PlayerGestures.HANDS_ABOVE_HEAD:
                    if (!isEmpty(head, handLeft, handRight, wristLeft, wristRight) &&
                        check(head.Y, Operator.GreaterYThan, handLeft, handRight, wristLeft, wristRight) &&
                        check(elbowLeft.X, Operator.GreaterXThan, wristLeft, handLeft) &&
                        check(elbowRight.X, Operator.LowerXThan, wristRight, handRight)
                        ) return true;
                    break;
                case PlayerGestures.HANDS_AEROPLAN:
                    if (!isEmpty(head, shoulderLeft, shoulderRight, handLeft, handRight, elbowLeft, elbowRight, wristLeft, wristRight) &&
                        check(head.Y, Operator.LowerYThan, handLeft, handRight, elbowLeft, elbowRight, wristLeft, wristRight) &&
                        check(shoulderLeft.Y, Operator.GreaterYThan, handLeft, handRight, elbowLeft, elbowRight, wristLeft, wristRight) &&
                        check(shoulderRight.Y, Operator.GreaterYThan, handLeft, handRight, elbowLeft, elbowRight, wristLeft, wristRight) &&
                        check(elbowLeft.X, Operator.GreaterXThan, wristLeft, handLeft) &&
                        check(elbowRight.X, Operator.LowerXThan, wristRight, handRight))
                    {
                        return true;
                    }
                    break;
                case PlayerGestures.LEFT_HAND_ABOVE_HEAD:
                    if (!isEmpty(head, handLeft, handRight, wristLeft, wristRight) &&
                        check(head.Y, Operator.GreaterYThan, handLeft, wristLeft) &&
                        check(head.Y, Operator.LowerYThan, handRight, wristRight)) return true;
                    break;
                case PlayerGestures.RIGHT_HAND_ABOVE_HEAD:
                    if (!isEmpty(head, handLeft, handRight, wristLeft, wristRight) &&
                        check(head.Y, Operator.LowerYThan, handLeft, wristLeft) &&
                        check(head.Y, Operator.GreaterYThan, handRight, wristRight)) return true;
                    break;
                case PlayerGestures.SWIPE_LEFT:
                    if (!isEmpty(shoudlerCenter, handLeft, wristLeft, handRight, wristRight) &&
                        check(shoudlerCenter.X, Operator.GreaterXThan, handLeft, wristLeft) &&
                        check(shoudlerCenter.X, Operator.GreaterXThan, handRight, wristRight)
                        ) return true;
                    break;
                case PlayerGestures.SWIPE_RIGHT:
                    if (!isEmpty(shoudlerCenter, handRight, wristRight, handLeft, wristLeft) &&
                        check(shoudlerCenter.X, Operator.LowerXThan, handRight, wristRight) &&
                        check(shoudlerCenter.X, Operator.LowerXThan, handLeft, wristLeft)
                        ) return true;
                    break;

                default:
                    break;
            }
            return false;
        }

    }
}






