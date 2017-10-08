using Microsoft.Kinect;
using System;
using System.Collections.Generic;
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
        void record(string toFile);

        void replay(string replayFile);

        /// <summary>
        /// Stop recording or playing
        /// </summary>
        void stop();

        CameraState State { get; }

        void update(ColorImageFrame color, DepthImageFrame depth, SkeletonFrame skeleton, KinectSensor sensor);

    }
}
