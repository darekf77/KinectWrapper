using Kinect_Wrapper.camera;
using Kinect_Wrapper.device.list;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Kinect_Wrapper.device
{
    public partial class Device
    {
        public static ObservableCollection<IDevice> Devices { get; set; }
        public static IDevice SelectedDevice { get; set; }
        public static DevicesList replaysList { get; set; }
        private static IDevice DefaultDevice { get; set; }

        public event EventHandler DeviceChanged;

        public static void init()
        {
            Devices = new TrulyObservableCollection<IDevice>();
            replaysList = new DevicesList(Devices);
            DefaultDevice = new Device();
            SelectedDevice = DefaultDevice;
            SelectedDevice.start();
            checkPotentialSensor();
            checkPotentialFiles();
            autopickupDeveic();
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
        }

        private static void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            foreach (var device in Devices)
            {
                if (device != null && device.sensor != null && device.sensor.UniqueKinectId.Equals(e.Sensor.UniqueKinectId)) return;
            }
            Devices.Add(new Device(e.Sensor));
        }

        static readonly object _locker = new object();
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

        #region is stopped
        public static bool IsStopped
        {
            get { return SelectedDevice.Equals(DefaultDevice); }
        }
        #endregion

        #region check potential files
        private static void checkPotentialFiles()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string[] potentialReplayfiles = Directory.GetFiles(currentDirectory, "*.replay", SearchOption.AllDirectories);
            foreach (var file in potentialReplayfiles)
            {
                FileInfo f = new FileInfo(file);
                if (f.Length == 0) continue;
                Devices.Add(new Device(file));
            }
        }
        #endregion

        #region check potential sensors
        private static void checkPotentialSensor()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor == null) continue;
                Devices.Add(new Device(potentialSensor));
            }
        }
        #endregion

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
        private static void autopickupDeveic()
        {
            if (!AutoPickUpFirstKinect) return;
            var kinectFounded = false;
            foreach (var device in Devices)
            {
                if (device.Type == DeviceType.KINECT_1)
                {
                    kinectFounded = true;
                    AutopickupDeviceType = DeviceType.KINECT_1;
                    SelectedDevice = device;
                    SelectedDevice.start();
                    break;
                }
            }
            if (!kinectFounded)
            {
                foreach (var device in Devices)
                {
                    if (device.Type == DeviceType.RECORD_FILE_KINECT_1)
                    {
                        AutopickupDeviceType = DeviceType.RECORD_FILE_KINECT_1;
                        SelectedDevice = device;
                        SelectedDevice.start();
                        break;
                    }
                }
            }
            OnAutopickupDeviceChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

    }
}
