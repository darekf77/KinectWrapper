using Kinect_Wrapper.camera;
using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.devicemanager
{
    public partial class DeviceManager : IDeviceManager
    {
        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private IDevice DefaultDevice { get; set; }
        public ObservableCollection<IDevice> Devices { get; private set; }
        private IDevice CurrentDevice { get; set; }
        public IDevice SelectedDevice { get; set; }
        public IKinectCamera Camera { get; set; }

        public event EventHandler DeviceChanged;

        #region constructor
        public DeviceManager(IKinectCamera Camera)
        {
            this.Camera = Camera;
            Devices = new TrulyObservableCollection<IDevice>();
            DefaultDevice = new Device();
            Device = DefaultDevice;
            Device.start();

            #region load device from all sources
            loadSensorsFromSystem();
            loadReplayFilesInWorkspace();
            loadReplayFilesFromConfigFile();
            autopickupDeveic();
            #endregion

            #region add / remove sensor when plugin in / out
            KinectSensor.KinectSensors.StatusChanged += (e, v) =>
            {
                foreach (var device in Devices)
                {
                    if (device != null && device.sensor != null && device.sensor.UniqueKinectId.Equals(v.Sensor.UniqueKinectId)) return;
                }
                Devices.Add(new Device(v.Sensor));
            };
            #endregion

            #region after successfull recording add new device
            Camera.RecordComplete += (e, path) =>
            {
                Helpers.SetTimeout(() => // TODO QUICK_FIX
                {
                    if (App.Current != null) App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Devices.Add(new Device(path));
                    }));
                }, 2000);
            };
            #endregion

            #region save device list automaticly to file
            Devices.CollectionChanged += (e, v) =>
            {
                if (!isLoadingFiles) save();
            };
            #endregion

        }
        #endregion

        #region device
        public IDevice Device
        {
            get { return CurrentDevice; }
            set
            {
                if (value == null) return;
                if (CurrentDevice != null && CurrentDevice.Equals(value)) return;
                CurrentDevice = value;
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    DeviceChanged?.Invoke(this, EventArgs.Empty);
                    OnPropertyChanged("IsStopped");
                    //_infoDeviceName.Value = _currentDevice.Name;
                }));

            }
        }
        #endregion

        #region is stopped
        public bool IsStopped
        {
            get { return CurrentDevice.Equals(DefaultDevice); }
        }
        #endregion

        #region autopickup device
        public event EventHandler OnAutopickupDeviceChanged;
        public DeviceType AutopickupDeviceType
        {
            get { return _autopickupDeviceType; }
            set
            {
                _autopickupDeviceType = value;
            }
        }
        private DeviceType _autopickupDeviceType = DeviceType.NO_DEVICE;
        public bool AutopickupDevice
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


    }
}
