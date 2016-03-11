using Kinect.Replay.Record;
using Kinect.Replay.Replay;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.video
{
    public abstract partial class VideoBase
    {
        private Boolean _isStoppingRecorder = false;
        private KinectRecorder _recorder;
        private RecordMode _recordMode = RecordMode.NO_AUDIO;

        private string _filename;
        public void startRecordAndSaveTo(string filename)
        {
            IsRecording = true;
            _filename = filename;
            _isStoppingRecorder = false;
            try
            {
                _recorder = new KinectRecorder(KinectRecordOptions.Frames, filename, _device.sensor);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                IsRecording = false;
            }
        }

        public void stopRecord(Boolean noEvent = false)
        {
            _isStoppingRecorder = true;
            if (_recorder == null) return;
            IsRecording = false;            
            _recorder.Stop();
            String fileName = _recorder.FileName;
            _recorder = null;
            if (!noEvent && RecordComplete != null)
            {
                RecordComplete(this, fileName);
            }
        }

        public RecordMode RecordingMode
        {
            get
            {
                return _recordMode;
            }
            set
            {
                _recordMode = value;
            }
        }

        private bool _isRecording = false;
        public bool IsRecording {
            get
            {
                return _isRecording;
            }
            set
            {
                _isRecording = value;
                OnPropertyChanged("IsRecording");
            }
        }

        public string RecordingFilePath
        {
            get
            {
                return _filename;
            }
        }

        private bool _isRecordingPossible = false;
        public bool IsRecordingPossible
        {
            get
            {
                return _isRecordingPossible;
            }
            set
            {
                _isRecordingPossible = value;
                OnPropertyChanged("IsRecordingPossible");
            }
        }


        public event EventHandler<String> RecordComplete;


    }
}
