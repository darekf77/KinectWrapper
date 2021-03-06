﻿using Kinect_Wrapper.camera.Recorder;
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
using Kinect_Wrapper.structures;

namespace Kinect_Wrapper.camera
{
    public partial class KinectCamera
    {
        #region propagate frame
        private bool isFramePropagated = false;
        private void propageteFrame()
        {
            isFramePropagated = true;
            FrameReady?.Invoke(this, frame);
            IsStreaming = (CurrentDevice != null && CurrentDevice.Type != DeviceType.NO_DEVICE);
            #region preview next frame only
            if (pauseOnNextFrame)
            {
                if (IsPaused)
                {
                    PausePlay.DoExecute();
                }
                else
                {
                    pauseOnNextFrame = false;
                    Pause.DoExecute();
                }
            }
            #endregion
        }
        #endregion

        #region worker update
        public void update()
        {
            isFramePropagated = false;
            if (CurrentDevice != null)
            {
                #region stop device if is no ready
                if (CurrentDevice.State == DeviceState.NOT_READY)
                {
                    updateRecordData();
                    CurrentDevice.stop();
                    State = CameraState.UNACTIVE;
                }
                #endregion
                switch (CurrentDevice.Type)
                {
                    case structures.DeviceType.NO_DEVICE:
                        updateNoDevice();
                        break;
                    case structures.DeviceType.KINECT_1:
                        updateKinect();
                        break;
                    case structures.DeviceType.RECORD_FILE_KINECT_1:
                        updateReplay();
                        break;
                    default:
                        break;
                }
            }
            if (!isFramePropagated) IsStreaming = false;
        }
        #endregion

        #region update replay data
        private ReplayFrame lastFrame;
        protected readonly List<ReplayFrame> framesCopy = new List<ReplayFrame>();
        public void updateReplay()
        {
            #region replay finish, play again frames
            if (frames != null && frames.Count == 0)
            {
                foreach (var frame in framesCopy) frames.Add(frame);
                onReplayEnd?.Invoke(this, EventArgs.Empty);
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
            if (State == CameraState.PLAYING)
            {
                lastFrame = frames.PopAt(0);
                framesCopy.Add(lastFrame);
            }
            #endregion

            var howlongSleep = TimeSpan.FromMilliseconds(lastFrame.TimeStamp);
            Thread.Sleep(howlongSleep);

            if (lastFrame.isCorrect)
            {
                frame.synchronize(lastFrame.Depth, lastFrame.Color, lastFrame.Skeleton, IsPaused);
                propageteFrame();
            }
        }
        #endregion

        #region update kinect
        private void updateKinect()
        {
            #region stop or cancel recording
            if (State == CameraState.RECORDING_CANCEL ||
                State == CameraState.RECORDING_STOPPING)
            {
                updateRecordData();
                State = CameraState.PLAYING;
                return;
            }
            #endregion

            #region stop device when is playing
            if (State == CameraState.PLAYING_STOPPING)
            {
                CurrentDevice.stop();
                State = CameraState.UNACTIVE;
                return;
            }
            #endregion

            #region handle pause in recording and playing
            if (IsPaused)
            {
                Thread.Sleep(300);
                FrameReady(this, frame);
                return;
            }
            #endregion

            #region prevent unprepared sensor to access streams
            var sensor = CurrentDevice.sensor;
            if (sensor == null || !sensor.IsRunning || sensor.Status != KinectStatus.Connected ||
                sensor.DepthStream == null || sensor.SkeletonStream == null || sensor.ColorStream == null ||
                !sensor.DepthStream.IsEnabled || !sensor.ColorStream.IsEnabled || !sensor.SkeletonStream.IsEnabled)
            {
                Console.WriteLine("something wrong with kinect");
                return;
            }
            #endregion

            using (DepthImageFrame depthFrame = sensor.IsRunning ? sensor.DepthStream.OpenNextFrame(0) : null)
            using (SkeletonFrame skeletonFrame = sensor.IsRunning ? sensor.SkeletonStream.OpenNextFrame(100) : null)
            using (ColorImageFrame colorFrame = sensor.IsRunning ? sensor.ColorStream.OpenNextFrame(10) : null)
            {
                if (depthFrame != null && colorFrame != null && skeletonFrame != null)
                {
                    frame.synchronize(depthFrame, colorFrame, skeletonFrame, sensor.CoordinateMapper, IsPaused);
                    if (State == CameraState.RECORDING)
                    {
                        updateRecordData(colorFrame, depthFrame, skeletonFrame, sensor);
                    }
                    propageteFrame();
                    return;
                }
                else
                {
                    Console.WriteLine("Somethig sucke with streams");
                }
            };
        }
        #endregion

        #region update when no deviec
        bool toogleVisibleMessage = false;
        private void updateNoDevice()
        {
            IKinectFrame noDeviceFrame = frame;
            frame.synchronize(CurrentDevice.Name, toogleVisibleMessage, IsPaused);
            toogleVisibleMessage = !toogleVisibleMessage;
            propageteFrame();
            Thread.Sleep(1000);
        }
        #endregion

        #region update record data
        private SkeletonRecorder skeletonRecorder;
        private ColorRecorder colorRecorder;
        private DepthRecorder depthRecorder;
        public void updateRecordData(bool unexpectedStop = false)
        {
            if (State == CameraState.RECORDING_STOPPING || unexpectedStop)
            {
                writer.Close();
                stream.Close();
                writer.Dispose();
                stream.Dispose();
                RecordComplete?.Invoke(this, this.RecordFilePath);
            }
            if (State == CameraState.RECORDING_CANCEL)
            {
                writer.Close();
                stream.Close();
                writer.Dispose();
                stream.Dispose();
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                File.Delete(RecordFilePath);
            }
        }
        public void updateRecordData(ColorImageFrame color, DepthImageFrame depth, SkeletonFrame skeleton, KinectSensor sensor)
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
