using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Recorder
{
    public class AudioRecorder
    {
        WaveFileWriter writer;
        WaveFormat recordingFormat = new WaveFormat();

        public AudioRecorder(WaveFileWriter writer)
        {
            this.writer = writer;
        }


    }
}
