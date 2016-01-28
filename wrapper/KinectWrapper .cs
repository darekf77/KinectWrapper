using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.helpers;
using Kinect_Wrapper.statistic;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.wrapper
{
    public partial class KinectWrapper: IKinectWrapper, INotifyPropertyChanged
    {
        #region singleton 
        private static IKinectWrapper _instance;
        static readonly object _locker = new object();
        public static IKinectWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new KinectWrapper();
                }
                return _instance;
            }
        }
        #endregion
        public Boolean AutoPickUpNewDevice { get; set; }
        private ObservableCollection<IDevice> _devices;
        private IDevice _currentDevice;
        private BackgroundWorker _worker;
        private Params _params = new Params();
        private IStream _stream;
        private IDevice _defaultDevice;
        private Statistics _statistic;
        private ObservableCollection<IStream> _streams = new ObservableCollection<IStream>();
        public ObservableCollection<InfoRow> Info { get; private set; }

        InfoRow _infoDeviceName = new InfoRow("Device Name", "");
        InfoRow _infoRightHand = new InfoRow("RightHandPos", "");
        InfoRow _infoLeftHand = new InfoRow("LeftHandPos", "");
        InfoRow _infoSpine = new InfoRow("SpinePos", "");
        InfoRow _infoIsSkeletonDetected = new InfoRow("IsSkeletonDetected", "");
        InfoRow _infoFramesPerSecond = new InfoRow("FPS", "");

        private KinectWrapper()
        {
            Grammar = new List<string>();
            UIEnable = true;
            _statistic = new Statistics();            
            Info = new TrulyObservableCollection<InfoRow>();
            Info.Add(_infoDeviceName);
            Info.Add(_infoRightHand);
            Info.Add(_infoLeftHand);
            Info.Add(_infoSpine);
            Info.Add(_infoIsSkeletonDetected);
            Info.Add(_infoFramesPerSecond);
            IStream color = new ColorStream();
            _streams.Add(color);
            _stream = color;
            IStream depth = new DepthStream();
            _streams.Add(depth);       
            AutoPickUpNewDevice = true;
            _devices = new ObservableCollection<IDevice>();
            _defaultDevice = new Device();
            prepareDevice(_defaultDevice);
            checkPotentialSensor();
            checkPotentialFiles();            
            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerAsync();
        }


        #region implementation
        // stop worker when replay from file on no current device
        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (_currentDevice != null)
                {                    
                    _currentDevice.update();
                    if (_currentDevice.Type == DeviceType.NO_DEVICE)
                    {
                        Thread.Sleep(1000);
                    }                    
                    continue;
                }
                lock (_locker)
                    while (_currentDevice == null || 
                        _currentDevice.Type == DeviceType.RECORD_FILE_KINECT_1)
                        Monitor.Wait(_locker);// thread is waiting until new data from kinect                 
            }
        }

        public void commit(IKinectFrame out_frame)
        {            
            if (DisplayImageReady != null)
            {
                _statistic.commitFrame(FrameType.NORMAL);
                DisplayImageReady(this, _stream.update(out_frame));
                _infoFramesPerSecond.Value = _statistic.FramesPerSecond(FrameType.NORMAL).ToString()+ "/s";
            }
        }

        public void commit(ImageSource out_frame)
        {
            if (DisplayImageReady != null)
            {
                _statistic.commitFrame(FrameType.NORMAL);
                DisplayImageReady(this, _stream.update(out_frame));
                _infoFramesPerSecond.Value = _statistic.FramesPerSecond(FrameType.NORMAL).ToString() + "/s";          
            }
        }

        void _currentDevice_RecordComplete(object sender, IDevice d)
        {
            _devices.Add(d);
        }
          
        public ObservableCollection<IDevice> Devices
        {
            get { return _devices; }
        }
        
        public IDevice Device
        {
            get { return _currentDevice; }
            set
            {
                _currentDevice = value;
                selectNewCurrentDevice(_currentDevice);
            }
        }

        public IStream Stream
        {
            get
            {
                return _stream;
            }
            set
            {
                _stream = value;
            }
        }

        public ObservableCollection<IStream> Streams
        {
            get
            {
                return _streams;
            }
            set
            {
                _streams = value;
            }
        }

        #endregion
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public event EventHandler<IKinectFrame> FrameReady;

        public event EventHandler<System.Windows.Media.ImageSource> DisplayImageReady;

        
        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public Boolean UIEnable { get; set; }


        public event EventHandler<IAudioMessage> NewUserMessageReady;
        
        public List<string> Grammar { get; set; }



    }
}
