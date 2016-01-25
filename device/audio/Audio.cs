using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kinect_Wrapper.device.audio
{
    public partial class Audio:IAudio
    {

        public void SayAsUser(string words, user.IKinectUser user)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<user.IKinectUser> UserSaying;

        public structures.AudioMode Mode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
