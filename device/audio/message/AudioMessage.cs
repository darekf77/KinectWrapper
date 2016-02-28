using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.audio.message
{
    public class AudioMessage:IAudioMessage
    {
        public AudioMessage(String message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}
