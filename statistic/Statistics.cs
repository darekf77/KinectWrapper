using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kinect_Wrapper.statistic
{
    class Frame
    {
        int _perSecond = 0;
        int _frameRate = 0;
        public void commit() {
            Interlocked.Increment(ref _perSecond);
        }
        public int getFrameRate() { return _frameRate; }
        public void reset()
        {
            _frameRate = _perSecond;
            _perSecond = 0;
        }
    }

    public class Statistics : IStatistics, INotifyPropertyChanged
    {
        Dictionary<FrameType, Frame> _frames;
        BackgroundWorker _worker;
        public Statistics()
        {
            _frames = new Dictionary<FrameType, Frame>();
            _frames[FrameType.NORMAL] = new Frame();
            _frames[FrameType.MISSING_TARGET] = new Frame();
            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerAsync();
        }
        
        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(1000);
                foreach (var item in _frames)
                {
                    item.Value.reset();
                }
                OnPropertyChanged("NormalFramesPerSecond");
                OnPropertyChanged("MissingBallDetectorFramesPerSecond");
            }
        }

        public int NormalFramesPerSecond {
            get
            {
                return _frames[FrameType.NORMAL].getFrameRate();
            }
        }

        public int MissingBallDetectorFramesPerSecond
        {
            get
            {
                return _frames[FrameType.MISSING_TARGET].getFrameRate();
            }
        }

        public int FramesPerSecond(FrameType frameType)
        {
            return _frames[frameType].getFrameRate();
        }

        public void commitFrame(FrameType frameType)
        {
            _frames[frameType].commit();
        }

        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
