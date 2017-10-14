using Kinect_Wrapper.camera;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.ComponentModel;
using System.IO;

namespace Kinect_Wrapper.device
{
    public partial class Device : IDevice, INotifyPropertyChanged
    {
        public KinectSensor sensor { get; private set; }
        public IKinectCamera camera { get; private set; }

        private DeviceType _deviceType;

        private String _filePath;

        private String _name;
        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public event EventHandler StateChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public Device(KinectSensor sensor_xbox360)
        {
            sensor = sensor_xbox360;
            _deviceType = DeviceType.KINECT_1;
            _name = "K360-" + sensor.UniqueKinectId;
        }

        public Device(String filePath)
        {
            _filePath = filePath;
            _deviceType = DeviceType.RECORD_FILE_KINECT_1;
            _name = "RF-" + System.IO.Path.GetFileName(_filePath);
            camera.init(this);
            camera.onReplayFinish += (e, v) =>
            {
                StateChanged?.Invoke(this, EventArgs.Empty);
            };

        }

        public Device()
        {
            _deviceType = DeviceType.NO_DEVICE;
            _name = "Plug in your Kinect";
        }

        private Boolean _initializingDevice = false;

        public void start()
        {
            _initializingDevice = true;
            stop();
            _video.StreamingStarted += _video_StreamingStarted;
            if (Type == DeviceType.KINECT_1)
            {
                try
                {
                    sensor.Start();
                    sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    sensor.SkeletonStream.Enable();


                    _video.CurrentDevice = this;
                    _audio.CurrentDevice = this;
                }
                catch (Exception e)
                {
                    Console.WriteLine("cannot start kinect or already started" + e.Message);
                }
            }
            else
            if (Type == DeviceType.RECORD_FILE_KINECT_1)
            {
                _video.CurrentDevice = this;
                _audio.CurrentDevice = this;
            }
            if (Type == DeviceType.NO_DEVICE)
            {
                _video.CurrentDevice = this;
                _audio.CurrentDevice = this;
                _initializingDevice = false;
            }
        }



        private Boolean _stoppingDevice = false;
        public void stop()
        {
            _stoppingDevice = true;
            _video.StreamingStarted -= _video_StreamingStarted;
            _video.CurrentDevice = null;
            _audio.CurrentDevice = null;
            _stoppingDevice = false;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public string Name
        {
            get { return _name; }
        }

        public string Path
        {
            get
            {
                if (_deviceType == DeviceType.KINECT_1)
                {
                    return sensor.DeviceConnectionId;
                }
                return _filePath;
            }
        }

        public DeviceType Type
        {
            get { return _deviceType; }
        }


        public override string ToString()
        {
            return Name;
        }

        public void update()
        {
            if (camera != null) camera.update();
        }


        public IKinectFrame nodeviceframe
        {
            get
            {
                String exePath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                string dir = System.IO.Path.GetDirectoryName(exePath);
                var url = dir + "\\..\\..\\assets\\img\\nosignalw640.png";
                if (File.Exists(url))
                {
                    var bitmap = new System.Drawing.Bitmap(url);
                    return new KinectFrame(bitmap);
                }
                else
                {
                    throw new Exception("bad url: " + url);
                }
            }
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
