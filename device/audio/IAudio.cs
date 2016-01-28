using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.structures;
using Kinect_Wrapper.user;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.audio
{
    public interface IAudio
    {
        void start(List<String> grammar);
        void stop();

        void Say(string words, IKinectUser user);
        void Say(IAudioMessage message);

        event EventHandler<IAudioMessage> UserSaying;

    }
}
