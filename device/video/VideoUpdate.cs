using Kinect.Replay.Record;
using Kinect.Replay.Replay;
using Kinect.Replay.Replay.Color;
using Kinect.Replay.Replay.Depth;
using Kinect.Replay.Replay.Skeletons;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.video
{
    public partial class Video
    {
        public event EventHandler StreamingStarted;
        private Boolean _isStreaming = false;

        public bool IsStreaming
        {
            get
            {
                return _isStreaming;
            }
            set
            {
                var before = _isStreaming;
                _isStreaming = value;
                if(value && !before && StreamingStarted!=null)
                {
                    StreamingStarted(this, EventArgs.Empty);
                }
                OnPropertyChanged("IsStreaming");
            }
        }


        public void update()
        {
            if (_device == null) return;
            if (_device != null && _device.Type == DeviceType.KINECT_1)
            {
                IsStreaming = updateFrames();
            }
            else if (_device != null && _device.Type == DeviceType.RECORD_FILE_KINECT_1)
            {
                IsStreaming = (_device!=null &&_device.replay.Started && !_device.replay.IsFinished);
            }
            else if (_device != null && _device.Type == DeviceType.NO_DEVICE)
            {
                IsStreaming = false;
                updateFramesNoDevice();
            }
        }

        void _replay_ReplayFinished()
        {
            IsStreaming = false;
            Console.WriteLine("Replay finish");
            if (_device.replay != null)
            {
                _device.replay.Start();
            }
        }

        void ReplayAllFramesReady(ReplayAllFramesReadyEventArgs e)
        {
            //Console.WriteLine("isStreaming replay" + IsStreaming.ToString());
            var color = e.AllFrames.ColorImageFrame;
            var skeleton = e.AllFrames.SkeletonFrame;
            var depth = e.AllFrames.DepthImageFrame;

            if (color != null && skeleton != null && depth != null)
            {
                _frame.synchronize(depth, color, skeleton);
                if(FrameReady!=null)
                FrameReady(this, _frame);
            }
        }

        private Boolean updateFrames()
        {
            //Console.WriteLine("isStreaming kinect" + IsStreaming.ToString());
            var sensor = _device.sensor;
            if (sensor == null ||
                !sensor.IsRunning ||
                sensor.Status != KinectStatus.Connected)
                return false;
            if (sensor.DepthStream == null || sensor.SkeletonStream == null || sensor.ColorStream == null) return false;
            if (!sensor.DepthStream.IsEnabled || !sensor.ColorStream.IsEnabled || !sensor.SkeletonStream.IsEnabled) return false;
            if (IsPaused)
            {
                Thread.Sleep(300);
                FrameReady(this, _frame);
                if (_isNextFrameSensor) IsPaused = false;
                return true;
            }
            else
            {
                if (_isNextFrameSensor)
                {
                    IsPaused = true;
                    _isNextFrameSensor = false;
                }
            }
            using (DepthImageFrame depthFrame = sensor.IsRunning ? sensor.DepthStream.OpenNextFrame(0) : null)
            using (SkeletonFrame skeletonFrame = sensor.IsRunning?sensor.SkeletonStream.OpenNextFrame(100):null)
            using (ColorImageFrame colorFrame = sensor.IsRunning?sensor.ColorStream.OpenNextFrame(100):null)
            {

                if (depthFrame != null && colorFrame != null && skeletonFrame != null)
                {
                    _frame.synchronize(depthFrame, colorFrame, skeletonFrame);
                    if (_recorder != null && _recorder.isRecording && !_isStoppingRecorder)
                    {
                        _recorder.Record(colorFrame);
                        _recorder.Record(depthFrame);
                        _recorder.Record(skeletonFrame,sensor);
                    }
                    FrameReady(this, _frame);
                    return true;
                }                
            }
            return false;
        }

        private Boolean toogleVisibleMessage = false;
        private void updateFramesNoDevice() {
            _frame.synchronize(_device.Name,toogleVisibleMessage);
            toogleVisibleMessage = !toogleVisibleMessage;
            if (FrameReady!=null) FrameReady(this, _frame);
        }
        

        

        
    }
}
