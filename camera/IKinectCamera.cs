using Apex.MVVM;
using Kinect_Wrapper.camera.Replayer;
using Kinect_Wrapper.device;
using Kinect_Wrapper.frame;
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
        PLAYING_STOPPING,
        RECORDING,
        RECORDING_PAUSE,
        RECORDING_STOPPING,
        RECORDING_CANCEL
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
        CameraState State { get; set; }
        IAudioRecorderReplayer Audio { get; }
        void init(IDevice device);
        void update();

        // FRAME
        int MaxDepth { get; }
        int MinDepth { get; }

        // COMMANDS

        Command Stop { get; }
        Command Pause { get; }
        Command NextFrame { get; }
        Command PausePlay { get; }
        Command CancelRecord { get; }

        // FLAGS

        bool IsRecordingPossible { get; }
        bool IsRecording { get; }
        bool IsPaused { get; }

        // EVENTS
        event EventHandler onReplayFinish;
        event EventHandler<String> RecordComplete;
        event EventHandler<IKinectFrame> FrameReady;

    }
}
