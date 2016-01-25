using Kinect_Wrapper.structures;
using Kinect_Wrapper.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.audio
{
    public interface IAudio
    {
        void SayAsUser(String words,IKinectUser user);

        event EventHandler<IKinectUser> UserSaying;

        AudioMode Mode { get; set; }
    }
}
