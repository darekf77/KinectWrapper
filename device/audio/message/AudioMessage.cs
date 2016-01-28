using Kinect_Wrapper.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.audio.message
{
    public class AudioMessage:IAudioMessage
    {
        public AudioMessage(String message, IKinectUser user)
        {
            Message = message;
            User = user;
        }
        public IKinectUser User { get; private set; }

        public string Message { get; private set; }
    }
}
