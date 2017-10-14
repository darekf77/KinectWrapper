using Kinect_Wrapper.camera;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kinect_Wrapper.device
{
    public enum DeviceUpdateType
    {
        FRAMES,
        STATE,
        ALL
    }



    public interface IDevice
    {
        void start();

        void stop();

        void update(DeviceUpdateType type);

        KinectSensor sensor { get; }

        IKinectCamera camera { get; }

        IKinectFrame nodeviceframe { get; }

        DeviceState State { get; }

        String Name { get; }

        String Path { get; }

        DeviceType Type { get; }

        event EventHandler StateChanged;

    }
}
