using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kinect_Wrapper.frame;
using Apex.MVVM;
using System.Threading;

namespace Kinect_Wrapper.gestures
{


    public partial class GesturesDetector
    {

        public Command BothHandsUp
        {
            get
            {
                return new Command(() =>
                {
                    trigger(PlayerGestures.BOTH_HAND_ABOVE_HEAD);
                });
            }
        }





        public Command Aeroplane
        {
            get
            {
                return new Command(() =>
                {
                    trigger(PlayerGestures.BOTH_HANDS_WIDE);
                });
            }
        }




        public Command SwipeLeft
        {
            get
            {
                return new Command(() =>
                {
                    trigger(PlayerGestures.SWIPE_LEFT);
                });
            }
        }



        public Command SwipeRight
        {
            get
            {
                return new Command(() =>
                {
                    trigger(PlayerGestures.SWIPE_RIGHT);
                });
            }
        }




        public Command RightHandAboveHead
        {
            get
            {
                return new Command(() =>
                {
                    trigger(PlayerGestures.RIGHT_HAND_ABOVE_HEAD);
                });
            }
        }




        public Command LeftHandAboveHead
        {
            get
            {
                return new Command(() =>
                {
                    trigger(PlayerGestures.LEFT_HAND_ABOVE_HEAD);
                });
            }
        }






    }
}





