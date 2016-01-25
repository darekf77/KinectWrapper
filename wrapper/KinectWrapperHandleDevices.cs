using Kinect_Wrapper.device;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Controls;

namespace Kinect_Wrapper.wrapper
{
    public partial class KinectWrapper
    {
        void _currentDevice_StateChanged(object sender, DeviceState e)
        {
            prepareDevice(_defaultDevice);
            Console.WriteLine("Current device stopped, no signal ");
            lock (_locker)
                Monitor.Pulse(_locker);
        }

        private void prepareDevice(IDevice device) {
            
            if (_currentDevice != null)
            {
                _currentDevice.StateChanged -= _currentDevice_StateChanged;
                if (_currentDevice.Video != null)
                {
                    _currentDevice.Video.RecordComplete -= _currentDevice_RecordComplete;
                    _currentDevice.Video.FramesReady -= Video_FramesReady;
                }
            }
            _currentDevice = device;
            _currentDevice.StateChanged += _currentDevice_StateChanged;
            if (_currentDevice.Video != null)
            {
                _currentDevice.Video.RecordComplete += _currentDevice_RecordComplete;
                _currentDevice.Video.FramesReady += Video_FramesReady;
            }
            _infoDeviceName.Value = _currentDevice.Name;            
        }


        private void selectNewCurrentDevice(IDevice selectedDevice)
        {
            foreach (IDevice device in _devices)
            {
                if (selectedDevice != null && !device.Equals(selectedDevice)) continue;
                prepareDevice(device);
                return;
            }
        }

        void Video_FramesReady(object sender, IKinectFrame e)
        {
            if (UIEnable && e.IsSkeletonDetected)
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _infoLeftHand.Value = e.Users[0].Skeleton.LeftHand.ToString();
                    _infoRightHand.Value = e.Users[0].Skeleton.RigthHand.ToString();
                    _infoSpine.Value = e.Users[0].Skeleton.Spine.ToString();
                    _infoIsSkeletonDetected.Value = e.IsSkeletonDetected.ToString();
                }));            
            }
            if (FrameReady != null)  FrameReady(this, e);             
        }

        private void checkPotentialFiles()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string[] potentialReplayfiles = Directory.GetFiles(currentDirectory, "*.replay", SearchOption.AllDirectories);
            foreach (var file in potentialReplayfiles)
            {
                FileInfo f = new FileInfo(file);
                if (f.Length == 0) continue;
                _devices.Add(new Device(file));
            }
        }

        private void checkPotentialSensor()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor == null) continue;
                _devices.Add(new Device(potentialSensor));
            }
        }


    }
}
