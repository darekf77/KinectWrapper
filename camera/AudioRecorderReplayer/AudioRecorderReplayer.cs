using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceRecorder.Audio;

namespace Kinect_Wrapper.camera
{
    public class AudioRecorderReplayer : IKinectCameraOperator
    {
        private IAudioPlayer player;
        private IAudioRecorder recorder;
        private AutoTuneSettings autoTuneSettings = new AutoTuneSettings();
        private IAudioRecordDevice device;

        public CameraState State { get; set; }

        public AudioRecorderReplayer()
        {

        }

        private String prepareAudioPath(string filePath)
        {
            return Path.ChangeExtension(filePath, "wav");
        }

        #region record
        public void record(string toFile)
        {
            device = AudioRecordDevice.Device;
            if (device == null) return;
            var audioFileName = prepareAudioPath(toFile);
            recorder.BeginMonitoring(device.guid);
            recorder.BeginRecording(audioFileName);
        }
        #endregion

        #region replay
        public void replay(string replayFile)
        {
            var audioFileName = prepareAudioPath(replayFile);
            recorder.BeginRecording(audioFileName);
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
