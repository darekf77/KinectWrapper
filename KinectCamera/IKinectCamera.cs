using Apex.MVVM;
using Kinect_Wrapper.camera.Replayer;
using Kinect_Wrapper.device;
using Kinect_Wrapper.frame;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Kinect_Wrapper.camera
{
    [Flags]
    public enum KinectRecordOptions
    {
        Frames = 1,
        Audio = 2,
        Everything = 3
    }



    public interface IKinectCamera : INotifyPropertyChanged
    {
        CameraState State { get; set; }
        IAudioRecorderReplayer Audio { get; }
        void update();
        IDevice CurrentDevice { get; set; }
        IDevice DeviceSelecteToPlay { get; set; }
        String ReplayFilePath { get; }
        String RecordFilePath { get; }


        // FRAME
        int MaxDepth { get; }
        int MinDepth { get; }

        // COMMANDS

        Command Record { get; }
        Command Play { get; }
        Command Stop { get; }
        Command Pause { get; }
        Command NextFrame { get; }
        Command PausePlay { get; }
        Command CancelRecord { get; }

        // FLAGS

        bool IsRecordingPossible { get; }
        bool IsRecording { get; }
        bool IsPaused { get; }
        bool IsStreaming { get; }

        // EVENTS
        event EventHandler onReplayEnd;
        event EventHandler onNoDeviceNeeded;
        event EventHandler onDeviceChanged;
        event EventHandler<String> RecordComplete;
        event EventHandler<IKinectFrame> FrameReady;

    }
}
