using Kinect_Wrapper.frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apex.MVVM;

namespace Kinect_Wrapper.gestures
{
    public enum GesturesDetectorState
    {
        UNACTIVE,
        ACTIVE
    }


    public interface IGesturesDetector : IGesturesDetectorVM
    {
        string LastDetectedGesture { get; }

        GesturesDetectorState State { get; }

        void start();

        void stop();

        void trigger(PlayerGestures gesture);
        void triggerResizeGesture(float percentage);

        void update(IKinectFrame frame);

        event EventHandler<PlayerGestures> onGesture;
        event EventHandler<float> onResizeGesture;


    }

    public interface IGesturesDetectorVM
    {
        Command BothHandsUp { get; }

        Command Aeroplane { get; }

        Command SwipeLeft { get; }

        Command SwipeRight { get; }

        Command RightHandAboveHead { get; }

        Command LeftHandAboveHead { get; }

    }

}
