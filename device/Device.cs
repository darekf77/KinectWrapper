using Kinect_Wrapper.camera;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Kinect_Wrapper.device
{
    public partial class Device : IDevice
    {
        public KinectSensor sensor { get; private set; }
        public DeviceType Type { get; private set; }
        private CameraState CameraState { get; set; }

        public event EventHandler StateChanged;

        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region constructors
        #region constructor sensor xbox360
        public Device(KinectSensor sensor_xbox360)
        {
            sensor = sensor_xbox360;
            Type = DeviceType.KINECT_1;
            Name = KinectSensor.KinectSensors.IndexOf(sensor_xbox360) + "-kinect-xb360";
            stop();
        }
        #endregion

        #region costructor replay file
        public Device(String filePath)
        {
            _filePath = filePath;
            Type = DeviceType.RECORD_FILE_KINECT_1;
            Name = "replay-" + System.IO.Path.GetFileName(_filePath);
        }
        #endregion

        #region constructor empty device
        public Device()
        {
            Type = DeviceType.NO_DEVICE;
            Name = "Plug in your Kinect";
        }
        #endregion
        #endregion

        public bool IsReadyToPlay
        {
            get
            {
                return (State == DeviceState.READY);
            }
        }

        #region Path
        private String _filePath;
        public string Path
        {
            get
            {
                if (Type == DeviceType.KINECT_1)
                {
                    return sensor.DeviceConnectionId;
                }
                return _filePath;
            }
        }
        #endregion

        #region name
        private String _name;
        public string Name
        {
            get { return _name; }
            private set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        #endregion

        #region state
        private DeviceState _state = DeviceState.NOT_READY;
        public DeviceState State
        {
            get
            {
                return _state;
            }
            set
            {
                var current = _state;

                if (current != value)
                {
                    _state = value;
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        OnPropertyChanged("State");
                        StateChanged?.Invoke(this, EventArgs.Empty);
                    }));
                }

            }
        }
        #endregion

        #region start
        public void start(Action actionWhenReadyToPlay)
        {
            State = DeviceState.INITIALIZING;
            switch (Type)
            {
                case DeviceType.NO_DEVICE:
                    State = DeviceState.READY;
                    if (actionWhenReadyToPlay != null) actionWhenReadyToPlay.Invoke();
                    break;
                case DeviceType.KINECT_1:
                    startKinect(() =>
                    {
                        State = DeviceState.READY;
                        if (actionWhenReadyToPlay != null) actionWhenReadyToPlay.Invoke();
                    });
                    break;
                case DeviceType.RECORD_FILE_KINECT_1:
                    State = DeviceState.READY;
                    if (actionWhenReadyToPlay != null) actionWhenReadyToPlay.Invoke();
                    break;
            }
        }

        private void startKinect(Action action)
        {
            if (sensor == null || sensor.Status == KinectStatus.Disconnected) return;
            if (sensor.IsRunning)
            {
                stop();
                //Helpers.SetTimeout(() =>
                //{
                //    startKinect(action);
                //}, 1000);
            }
            try
            {
                sensor.Start();
                sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                sensor.SkeletonStream.Enable();
                action.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine("cannot start kinect or already started" + e.Message);
            }
        }
        #endregion

        #region stop
        bool stopNextTime = false;
        public void stop()
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
            stopNextTime = true;
        }
        #endregion

        #region override to string
        public override string ToString()
        {
            return Name;
        }
        #endregion

        #region no deivce frame
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
        #endregion

        #region try restart kienct
        void tryRestartKinect(Action actionAfterRestart)
        {
            stop();
            Helpers.SetTimeout(() =>
            {
                start(() =>
                {
                    State = DeviceState.READY;
                });
            });
        }
        #endregion

        #region update 
        public void update(CameraState CameraState, bool isActive)
        {
            if (stopNextTime)
            {
                stopNextTime = false; ;
                KinectCamera.Instance.Stop.DoExecute();
                return;
            }
            this.CameraState = CameraState;
            switch (Type)
            {
                case DeviceType.NO_DEVICE:
                    State = DeviceState.READY;
                    break;
                case DeviceType.KINECT_1:
                    if (sensor == null) State = DeviceState.NOT_READY;
                    else if (isActive && State == DeviceState.RESTARTING && (sensor.Status != KinectStatus.Connected)) break;
                    else if (sensor.Status == KinectStatus.Initializing) State = DeviceState.INITIALIZING;
                    else if (sensor.Status == KinectStatus.Connected)
                    {
                        if (isActive && CameraState.PLAYING.EnumGroupRange().isInside((int)CameraState))
                        {
                            State = DeviceState.PLAYING;
                        }
                        else if (isActive && CameraState.RECORDING.EnumGroupRange().isInside((int)CameraState))
                        {
                            State = DeviceState.RECORDING;
                        }
                        else
                        {
                            State = DeviceState.READY;
                        }
                    }
                    else if (isActive && sensor.Status == KinectStatus.Error)
                    {
                        State = DeviceState.RESTARTING;
                        KinectCamera.Instance.Pause.DoExecute();
                        tryRestartKinect(() =>
                        {
                            KinectCamera.Instance.PausePlay.DoExecute();
                        });
                    }
                    else
                    {
                        State = DeviceState.NOT_READY;
                    }
                    break;
                case DeviceType.RECORD_FILE_KINECT_1:

                    if (!File.Exists(Path))
                    {
                        State = DeviceState.NOT_READY;
                    }
                    else if (isActive && CameraState.PLAYING.EnumGroupRange().isInside((int)CameraState))
                    {
                        State = DeviceState.PLAYING;
                    }
                    else if (isActive && CameraState.RECORDING.EnumGroupRange().isInside((int)CameraState))
                    {
                        State = DeviceState.RECORDING;
                    }
                    else
                    {
                        State = DeviceState.READY;
                    }
                    break;
            }
        }
        #endregion
    }
}
