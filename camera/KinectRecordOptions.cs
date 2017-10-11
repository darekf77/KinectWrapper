using System;

namespace Kinect_Wrapper.camera
{
    [Flags]
    public enum KinectRecordOptions
    {
        Frames = 1,
        Audio = 2,
        Everything = 3
    }
}