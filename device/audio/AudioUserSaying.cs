using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kinect_Wrapper.device.audio
{
    public partial class Audio
    {

        // GOOD SPEACH
        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (UserSaying != null)
            {
                UserSaying(this, new AudioMessage(e.Result.Text, e.Result.Confidence));
            }
        }

        // REJECTED SPEACH
        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speach rejected :" + e.Result.Text);
        }


        // HYPOTHESIS SPEACH
        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.WriteLine("Speach hypothesized: " + e.Result.Text + ", confidence:" + e.Result.Confidence);
        }

        public event EventHandler<IAudioMessage> UserSaying;



    }
}
