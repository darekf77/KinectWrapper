using Kinect_Wrapper.camera.Recorder;
using Microsoft.Kinect;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Kinect_Wrapper.device;
using Kinect_Wrapper.camera.Replayer;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Apex.MVVM;
using Kinect_Wrapper.frame;

namespace Kinect_Wrapper.camera
{
    public partial class KinectCamera
    {
        #region worker update
        public void update()
        {

        }
        #endregion

        #region update replay data
        private ReplayFrame lastFrame;
        protected readonly List<ReplayFrame> framesCopy = new List<ReplayFrame>();
        public void update(List<ReplayFrame> frames)
        {
            if (State != CameraState.PLAYING &&
                State != CameraState.PLAYING_PAUSE &&
                State != CameraState.PLAYING_STOPPING) return;

            #region replay finish, play again frames
            if (frames != null && frames.Count == 0)
            {
                foreach (var frame in framesCopy) frames.Add(frame);
                onReplayFinish?.Invoke(this, EventArgs.Empty);
            }
            #endregion

            #region clear frames when stopping, set unactive
            if (State == CameraState.PLAYING_STOPPING)
            {
                frames.Clear();
                framesCopy.Clear();
                State = CameraState.UNACTIVE;
                return;
            }
            #endregion

            #region when is not pause, get new frame
            if (State != CameraState.PLAYING_PAUSE)
            {
                lastFrame = frames.PopAt(0);
                framesCopy.Add(lastFrame);
            }
            #endregion

            Thread.Sleep(TimeSpan.FromMilliseconds(lastFrame.TimeStamp));

            if (lastFrame.isCorrect)
            {
                frame.synchronize(lastFrame.Depth, lastFrame.Color, lastFrame.Skeleton, IsPaused);
                FrameReady?.Invoke(this, frame);
            }
        }
        #endregion

        #region update kinect
        private void update(KinectSensor sensor)
        {
            if (sensor == null ||
                !sensor.IsRunning ||
                sensor.Status != KinectStatus.Connected)
                return;
            if (sensor.DepthStream == null || sensor.SkeletonStream == null || sensor.ColorStream == null) return;
            if (!sensor.DepthStream.IsEnabled || !sensor.ColorStream.IsEnabled || !sensor.SkeletonStream.IsEnabled) return;
            if (IsPaused)
            {
                Thread.Sleep(300);
                FrameReady(this, frame);
                return;
            }

            using (DepthImageFrame depthFrame = sensor.IsRunning ? sensor.DepthStream.OpenNextFrame(0) : null)
            using (SkeletonFrame skeletonFrame = sensor.IsRunning ? sensor.SkeletonStream.OpenNextFrame(100) : null)
            using (ColorImageFrame colorFrame = sensor.IsRunning ? sensor.ColorStream.OpenNextFrame(10) : null)
            {


                if (depthFrame != null && colorFrame != null && skeletonFrame != null)
                {
                    frame.synchronize(depthFrame, colorFrame, skeletonFrame, sensor.CoordinateMapper, IsPaused);
                    if (State == CameraState.RECORDING)
                    {
                        update(colorFrame, depthFrame, skeletonFrame, sensor);
                    }
                    FrameReady(this, frame);
                    return;
                }
            }
            return;
        }
        #endregion

        #region update when no deviec
        private void update(IKinectFrame noDeviceFrame)
        {
            //_frame.synchronize(_device.Name, toogleVisibleMessage, _isPaused);
            //toogleVisibleMessage = !toogleVisibleMessage;
            //if (FrameReady != null) FrameReady(this, _frame);
        }
        #endregion

        #region update record data
        private SkeletonRecorder skeletonRecorder;
        private ColorRecorder colorRecorder;
        private DepthRecorder depthRecorder;
        public void update(ColorImageFrame color, DepthImageFrame depth, SkeletonFrame skeleton, KinectSensor sensor)
        {
            if (skeletonRecorder == null || colorRecorder == null ||
                depthRecorder == null || sensor == null || writer == null)
            {
                Console.WriteLine("bad data Kinect.Camera.update");
                return;
            }

            skeletonRecorder.update(skeleton);
            Flush();
            colorRecorder.update(color);
            Flush();
            depthRecorder.update(depth);
            Flush();
        }
        #region flush date
        private DateTime previousFlushDate;
        private void Flush()
        {
            var now = DateTime.Now;

            if (now.Subtract(previousFlushDate).TotalSeconds > 60)
            {
                previousFlushDate = now;
                writer.Flush();
            }
        }
        #endregion
        #endregion

    }
}
