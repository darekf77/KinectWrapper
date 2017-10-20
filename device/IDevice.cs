using Kinect_Wrapper.camera;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Kinect_Wrapper.device
{
    public interface IDevice : INotifyPropertyChanged
    {
        void start(Action actionWhenReadyToPlay);

        void stop();


        void update(CameraState CameraState, bool isActive);

        KinectSensor sensor { get; }

        IKinectFrame nodeviceframe { get; }

        DeviceState State { get; }

        String Name { get; }

        String Path { get; }

        DeviceType Type { get; }

        event EventHandler StateChanged;

    }
}
