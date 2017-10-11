using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using System;

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
