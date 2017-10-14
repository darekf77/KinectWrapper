using Kinect_Wrapper.camera;
using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.ComponentModel;

namespace Kinect_Wrapper.device
{
    public partial class Device : IDevice, INotifyPropertyChanged
    {
        public KinectSensor sensor { get; private set; }
        public IKinectCamera camera { get; private set; }

        private DeviceType _deviceType;

        private String _filePath;

        private Audio _audio;
        private Video _video;
        private String _name;
        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public event EventHandler StateChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public Device(Audio audio, Video video, KinectSensor sensor_xbox360)
        {
            _audio = audio;
            _video = video;
            sensor = sensor_xbox360;
            _deviceType = DeviceType.KINECT_1;
            _name = "K360-" + sensor.UniqueKinectId;
        }

        public Device(Audio audio, Video video, String filePath)
        {
            _audio = audio;
            _video = video;
            _filePath = filePath;
            _deviceType = DeviceType.RECORD_FILE_KINECT_1;
            _name = "RF-" + System.IO.Path.GetFileName(_filePath);
            camera.init(this);
            camera.onReplayFinish += (e, v) =>
            {
                StateChanged?.Invoke(this, EventArgs.Empty);
            };

        }

        public Device(Audio audio, Video video)
        {
            _audio = audio;
            _video = video;
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
            if (_video != null) _video.update();
        }



    }
}
