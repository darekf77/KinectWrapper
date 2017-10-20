using Apex.MVVM;
using Kinect_Wrapper.camera;
using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using Microsoft.Win32;
using SharedLibJG.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
        public IKinectCamera Camera { get; set; }
        public event EventHandler DeviceChanged;

        #region constructor
        public DeviceManager(IKinectCamera Camera)
        {
            this.Camera = Camera;
            Devices = new TrulyObservableCollection<IDevice>();
            DefaultDevice = new Device();
            Camera.DeviceSelecteToPlay = DefaultDevice;

            #region is repaly or sensor set state to unactive - set no device to work
            Camera.onNoDeviceNeeded += (e, v) =>
            {
                Camera.DeviceSelecteToPlay = DefaultDevice;
                Camera.Play.DoExecute();
            };
            #endregion

            #region load device from all sources            
            loadSensorsFromSystem();
            loadReplayFilesInWorkspace();
            loadReplayFilesFromConfigFile();
            #endregion

            //autopickupDeveic();

            #region add / remove sensor when plugin in / out
            KinectSensor.KinectSensors.StatusChanged += (e, v) =>
            {
                #region add new sensor if isn't on list
                var sensors = KinectSensor.KinectSensors;
                foreach (var sensor in sensors)
                {
                    var dev = Devices.First(s => { return (s != null & s.sensor != null & s.sensor.UniqueKinectId == sensor.UniqueKinectId); });
                    if (dev == null) Devices.Add(new Device(sensor));
                }
                #endregion

                #region remove sensor is is not available in system
                List<IDevice> toRemove = new List<IDevice>();
                foreach (var device in Devices)
                {
                    if (device != null && device.sensor != null)
                    {
                        var sensor = sensors.First(s => { return (s != null && s.UniqueKinectId == device.sensor.UniqueKinectId); });
                        if (sensor == null) toRemove.Add(device);
                    }
                }
                for (int i = 0; i < toRemove.Count; i++)
                {
                    Devices.Remove(toRemove[i]);
                }
                #endregion
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
                if (!isLoadingFiles) saveDevicesToConfigFile();
            };
            #endregion

            Camera.Play.DoExecute();

            Camera.onDeviceChanged += (e, v) =>
            {
                lock (_lockerWorkerState)
                {
                    Monitor.PulseAll(_lockerWorkerState);
                }
                lock (_lockerWorkerUpdate)
                {
                    Monitor.PulseAll(_lockerWorkerUpdate);
                }
                DeviceChanged?.Invoke(this, EventArgs.Empty);
            };

            initWorkers();
        }
        #endregion

        #region add from harddrive to list
        public Command AddFromHardDrive
        {
            get
            {
                return new Command(() =>
                {
                    var openFileDialog = new OpenFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };
                    if (openFileDialog.ShowDialog() != true) return;
                    Devices.Add(new Device(openFileDialog.FileName));
                });
            }
        }
        #endregion

        #region remove from devices
        public Command RemoveFromDevices
        {
            get
            {
                return new Command(() =>
                {
                    MessageBoxResult messageBoxResult =
                      MessageBox.Show("Are you sure?", "Delete device from list",
                      MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                        Devices.Remove(SelectedDevice);
                });
            }
        }
        #endregion

        #region is selcted device
        public bool IsSelectedDevice
        {
            get
            {
                return (SelectedDevice != null);
            }
        }
        #endregion

        #region selcted device
        public IDevice SelectedDevice
        {
            get { return Camera.DeviceSelecteToPlay; }
            set
            {
                Camera.DeviceSelecteToPlay = value;
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OnPropertyChanged("IsStopped");
                    OnPropertyChanged("IsSelectedDevice");
                }));
            }
        }
        #endregion

        #region is stopped
        public bool IsStopped
        {
            get { return Camera.CurrentDevice.Equals(DefaultDevice); }
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
                    Camera.DeviceSelecteToPlay = device;
                    Camera.Play.DoExecute();
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
                        Camera.DeviceSelecteToPlay = device;
                        Camera.Play.DoExecute();
                        break;
                    }
                }
            }
            OnAutopickupDeviceChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion


    }
}
