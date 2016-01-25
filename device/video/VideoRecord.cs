using Kinect.Replay.Record;
using Kinect.Replay.Replay;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Kinect_Wrapper.user;
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
        public void startRecordAndSaveTo(string filename)
        {
            IsRecording = true;
            _isStoppingRecorder = false;
            _recorder = new KinectRecorder(KinectRecordOptions.Frames, filename, _sensor);
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
                RecordComplete(this, new Device(fileName));
            }
        }


    }
}
