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
using SharedLibJG.config;
using Microsoft.Win32;
using SharedLibJG.Helpers;

namespace Kinect_Wrapper.camera
{
    public partial class KinectCamera : IKinectCamera
    {
        BinaryWriter writer { get; set; }
        BinaryReader reader { get; set; }
        Stream stream { get; set; }
        KinectRecordOptions Options = KinectRecordOptions.Everything;
        IKinectFrame frame { get; set; }

        #region file pathes
        public String ReplayFilePath { get; private set; }
        public String RecordFilePath { get; private set; }
        #endregion

        #region device
        /// <summary>
        /// Same device in Device Manager 
        /// </summary>
        public IDevice CurrentDevice { get; set; }
        #endregion

        public event EventHandler<IKinectFrame> FrameReady;
        public event EventHandler onNoDeviceNeeded;
        public event EventHandler onReplayEnd;
        public event EventHandler<string> RecordComplete;

        #region singleton 
        private volatile static IKinectCamera instance;
        static readonly object syncRoot = new object();
        public static IKinectCamera Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new KinectCamera();
                    }

                }
                return instance;
            }
        }
        private KinectCamera()
        {
            initActionIsStreaming();
        }
        #endregion

        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public event EventHandler onDeviceChanged;
        #region play sensor or replay
        public Command Play
        {
            get
            {
                return new Command(() =>
                {
                    if (DeviceSelecteToPlay == null) return;
                    CurrentDevice = null;
                    DeviceSelecteToPlay.start(() =>
                    {
                        CurrentDevice = DeviceSelecteToPlay;
                        onDeviceChanged?.Invoke(this, EventArgs.Empty);
                        //audio.init(CurrentDevice);
                        switch (CurrentDevice.Type)
                        {
                            case structures.DeviceType.NO_DEVICE:
                                frame = CurrentDevice.nodeviceframe;
                                break;
                            case structures.DeviceType.KINECT_1:
                                frame = new KinectFrame(this);
                                break;
                            case structures.DeviceType.RECORD_FILE_KINECT_1:
                                ReplayFilePath = CurrentDevice.Path;
                                frame = new KinectFrame(this);
                                stream = File.OpenRead(ReplayFilePath);
                                reader = new BinaryReader(stream);
                                Options = (KinectRecordOptions)reader.ReadInt32();
                                //var paramsArrayLength = reader.ReadInt32();
                                //var colorToDepthRelationalParameters = reader.ReadBytes(paramsArrayLength);
                                //CoordinateMapper = new CoordinateMapper(colorToDepthRelationalParameters);
                                AddFrames(reader);
                                //audio.replay(ReplayFilePath);
                                break;
                        }
                        State = CameraState.PLAYING;
                        OnPropertyChanged("isRecordingPossible");
                    });
                });
            }
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
                if (value == CameraState.UNACTIVE)
                {
                    onNoDeviceNeeded?.Invoke(this, EventArgs.Empty);
                }
                OnPropertyChanged("IsRecording");
                OnPropertyChanged("IsRecordingPossible");
                OnPropertyChanged("IsPaused");
            }
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
        public bool IsRecordingPossible
        {
            get
            {
                return (CurrentDevice != null && CurrentDevice.sensor != null &&
                    CurrentDevice.sensor.IsRunning &&
                    CurrentDevice.sensor.Status == KinectStatus.Connected);
            }

        }
        #endregion

        #region record
        public Command Record
        {
            get
            {
                return new Command(() =>
                {
                    if (IsRecording)
                    {
                        Stop.DoExecute();
                    }
                    else
                    {
                        #region  get filename from save dialog
                        var saveFileDialog = new SaveFileDialog
                        {
                            Title = "Select filename",
                            Filter = "Replay files|*.replay"
                        };
                        if (saveFileDialog.ShowDialog() != true) return;
                        #endregion

                        var toFile = saveFileDialog.FileName;
                        if (Path.GetExtension(toFile).Trim() != ".replay") return;
                        if (File.Exists(toFile)) File.Delete(toFile);

                        RecordFilePath = toFile;
                        if (!IsRecordingPossible) return;
                        stream = File.Create(toFile);
                        writer = new BinaryWriter(stream);
                        writer.Write((int)Options); // TODO delete this write version
                        colorRecorder = new ColorRecorder(writer, CurrentDevice.sensor);
                        depthRecorder = new DepthRecorder(writer, CurrentDevice.sensor);
                        skeletonRecorder = new SkeletonRecorder(writer, CurrentDevice.sensor);
                        //audio.record(toFile);
                        previousFlushDate = DateTime.Now;
                        State = CameraState.RECORDING;
                        OnPropertyChanged("IsRecording");
                    }
                });
            }
        }
        public bool IsRecording
        {
            get
            {
                return (CameraState.RECORDING.EnumGroupRange().isInside((int)State));
            }
        }
        #endregion

        #region stop        
        public Command Stop
        {
            get
            {
                return new Command(() =>
                {
                    switch (State)
                    {
                        case CameraState.PLAYING:
                            State = CameraState.PLAYING_STOPPING;
                            break;
                        case CameraState.PLAYING_PAUSE:
                            State = CameraState.PLAYING_STOPPING;
                            break;
                        case CameraState.RECORDING:
                            State = CameraState.RECORDING_STOPPING;
                            break;
                        case CameraState.RECORDING_PAUSE:
                            State = CameraState.RECORDING_STOPPING;
                            break;
                    }
                });
            }
        }
        #endregion

        #region pause

        public bool IsPaused
        {
            get
            {
                return (State == CameraState.PLAYING_PAUSE || State == CameraState.RECORDING_PAUSE);
            }
        }

        public Command Pause
        {
            get
            {
                return new Command(() =>
                {
                    switch (State)
                    {
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
                    OnPropertyChanged("IsPaused");
                });
            }
        }
        #endregion

        #region next frame
        private bool pauseOnNextFrame { get; set; }
        public Command NextFrame
        {
            get
            {
                return new Command(() =>
                {
                    if (!IsPaused) Pause.DoExecute();
                    else pauseOnNextFrame = true;
                });
            }
        }
        #endregion

        #region pause play
        public Command PausePlay
        {
            get
            {
                return new Command(() =>
                {
                    switch (State)
                    {
                        case CameraState.PLAYING_PAUSE:
                            State = CameraState.PLAYING;
                            break;
                        case CameraState.RECORDING_PAUSE:
                            State = CameraState.RECORDING;
                            break;
                    }
                    OnPropertyChanged("IsPaused");
                });
            }
        }
        #endregion

        #region cancle record
        public Command CancelRecord
        {
            get
            {
                return new Command(() =>
                {
                    State = CameraState.RECORDING_CANCEL;
                });
            }
        }
        #endregion

        #region max / min depth
        public int MaxDepth
        {
            get
            {
                return GlobalConfig.MaxDepth;
            }
        }

        public int MinDepth
        {
            get
            {
                return GlobalConfig.MinDepth;
            }
        }
        #endregion

        #region is streaming
        private bool __IsStreaming;

        private Action<int> actionUpdateIsStreaming;
        private void initActionIsStreaming()
        {
            actionUpdateIsStreaming = (e) =>
             {
                 OnPropertyChanged("IsStreaming");
             };
        }

        public bool IsStreaming
        {
            get { return __IsStreaming; }
            set
            {
                __IsStreaming = value;
                if (value != IsStreamingPrev)
                {
                    var debouceWrapp = actionUpdateIsStreaming.Debounce();
                    debouceWrapp(1000);
                }
                IsStreamingPrev = value;
            }
        }

        private bool IsStreamingPrev { get; set; }
        #endregion

        #region selected device
        public IDevice DeviceSelecteToPlay { get; set; }
        #endregion
    }
}
