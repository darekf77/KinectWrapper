using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.audio.message
{
    public interface IAudioMessage
    {
        String Message { get; }
        float Confidence { get; }
    }
}
