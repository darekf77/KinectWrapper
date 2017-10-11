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
        RECORDING
    }



    public interface IKinectCamera
    {
        void init(KinectSensor sensor);

        void record(string toFile);
        bool isRecordingPossible();

        void replay(string replayFile);

        void stop();
        void pause();

        ObservableCollection<IAudioRecordDevice> RecordingDevices { get; }

        void refreshAudioRecordingDevices(IDevice currentDevice);

        CameraState State { get; }

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
