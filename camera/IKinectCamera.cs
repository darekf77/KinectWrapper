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

    public interface IKinectCameraOperator
    {
        void record(string toFile);
        void replay(string replayFile);
        void stop();
        void pause();
        CameraState State { get; set; }
    }



    public interface IKinectCamera : IKinectCameraOperator
    {
        void init(KinectSensor sensor);
        bool isRecordingPossible();

        event EventHandler onReplayFinish;
        event EventHandler<ReplayFrame> onFrameReady;

        ObservableCollection<IAudioRecordDevice> RecordingDevices { get; }

        void refreshAudioRecordingDevices(IDevice currentDevice);

        /// <summary>
        /// Update when recording
        /// </summary>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <param name="skeleton"></param>
        /// <param name="sensor"></param>
        void update(ColorImageFrame color, DepthImageFrame depth, SkeletonFrame skeleton, KinectSensor sensor);

        /// <summary>
        /// Update when replaying
        /// </summary>
        void update();

    }
}
