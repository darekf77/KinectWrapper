using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Kinect_Wrapper.gestures;
using System.Runtime.CompilerServices;
using Kinect_Wrapper.camera;
using Kinect_Wrapper.frame;
using SharedLibJG.models;
using SharedLibJG.Helpers;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.devicemanager;

namespace Kinect_Wrapper.wrapper
{
    public partial class KinectWrapper : IKinectWrapper, INotifyPropertyChanged
    {
        public IKinectCamera Camera { get; private set; }
        public IDeviceManager Manager { get; set; }


        #region singleton 
        private static IKinectWrapper _instance;
        static readonly object _locker = new object();
        public static IKinectWrapper Instance(bool autopickupKinect = true)
        {

            if (_instance == null)
            {
                _instance = new KinectWrapper();
                _instance.Manager.AutopickupDevice = autopickupKinect;
            }
            return _instance;

        }
        #endregion

        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region constructor
        private KinectWrapper()
        {
            Camera = KinectCamera.Instance;
            Camera.FrameReady += Video_FramesReady;

            Manager = new DeviceManager(Camera);

            UIEnable = true;
            initStatistics();
            initStreams();
            initGestures();
        }
        #endregion

        #region gestures
        public IGesturesDetector Gestures { get; private set; }
        private void initGestures()
        {
            Gestures = new GesturesDetector();
            Camera.FrameReady += (e, frame) =>
            {
                if (Manager.Device.Type == DeviceType.KINECT_1)
                {
                    Gestures.update(frame);
                }
            };
            Gestures.start();
        }
        #endregion

        #region streams
        public event EventHandler<StreamBase> StreamChanged;
        private StreamBase _stream;
        private ObservableCollection<StreamBase> _streams = new ObservableCollection<StreamBase>();

        private void initStreams()
        {
            StreamBase color = new ColorStream();
            _streams.Add(color);
            Stream = color;
            StreamBase depth = new DepthStream();
            _streams.Add(depth);
        }

        public StreamBase Stream
        {
            get
            {
                return _stream;
            }
            set
            {
                var different = (_stream != null && !_stream.Equals(value));
                _stream = value;
                OnPropertyChanged("Stream");
                if (different && StreamChanged != null && value != null)
                {
                    StreamChanged(this, value);
                }
            }
        }

        public ObservableCollection<StreamBase> Streams
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

    }
    #endregion


}
