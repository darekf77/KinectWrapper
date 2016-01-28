using Kinect_Wrapper.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.audio.message
{
    public interface IAudioMessage
    {
        IKinectUser User { get; }
        String Message { get; }
    }
}
