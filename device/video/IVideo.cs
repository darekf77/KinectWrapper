using Kinect.Replay.Replay;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kinect_Wrapper.device.video
{
    public interface IVideo
    {
        int Width { get; }

        int Height { get; }

        IDevice CurrentDevice { get; set; }
        
        void update();                

        Boolean IsPaused { get; }

        void pausePlay();

        void nextFrame();  

        Boolean IsStreaming { get; }

        Boolean IsRecording { get; }

        Boolean IsRecordingPossible { get; }

        RecordMode RecordingMode { get; set; }

        void startRecordAndSaveTo(String filename);

        void stopRecord(Boolean noEvent = false);

        String RecordingFilePath { get; }

        String StreamingFilePath { get; }

        event EventHandler StreamingStarted;
        event EventHandler<String> RecordComplete;
        event EventHandler<IKinectFrame> FrameReady;
        
    }
}
