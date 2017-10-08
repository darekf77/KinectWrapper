using Kinect_Wrapper.device;
using Kinect_Wrapper.device.list;
using Kinect_Wrapper.structures;
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
        private DevicesList replaysList;
        private Device _defaultDevice;
        private ObservableCollection<Device> _devices;
        private Device _currentDevice;
        public Device SelectedDevice { get; set; }

        #region autopickup devoce
        public event EventHandler OnAutopickupDeviceChanged;
        public DeviceType AutopickupDeviceType
        {
            get { return _autopickupDeviceType; }
            private set
            {
                _autopickupDeviceType = value;
            }
        }
        private DeviceType _autopickupDeviceType = DeviceType.NO_DEVICE;
        private static bool AutopickupDevice
        {
            get
            {
                return AutoPickUpFirstKinect;
            }
            set
            {
                if (!AutoPickUpFirstKinectIsSet)
                {
                    AutoPickUpFirstKinect = value;
                    AutoPickUpFirstKinectIsSet = true;
                    Console.WriteLine("[KinectWrapper] - device autopickup already set");
                }
            }
        }

        private static Boolean AutoPickUpFirstKinect = true;
        private static Boolean AutoPickUpFirstKinectIsSet = false;
        private void autopickupDeveic()
        {
            if (!AutoPickUpFirstKinect) return;
            var kinectFounded = false;
            foreach (var device in Devices)
            {
                if (device.Type == structures.DeviceType.KINECT_1)
                {
                    kinectFounded = true;
                    AutopickupDeviceType = DeviceType.KINECT_1;
                    Device = device;
                    Device.start();
                    break;
                }
            }
            if (!kinectFounded)
            {
                foreach (var device in Devices)
                {
                    if (device.Type == structures.DeviceType.RECORD_FILE_KINECT_1)
                    {
                        AutopickupDeviceType = DeviceType.RECORD_FILE_KINECT_1;
                        Device = device;
                        Device.start();
                        break;
                    }
                }
            }
            OnAutopickupDeviceChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        private void initDevices()
        {
            _devices = new TrulyObservableCollection<Device>();
            replaysList = new DevicesList(Devices, Audio, Video);
            _defaultDevice = new Device(Audio, Video);
            Device = _defaultDevice;
            Device.start();
            checkPotentialSensor();
            checkPotentialFiles();
            autopickupDeveic();
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged; ;
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            foreach (var device in Devices)
            {
                if (device != null && device.sensor != null && device.sensor.UniqueKinectId.Equals(e.Sensor.UniqueKinectId)) return;
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
                _devices.Add(new Device(Audio, Video, file));
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

        public ObservableCollection<Device> Devices
        {
            get { return _devices; }
        }

        public event EventHandler DeviceChanged;

        public Device Device
        {
            get { return _currentDevice; }
            set
            {
                if (value == null) return;
                if (_currentDevice != null && _currentDevice.Equals(value)) return;
                if (_currentDevice != null) _currentDevice.StateChanged -= _currentDevice_StateChanged;
                _currentDevice = value;
                if (_currentDevice != null) _currentDevice.StateChanged += _currentDevice_StateChanged;
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _infoDeviceName.Value = _currentDevice.Name;
                }));
                if (DeviceChanged != null) DeviceChanged(this, EventArgs.Empty);
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
