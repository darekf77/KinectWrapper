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

namespace Kinect_Wrapper.camera
{
    public class KinectCamera : IKinectCamera
    {
        KinectSensor sensor;
        BinaryWriter writer;
        BinaryReader reader;
        Stream stream;
        KinectRecordOptions Options = KinectRecordOptions.Everything;

        #region singleton 
        private volatile static IKinectCamera _instance;
        static readonly object _locker = new object();
        public static IKinectCamera Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new KinectCamera();
                }
                return _instance;
            }
        }
        private KinectCamera()
        {

        }
        #endregion

        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region init / constructor
        public void init(IDevice device)
        {
            this.sensor = device.sensor;
            audio.init(device);
            OnPropertyChanged("isRecordingPossible");
        }

        #endregion

        #region audio
        AudioRecorderReplayer audio = new AudioRecorderReplayer();
        public IAudioRecorderReplayer Audio
        {
            get { return audio; }
        }
        #endregion        

        #region state
        private CameraState _state = CameraState.UNACTIVE;
        public CameraState State
        {
            get { return _state; }
            set
            {
                _state = value;
                audio.State = value;
            }
        }
        #endregion

        #region replay (file)
        public event EventHandler onReplayFinish;
        public void replay(string replayFile)
        {
            stream = File.OpenRead(replayFile);
            reader = new BinaryReader(stream);

            Options = (KinectRecordOptions)reader.ReadInt32();
            //var paramsArrayLength = reader.ReadInt32();
            //var colorToDepthRelationalParameters = reader.ReadBytes(paramsArrayLength);
            //CoordinateMapper = new CoordinateMapper(colorToDepthRelationalParameters);

            AddFrames(reader);
            onReplayFinish?.Invoke(this, EventArgs.Empty);
            audio.replay(replayFile);
            State = CameraState.PLAYING;
        }
        #endregion

        #region update replay data
        public event EventHandler<ReplayFrame> onFrameReady;
        private ReplayFrame lastFrame;
        public void update()
        {
            if (State != CameraState.PLAYING && State != CameraState.PLAYING_PAUSE) return;
            if (State != CameraState.PLAYING_PAUSE)
            {
                lastFrame = frames.PopAt(0);
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(lastFrame.TimeStamp));
            onFrameReady?.Invoke(this, lastFrame);
        }
        #endregion

        #region add replay frames
        protected readonly List<ReplayFrame> frames = new List<ReplayFrame>();
        private void AddFrames(BinaryReader reader)
        {
            //not the best of approaches - assuming that color frame is the 1st frame followed by depth and skeleton frame
            while (reader.PeekChar() != -1 || reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var header = (FrameType)reader.ReadInt32();
                switch (header)
                {
                    case FrameType.Color:
                        var colorFrame = new FrameColorReplayer(reader);
                        frames.Add(new ReplayFrame { Color = colorFrame });
                        break;
                    case FrameType.Depth:

                        var depthFrame = new FrameDepthReplayer(reader);
                        if (frames.Any())
                            frames.Last().Depth = depthFrame;
                        break;
                    case FrameType.Skeletons:
                        var skeletonFrame = new FrameSkeletonReplayer(reader);
                        if (frames.Any())
                            frames.Last().Skeleton = skeletonFrame;
                        break;
                }
            }
        }
        #endregion

        #region is recording possible
        public bool isRecordingPossible()
        {
            return (sensor != null && sensor.IsRunning && sensor.Status == KinectStatus.Connected);
        }
        #endregion

        #region record
        public void record(string toFile)
        {
            if (Path.GetExtension(toFile).Trim() != "replay") return;
            if (!isRecordingPossible()) return;
            stream = File.Create(toFile);
            writer = new BinaryWriter(stream);



            writer.Write((int)Options); // TODO delete this write version

            colorRecorder = new ColorRecorder(writer, sensor);
            depthRecorder = new DepthRecorder(writer, sensor);
            skeletonRecorder = new SkeletonRecorder(writer, sensor);
            audio.record(toFile);

            previousFlushDate = DateTime.Now;
            State = CameraState.RECORDING;
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

        #region stop
        void stopRecord()
        {
            writer.Close();
            stream.Close();
            writer.Dispose();
            stream.Dispose();
            State = CameraState.UNACTIVE;
        }

        public void stop()
        {
            if (State == CameraState.RECORDING) stopRecord();
        }

        public Command Stop
        {
            get
            {
                return new Command(() =>
                {
                    stop();
                });
            }
        }

        #endregion

        #region pause
        public void pause()
        {
            switch (State)
            {
                case CameraState.UNACTIVE:
                    break;
                case CameraState.PLAYING:
                    State = CameraState.PLAYING_PAUSE;
                    break;
                case CameraState.PLAYING_PAUSE:
                    State = CameraState.PLAYING;
                    break;
                case CameraState.RECORDING:
                    State = CameraState.RECORDING_PAUSE;
                    break;
                case CameraState.RECORDING_PAUSE:
                    State = CameraState.RECORDING;
                    break;
                default:
                    break;
            }
        }
        public Command Pause
        {
            get
            {
                return new Command(() =>
                {
                    pause();
                });
            }
        }

        #endregion

    }
}
