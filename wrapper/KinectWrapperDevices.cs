using Kinect_Wrapper.device;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace Kinect_Wrapper.wrapper
{
    public partial class KinectWrapper
    {
        private DeviceBase _defaultDevice;
        private ObservableCollection<DeviceBase> _devices;
        private DeviceBase _currentDevice;
        public DeviceBase SelectedDevice { get; set; }

        public static Boolean AutoPickUpFirstKinect = false;

        private void initDevices()
        {
            _devices = new TrulyObservableCollection<DeviceBase>();
            _defaultDevice = new Device(Audio, Video);
            Device = _defaultDevice;
            Device.start();
            checkPotentialSensor();
            checkPotentialFiles();
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged; ;
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            foreach (var device in Devices)
            {
                if (device.sensor.UniqueKinectId.Equals(e.Sensor.UniqueKinectId)) return;
            }
            Devices.Add(new Device(Audio, Video, e.Sensor));
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
        
        public bool IsStopped
        {
            get { return _currentDevice.Equals(_defaultDevice); }
        }
        
        public ObservableCollection<DeviceBase> Devices
        {
            get { return _devices; }
        }

        public event EventHandler DeviceChanged;

        public DeviceBase Device
        {
            get { return _currentDevice; }
            set
            {
                if (value == null) return;
                if (_currentDevice != null && _currentDevice.Equals(value)) return;
                if(_currentDevice!=null) _currentDevice.StateChanged -= _currentDevice_StateChanged;
                _currentDevice = value;
                if (_currentDevice != null) _currentDevice.StateChanged += _currentDevice_StateChanged;                                
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _infoDeviceName.Value = _currentDevice.Name;
                }));
                if(DeviceChanged != null) DeviceChanged(this, EventArgs.Empty);
                OnPropertyChanged("IsStopped");
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
