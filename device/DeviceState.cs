using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.IO;

namespace Kinect_Wrapper.device
{
    public partial class Device
    {

        private void Replay_ReplayFinished()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void _video_StreamingStarted(object sender, EventArgs e)
        {
            if (_initializingDevice) _initializingDevice = false;
        }

        private DeviceState resolveState()
        {
            if (_initializingDevice) return DeviceState.INITIALIZING;
            if (_stoppingDevice) return DeviceState.STOPPING;

            if (_deviceType == DeviceType.KINECT_1)
            {
                if (sensor == null) return DeviceState.NOT_READY;
                if (sensor.Status != KinectStatus.Connected) return DeviceState.NOT_READY;
            }
            else if (_deviceType == DeviceType.RECORD_FILE_KINECT_1)
            {
                if (_filePath == null) return DeviceState.NOT_READY;
                if (_filePath.Length == 0) return DeviceState.NOT_READY;
                var recordFile = new FileInfo(_filePath);
                if (recordFile.Length == 0) return DeviceState.NOT_READY;
                if (!recordFile.Exists) return DeviceState.NOT_READY;
            }
            else if (_deviceType == DeviceType.NO_DEVICE)
            {
                return DeviceState.PLAYING;
            }

            if (_video.CurrentDevice != null && _video.CurrentDevice.Equals(this))
            {
                if (_video.IsRecording) return DeviceState.RECORDING;
                if (_video.IsStreaming) return DeviceState.PLAYING;
            }

            return DeviceState.READY;
        }

        private DeviceState _state = DeviceState.READY;
        public DeviceState State
        {
            get
            {
                var newState = resolveState();
                if (newState != _state)
                {
                    _state = newState;
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        OnPropertyChanged("State");
                    }));
                }
                return _state;
            }
        }

    }
}
