using Kinect.Replay.Record;
using Kinect.Replay.Replay;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.device.video
{
    public partial class Video:IVideo,INotifyPropertyChanged
    {
        private static readonly object _locker = new object();
        
        private KinectSensor _sensor;
        private IDevice _device;
        private IKinectFrame _frame;
        KinectReplay _replay;
        private KinectRecorder _recorder;
        private RecordMode _recordMode = RecordMode.NO_AUDIO;
        private Boolean _isStreaming = false;
        private Boolean _isStoppingRecorder = false;
 
        void intit()
        {
            IsStreaming = false;
            IsRecording = false;
            IsPaused = false;
        }

        public Video(IDevice device,KinectSensor sensor)
        {
            intit();
            _device = device;
            _sensor = sensor;            
            _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            _sensor.SkeletonStream.Enable();
            _frame = new KinectFrame(sensor);
        }

        public Video(IDevice device, KinectReplay replay)
        {
            intit();
            _device = device;
            _replay = replay;
            _frame = new KinectFrame(replay);
            _replay.AllFramesReady += ReplayAllFramesReady;
            _replay.ReplayFinished += _replay_ReplayFinished;
        }

        public Video(IDevice device)
        {
            intit();
            _device = device;
            String exePath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            string dir = Path.GetDirectoryName(exePath);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            bi.UriSource = new Uri(dir + @"\..\..\assets\img\nosignalw640.png", UriKind.Relative);
            bi.EndInit();
            ///TODO scale bitmap
            //var bitmapScale = new TransformedBitmap(bi,
            //     new ScaleTransform(640 / bi.Width,
            //                        480 / bi.Height));
            _frame = new KinectFrame(BitmapImage2Bitmap(bi));  
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            Console.WriteLine(bitmapImage);
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        
        
        public bool IsPaused { get; private set; }

        
        public void pausePlay()
        {
            IsPaused = !IsPaused;
            if (_replay != null)
            {
                _replay.setPauseMode(IsPaused);
            }
        }

        private Boolean _isNextFrameSensor = false;
        public void nextFrame()
        {
            if (_device.Type == DeviceType.RECORD_FILE_KINECT_1)
            {
                _replay.nextFrame();
                IsPaused = true;
            }
            else if (_device.Type == DeviceType.KINECT_1)
            {
                IsPaused = true;
                _isNextFrameSensor = true;
            }
        }

        

        public void update()
        {
            if (_device.Type == DeviceType.KINECT_1)
            {
                IsStreaming = updateFrames();
                if (ReadyFrame != null)
                {
                    ReadyFrame(this, _frame);
                }
            }
            else if (_device.Type == DeviceType.RECORD_FILE_KINECT_1)
            {
                IsStreaming = (_replay.Started && !_replay.IsFinished);
            }
            else if (_device.Type == DeviceType.NO_DEVICE)
            {
                IsStreaming = false;
                updateFramesNoDevice();
            }
        }

        
        public bool IsStreaming {
            get 
            {
                return _isStreaming;
            }
            set 
            {
                _isStreaming = value;
                OnPropertyChanged("IsStreaming");
            }
        }
        
      

        public RecordMode RecordingMode
        {
            get
            {
                return _recordMode;
            }
            set
            {
                _recordMode = value;
            }
        }

        public bool IsRecording { get; private set; }

        public event EventHandler<IKinectFrame> ReadyFrame;

        public event EventHandler<IDevice> RecordComplete;

        
        public event EventHandler<IKinectFrame> FramesReady;

        public event PropertyChangedEventHandler PropertyChanged;

        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}
