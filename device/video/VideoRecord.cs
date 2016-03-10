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
    public partial class Video
    {
        private Boolean _isStoppingRecorder = false;
        private KinectRecorder _recorder;
        private RecordMode _recordMode = RecordMode.NO_AUDIO;

        public void startRecordAndSaveTo(string filename)
        {
            IsRecording = true;
            _isStoppingRecorder = false;
            _recorder = new KinectRecorder(KinectRecordOptions.Frames, filename, _device.sensor);
        }

        public void stopRecord()
        {
            _isStoppingRecorder = true;
            if (_recorder == null) return;
            IsRecording = false;            
            _recorder.Stop();
            String fileName = _recorder.FileName;
            _recorder = null;
            if (RecordComplete != null)
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

        public bool IsRecording { get; private set; }

        public event EventHandler<String> RecordComplete;


    }
}
