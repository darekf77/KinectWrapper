using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio.message;
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
        private IDevice _defaultDevice;
        private ObservableCollection<IDevice> _devices;
        private IDevice _currentDevice;
        public IDevice SelectedDevice { get; set; }

        public static Boolean AutoPickUpFirstKinect = false;

        private void initDevices()
        {
            _devices = new ObservableCollection<IDevice>();
            _defaultDevice = new Device(Audio, Video);
            Device = _defaultDevice;
            Device.start();
            checkPotentialSensor();
            checkPotentialFiles();
        }
        
        private void checkPotentialFiles()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string[] potentialReplayfiles = Directory.GetFiles(currentDirectory, "*.replay", SearchOption.AllDirectories);
            foreach (var file in potentialReplayfiles)
            {
                FileInfo f = new FileInfo(file);
                if (f.Length == 0) continue;
                _devices.Add(new Device(Audio,Video,file));
            }
        }

        private void checkPotentialSensor()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor == null) continue;
                _devices.Add(new Device(Audio, Video, potentialSensor));
            }
        }
                

        public bool DeviceNotAvailable
        {
            get { return _currentDevice.Equals(_defaultDevice); }
        }


        public ObservableCollection<IDevice> Devices
        {
            get { return _devices; }
        }

        public IDevice Device
        {
            get { return _currentDevice; }
            set
            {
                if (value == null) return;  
                if(_currentDevice!=null) _currentDevice.StateChanged -= _currentDevice_StateChanged;
                _currentDevice = value;
                if (_currentDevice != null) _currentDevice.StateChanged += _currentDevice_StateChanged;                                
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _infoDeviceName.Value = _currentDevice.Name;
                }));                
            }
        }

        private void _currentDevice_StateChanged(object sender, EventArgs e)
        {
            //if (_currentDevice.Type != DeviceType.RECORD_FILE_KINECT_1)
            //{
            lock (_locker)
            {
                Monitor.Pulse(_locker);
            }
            
            //}
        }
    }
}
