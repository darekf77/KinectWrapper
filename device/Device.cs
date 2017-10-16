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
        public IKinectCamera camera { get; private set; }
        public DeviceType Type { get; private set; }

        public event EventHandler StateChanged;

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
        }
        #endregion

        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region constructor sensor xbox360
        public Device(KinectSensor sensor_xbox360)
        {
            sensor = sensor_xbox360;
            Type = DeviceType.KINECT_1;
            _name = "K360-" + sensor.UniqueKinectId;
        }
        #endregion

        #region costructor replay file
        public Device(String filePath)
        {
            _filePath = filePath;
            Type = DeviceType.RECORD_FILE_KINECT_1;
            _name = "RF-" + System.IO.Path.GetFileName(_filePath);
            camera.init(this);
            camera.onReplayFinish += (e, v) =>
            {
                StateChanged?.Invoke(this, EventArgs.Empty);
            };
        }
        #endregion

        #region constructor empty device
        public Device()
        {
            Type = DeviceType.NO_DEVICE;
            _name = "Plug in your Kinect";
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

        void updateState()
        {
            switch (Type)
            {
                case DeviceType.NO_DEVICE:
                    State = DeviceState.READY;
                    break;
                case DeviceType.KINECT_1:
                    if (sensor == null) State = DeviceState.NOT_READY;
                    else if (State == DeviceState.RESTARTING && (sensor.Status != KinectStatus.Connected)) break;
                    else if (sensor.Status == KinectStatus.Initializing) State = DeviceState.INITIALIZING;
                    else if (sensor.Status == KinectStatus.Connected)
                    {
                        if (CameraState.PLAYING.EnumGroupRange().isInside((int)camera.State))
                        {
                            State = DeviceState.PLAYING;
                        }
                        else if (CameraState.RECORDING.EnumGroupRange().isInside((int)camera.State))
                        {
                            State = DeviceState.RECORDING;
                        }
                        else //  if (camera.State == CameraState.UNACTIVE) // is obvous
                        {
                            State = DeviceState.READY;
                        }
                    }
                    else if (sensor.Status == KinectStatus.Error)
                    {
                        State = DeviceState.RESTARTING;
                        tryRestartKinect();
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
                    else if (CameraState.PLAYING.EnumGroupRange().isInside((int)camera.State))
                    {
                        State = DeviceState.PLAYING;
                    }
                    else if (CameraState.RECORDING.EnumGroupRange().isInside((int)camera.State))
                    {
                        State = DeviceState.RECORDING;
                    }
                    else //  if (camera.State == CameraState.UNACTIVE) // is obvous
                    {
                        State = DeviceState.READY;
                    }

                    break;
                default:
                    break;
            }
        }




        #endregion

        #region start
        public void start()
        {
            State = DeviceState.INITIALIZING;

            switch (Type)
            {
                case DeviceType.NO_DEVICE:
                    break;
                case DeviceType.KINECT_1:
                    startKinect();
                    break;
                case DeviceType.RECORD_FILE_KINECT_1:
                    break;
                default:
                    break;
            }


            //stop();
            //_video.StreamingStarted += _video_StreamingStarted;
            //if (Type == DeviceType.KINECT_1)
            //{
            //    try
            //    {
            //        sensor.Start();
            //        sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            //        sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            //        sensor.SkeletonStream.Enable();


            //        _video.CurrentDevice = this;
            //        _audio.CurrentDevice = this;
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("cannot start kinect or already started" + e.Message);
            //    }
            //}
            //else
            //if (Type == DeviceType.RECORD_FILE_KINECT_1)
            //{
            //    _video.CurrentDevice = this;
            //    _audio.CurrentDevice = this;
            //}
            //if (Type == DeviceType.NO_DEVICE)
            //{
            //    _video.CurrentDevice = this;
            //    _audio.CurrentDevice = this;
            //    _initializingDevice = false;
            //}
        }

        private void startKinect()
        {
            if (sensor == null || sensor.Status == KinectStatus.Disconnected) return;
            if (sensor.IsRunning)
            {
                stopKinect();
                Helpers.SetTimeout(() =>
                {
                    startKinect();
                }, 1000);
            }
            try
            {
                sensor.Start();
                sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                sensor.SkeletonStream.Enable();
                State = DeviceState.READY;
            }
            catch (Exception e)
            {
                Console.WriteLine("cannot start kinect or already started" + e.Message);
            }
        }

        private void stopKinect()
        {
            try
            {
                sensor.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        #endregion

        #region stop
        public void stop()
        {
            State = DeviceState.STOPPING;
            camera.Stop.DoExecute();
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

        private void _video_StreamingStarted(object sender, EventArgs e)
        {
            //if (_initializingDevice) _initializingDevice = false;
        }


        void tryRestartKinect()
        {
            sensor.Stop();

        }

        #region update 
        public void update(DeviceUpdateType type)
        {
            switch (type)
            {
                case DeviceUpdateType.FRAMES:
                    camera.update();
                    break;
                case DeviceUpdateType.STATE:
                    updateState();
                    break;
                case DeviceUpdateType.ALL:
                    updateState();
                    camera.update();
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
