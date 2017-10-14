using Apex.MVVM;
using Kinect_Wrapper.camera.Replayer;
using Kinect_Wrapper.device;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera
{
    public enum CameraState
    {
        UNACTIVE,
        PLAYING,
        PLAYING_PAUSE,
        RECORDING,
        RECORDING_PAUSE
    }


    [Flags]
    public enum KinectRecordOptions
    {
        Frames = 1,
        Audio = 2,
        Everything = 3
    }



    public interface IKinectCamera
    {
        void record(string toFile);
        void replay(string replayFile);
        Command Stop { get; }
        Command Pause { get; }
        CameraState State { get; set; }

        IAudioRecorderReplayer Audio { get; }

        void init(IDevice device);
        bool isRecordingPossible();

        event EventHandler onReplayFinish;
        event EventHandler<ReplayFrame> onFrameReady;


        /// <summary>
        /// Update when recording
        /// </summary>
        void update(ColorImageFrame color, DepthImageFrame depth, SkeletonFrame skeleton, KinectSensor sensor);

        /// <summary>
        /// Update when replaying
        /// </summary>
        void update();

    }
}
