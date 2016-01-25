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
        void _replay_ReplayFinished()
        {
            IsStreaming = false;
            Console.WriteLine("Replay finish");
            if (_replay != null)
            {
                _replay.Start();
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
                if(FramesReady!=null)
                FramesReady(this, _frame);
            }
        }

        private Boolean updateFrames()
        {
            //Console.WriteLine("isStreaming kinect" + IsStreaming.ToString());
            if (_sensor == null ||
                !_sensor.IsRunning ||
                _sensor.Status != KinectStatus.Connected)
                return false;
            if (_sensor.DepthStream == null || _sensor.SkeletonStream == null || _sensor.ColorStream == null) return false;
            if (!_sensor.DepthStream.IsEnabled || !_sensor.ColorStream.IsEnabled || !_sensor.SkeletonStream.IsEnabled) return false;
            if (IsPaused)
            {
                Thread.Sleep(300);
                FramesReady(this, _frame);
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
            using (DepthImageFrame depthFrame = _sensor.IsRunning ? _sensor.DepthStream.OpenNextFrame(0) : null)
            using (SkeletonFrame skeletonFrame = _sensor.IsRunning?_sensor.SkeletonStream.OpenNextFrame(100):null)
            using (ColorImageFrame colorFrame = _sensor.IsRunning?_sensor.ColorStream.OpenNextFrame(100):null)
            {

                if (depthFrame != null && colorFrame != null && skeletonFrame != null)
                {
                    _frame.synchronize(depthFrame, colorFrame, skeletonFrame);
                    if (_recorder != null && _recorder.isRecording && !_isStoppingRecorder)
                    {
                        _recorder.Record(colorFrame);
                        _recorder.Record(depthFrame);
                        _recorder.Record(skeletonFrame,_sensor);
                    }
                    FramesReady(this, _frame);
                    return true;
                }                
            }
            return false;
        }

        private Boolean toogleVisibleMessage = false;
        private void updateFramesNoDevice() {
            _frame.synchronize(_device.Name,toogleVisibleMessage);
            toogleVisibleMessage = !toogleVisibleMessage;
            if (FramesReady!=null) FramesReady(this, _frame);
        }
        

        

        
    }
}
