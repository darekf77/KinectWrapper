using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kinect_Wrapper.frame;
using SharedLibJG.models;

namespace Kinect_Wrapper.gestures
{
    public partial class GesturesDetector 
    {

        bool detect(PlayerGestures gesture)
        {
            switch (gesture)
            {
                case PlayerGestures.BOTH_HAND_ABOVE_HEAD:
                    var left = Skeleton[SkeletonDataType.HandLeft];
                    var right = Skeleton[SkeletonDataType.HandRight];


                    break;
                case PlayerGestures.LEFT_HAND_ABOVE_HEAD:
                    break;
                case PlayerGestures.RIGHT_HAND_ABOVE_HEAD:
                    break;
                case PlayerGestures.SWIPE_LEFT:
                    break;
                case PlayerGestures.SWIPE_RIGHT:
                    break;
                case PlayerGestures.BOTH_HANDS_WIDE:
                    break;
                default:
                    break;
            }
            return false;
        }
        
    }
}






