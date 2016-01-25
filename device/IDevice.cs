using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kinect_Wrapper.device
{
    public interface IDevice
    {
        /// <summary>
        /// update device data from input
        /// </summary>
        void update();

        /// <summary>
        /// Check device state
        /// </summary>
        DeviceState State { get; }
        
        /// <summary>
        /// Device name
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Path to device
        /// </summary>
        String Path { get; }


        /// <summary>
        /// Type of device
        /// </summary>
        DeviceType Type { get; }

        /// <summary>
        /// Start device
        /// </summary>
        void start();

        /// <summary>
        /// Stop kinect device
        /// </summary>
        void stop();        

        /// <summary>
        /// Audio stream configuration / information
        /// </summary>
        IAudio Audio { get; }

        /// <summary>
        /// Video stram configuration / information
        /// </summary>
        IVideo Video { get; }

        /// <summary>
        /// Event when something happen to device
        /// </summary>
        event EventHandler<DeviceState> StateChanged;

         
        

    }
}
