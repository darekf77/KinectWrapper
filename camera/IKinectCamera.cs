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

        void refreshRecordingDevices(IDevice currentDevice);

        CameraState State { get; }

        void update(ColorImageFrame color, DepthImageFrame depth, SkeletonFrame skeleton, KinectSensor sensor);

    }
}
