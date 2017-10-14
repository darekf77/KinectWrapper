using Kinect_Wrapper.camera;
using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kinect_Wrapper.device
{
    public interface IDevice
    {
        void start();

        void stop();

        void update();

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
