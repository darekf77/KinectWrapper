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
        public Video()
        {
            IsPaused = false;
        }        
        
        public bool IsPaused { get; private set; }
        
        public void pausePlay()
        {
            IsPaused = !IsPaused;
            if (_device.replay != null)
            {
                _device.replay.setPauseMode(IsPaused);
            }
        }
        
        
        private Boolean _isNextFrameSensor = false;

        public void nextFrame()
        {
            if (_device.Type == DeviceType.RECORD_FILE_KINECT_1)
            {
                _device.replay.nextFrame();
                IsPaused = true;
            }
            else if (_device.Type == DeviceType.KINECT_1)
            {
                IsPaused = true;
                _isNextFrameSensor = true;
            }
        }    

        public int Width
        {
            get
            {
                return 640;
            }
        }

        public int Height
        {
            get
            {
                return 480;
            }
        }

        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        
        
        public event EventHandler<IKinectFrame> FrameReady;

        public event PropertyChangedEventHandler PropertyChanged;

        

    }
}
