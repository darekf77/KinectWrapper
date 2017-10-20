using Kinect_Wrapper.device;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoiceRecorder.Audio;

namespace Kinect_Wrapper.camera
{
    public class AudioRecorderReplayer : IAudioRecorderReplayer
    {
        private IAudioPlayer player;
        private IAudioRecorder recorder;
        public IAudioRecognition Recognizer { get; private set; }
        private AutoTuneSettings autoTuneSettings = new AutoTuneSettings();

        public CameraState State { get; set; }
        public ObservableCollection<IAudioSourceDevice> Devices { get; private set; }

        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region constructor
        public AudioRecorderReplayer()
        {
            Devices = new ObservableCollection<IAudioSourceDevice>();
            Recognizer = new AudioRecognition();
            recorder = new AudioRecorder();
        }
        #endregion

        #region ini camera from device
        public void init(IDevice device)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (device == null) return;
                Devices.Clear();
                int i = 0;
                for (i = 0; i < WaveIn.DeviceCount; i++)
                {
                    var product = WaveIn.GetCapabilities(i);
                    var dev = new AudioSourceDevice(product, device.sensor, player, i);
                    Devices.Add(dev);
                }
                if (File.Exists(device.Path))
                {
                    var replayDevice = new AudioSourceDevice(device.Path, i);
                    SelectedDevice = replayDevice;
                }
                else
                {
                    SelectedDevice = Devices.FirstOrDefault();
                }
            }));
        }
        #endregion

        #region is recording possible
        private bool _isRecordingPossible = false;

        public bool IsRecordingPossible
        {
            get { return _isRecordingPossible; }
            set { _isRecordingPossible = value; OnPropertyChanged(); }
        }
        #endregion

        #region is preparing audio
        private bool _IsPreparingAudio = false;

        public bool IsPreparingAudio
        {
            get { return _IsPreparingAudio; }
            set { _IsPreparingAudio = value; OnPropertyChanged(); }
        }

        #endregion

        #region selected device
        private IAudioSourceDevice _selectedDevice;

        public IAudioSourceDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                IsRecordingPossible = (value != null && value.isRecordingPossible);
                OnPropertyChanged();
                IsPreparingAudio = true;
                Recognizer.init(value);
                IsPreparingAudio = false;
                if (recorder.RecordingState == RecordingState.Stopped)
                {
                    recorder.BeginMonitoring(SelectedDevice.Id);
                    enableMicrophoeVisualization();
                }
                else
                {
                    recorder.Stop();
                    recorder.Stopped += (e, v) =>
                    {
                        enableMicrophoeVisualization();
                    };
                }

            }
        }
        #endregion

        #region microphone visualization
        void enableMicrophoeVisualization()
        {
            recorder.SampleAggregator.MaximumCalculated += (e, v) =>
            {
                lastPeak = Math.Max(v.MaxSample, Math.Abs(v.MinSample));
                OnPropertyChanged("CurrentInputLevel");
                OnPropertyChanged("RecordedTime");
            };
        }

        private float lastPeak;
        public float CurrentInputLevel { get { return lastPeak * 100; } }
        #endregion

        #region prepare audio path
        private String prepareAudioPath(string filePath)
        {
            return Path.ChangeExtension(filePath, "wav");
        }
        #endregion

        #region record
        public void record(string toFile)
        {
            if (SelectedDevice == null) return;
            var audioFileName = prepareAudioPath(toFile);
            recorder.BeginRecording(audioFileName);
        }
        #endregion

        #region replay
        public void replay(string replayFile)
        {
            var audioFileName = prepareAudioPath(replayFile);
            if (File.Exists(audioFileName))
            {
                recorder.BeginRecording(audioFileName);
            }
        }
        #endregion

        #region stop
        public void stop()
        {
            if (recorder.RecordingState == RecordingState.Recording)
            {
                recorder.Stop();
            }
            else if (player.CurrentPosition > player.StartPosition)
            {
                player.Stop();
            }

        }
        #endregion

        #region pause
        public void pause()
        {
            if (State == CameraState.PLAYING || State == CameraState.PLAYING_PAUSE)
            {
                player.Pause = !player.Pause;
            }
            else if (State == CameraState.RECORDING || State == CameraState.RECORDING_PAUSE)
            {
                recorder.Pause = !recorder.Pause;
            }

        }
        #endregion
    }
}
