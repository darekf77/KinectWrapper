using Kinect.Replay.Record;
using Kinect.Replay.Replay;
using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.device
{
    public partial class Device :IDevice
    {
        public event EventHandler<IDevice> RecordComplete;
        private KinectSensor _sensor;
        private DeviceType _deviceType;
        private String _filePath;        
        private KinectReplay _replay;
        private IAudio _audio;
        private Video _video;
        private String _name;
      

        public Device(KinectSensor sensor_xbox360)
        {
            _sensor = sensor_xbox360;
            _deviceType = DeviceType.KINECT_1;
            _name = "KinectX360-" + _sensor.UniqueKinectId;            
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged; //TODO what to do
            if (_sensor.Status == KinectStatus.Connected)
            {
                _video = new Video(this, _sensor);
                _audio = new Audio(this, _sensor);
            }
        }
                
        public Device(String filePath) {
            _filePath = filePath;
            _deviceType = DeviceType.RECORD_FILE_KINECT_1;
            _name = "RecordFile-" + System.IO.Path.GetFileName(_filePath);
            _replay = new KinectReplay(_filePath);
            _video = new Video(this, _replay);
            _audio = new Audio(this,_filePath);
        }

        public Device()
        {
            _deviceType = DeviceType.NO_DEVICE;
            _name = "Plug in your Kinect";
            _video = new Video(this);
            _audio = new Audio(this);
        }
                

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status == KinectStatus.Connected && _video != null)
            {
                _video = new Video(this, _sensor);
                _audio = new Audio(this, _sensor);
            }
        }

        #region implementation

        public void start()
        {
            try
            {
                if (_deviceType == DeviceType.KINECT_1)
                {
                    if (_sensor != null)
                    {
                        try
                        {
                            _sensor.Start();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("cannot start kinect or already started"+ e.Message);
                        }
                        
                        if (_video == null) _video = new Video(this, _sensor);
                    }
                    
                }
                else if (_deviceType == DeviceType.RECORD_FILE_KINECT_1)
                {
                    if (_replay != null) _replay.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        public void stop()
        {
            if (_video != null)
            {
                if(_video.IsRecording) _video.stopRecord();
            }
            if (_deviceType == DeviceType.KINECT_1)
            {
                if (_sensor != null) _sensor.Stop();
            }
            else if(_deviceType == DeviceType.RECORD_FILE_KINECT_1)
            {
                if (_replay != null) _replay.Stop();
            }
            if (StateChanged!=null)
                StateChanged(this, State);
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
                    return _sensor.DeviceConnectionId;
                }
                return _filePath;
            }
        }

        public Kinect_Wrapper.structures.DeviceType Type
        {
            get { return _deviceType; }
        }
                
        public audio.IAudio Audio
        {
            get { return _audio; }
        }

        public video.IVideo Video
        {
            get { return _video; }
        }

        public override string ToString()
        {
            return "Device from list";
        }

        public void update()
        {
            if (_video != null) _video.update();
        }

        #endregion

        public event EventHandler<DeviceState> StateChanged;



    }
}
