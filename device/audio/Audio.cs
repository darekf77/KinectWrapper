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
    public partial class Audio:IAudio
    {
        private IDevice _device;
        private BackgroundWorker worker;        
        private KinectSensor _sensor;
        private String filePathAudio;
        private Boolean _deviceStarted = false;

        private SpeechRecognitionEngine speechRecognizer;
        private Choices _grammar;
        private SpeechRecognitionEngine sre;
        private KinectAudioSource audioSource;

        public Audio(IDevice device,KinectSensor sensor)
        {
            _device = device;
            _sensor = sensor;           
        }

        public Audio(IDevice device, String filepath)
        {
            _device = device;
            filePathAudio = filepath;
        }

        public Audio(IDevice device)
        {
            _device = device;
        }

        private void userIsSaying(String words)
        {
            if (UserSaying != null)
            {
                if (UserSaying != null)
                {
                    UserSaying(this, new AudioMessage(words));
                }
            }
        }

        public void Say(string words)
        {
            Say( new AudioMessage(words));
        }

        public void Say(IAudioMessage message)
        {
            userIsSaying(message.Message);
        }               

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            userIsSaying(e.Result.Text);
        }

        public event EventHandler<IAudioMessage> UserSaying;

        public void start(List<string> grammar)
        {
            if (_device.Type == DeviceType.KINECT_1)
            {
                _grammar = new Choices();
                foreach (var word in grammar)
                {
                    _grammar.Add(word);
                }
                worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.RunWorkerAsync();
            }
        }

        public void stop()
        {
            if (_device.Type == DeviceType.KINECT_1 && _deviceStarted)
            {
                _deviceStarted = false;
                audioSource.Stop();
                speechRecognizer.RecognizeAsyncStop();
                speechRecognizer.UnloadAllGrammars();
                sre.SpeechRecognized -= SreSpeechRecognized;
                sre.SpeechHypothesized -= SreSpeechHypothesized;
                sre.SpeechRecognitionRejected -= SreSpeechRecognitionRejected;
                worker.DoWork -= new DoWorkEventHandler(worker_DoWork);
            }            
        }



    }
}
