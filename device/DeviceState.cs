using Kinect.Replay.Record;
using Kinect.Replay.Replay;
using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Kinect_Wrapper.device
{
    public partial class Device
    {
        public DeviceState State
        {
            get
            {
                if (_deviceType == DeviceType.KINECT_1)
                {
                    if (_sensor == null) return DeviceState.NOT_READY;
                    if (_sensor.Status != KinectStatus.Connected) return DeviceState.NOT_READY;

                }
                else if (_deviceType == DeviceType.RECORD_FILE_KINECT_1)
                {
                    if (_filePath == null) return DeviceState.NOT_READY;
                    if (_filePath.Length == 0) return DeviceState.NOT_READY;
                    FileInfo recordFile = new FileInfo(_filePath);
                    if (recordFile.Length == 0) return DeviceState.NOT_READY;
                    if (!recordFile.Exists) return DeviceState.NOT_READY;
                }
                if (_video != null && _video.IsStreaming) return DeviceState.IS_WORKING_PLAYING;
                if (_video != null && _video.IsRecording) return DeviceState.IS_WORKING_RECORDING;
                return DeviceState.IS_READY_NOT_WORKING;
            }
        }

    }
}
