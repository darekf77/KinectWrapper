using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
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
        /// <summary>
        /// Width of frame
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of frame
        /// </summary>
        int Height { get; }

        /// <summary>
        /// check whether device video is paused
        /// </summary>
        Boolean IsPaused { get; }

        /// <summary>
        /// Toogle pause play device frames
        /// </summary>
        void pausePlay();

        /// <summary>
        /// Get only next frame
        /// </summary>
        void nextFrame();

        /// <summary>
        /// Frame is generating 
        /// </summary>
        Boolean IsStreaming { get; }

        /// <summary>
        /// check whether video is recorded
        /// </summary>
        Boolean IsRecording { get; }

        /// <summary>
        /// Recording mode 
        /// </summary>
        RecordMode RecordingMode { get; set; }

        /// <summary>
        /// Record stram data and save to file
        /// </summary>
        /// <param name="filename">Name of *.record file</param>
        /// <returns>False when bad filename, or not enought space </returns>
        void startRecordAndSaveTo(String filename);

        /// <summary>
        /// Stop recording process and create new recorder device
        /// </summary>
        void stopRecord();

        /// <summary>
        /// Record complete and new Device-File created
        /// </summary>
        event EventHandler<IDevice> RecordComplete;

        /// <summary>
        /// check wheather kinect frames are ready
        /// </summary>
        event EventHandler<IKinectFrame> FramesReady;
        
    }
}
