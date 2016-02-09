using Kinect_Wrapper.structures;
using Kinect_Wrapper.user;
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
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = "Kinect audio thread";
            speechRecognizer = CreateSpeechRecognizer();
            Start();
            _deviceStarted = true;
        }

        

        private void Start()
        {
            if (_sensor == null || !_sensor.IsRunning || _sensor.Status != KinectStatus.Connected)
            {
                Console.WriteLine("Audio initiation fail");
                return;
            }
            //set sensor audio source to variable
            audioSource = _sensor.AudioSource;
            //Set the beam angle mode - the direction the audio beam is pointing
            //we want it to be set to adaptive
            audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            //start the audiosource 
            var kinectStream = audioSource.Start();            
            //configure incoming audio stream
            speechRecognizer.SetInputToAudioStream(
                kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            //make sure the recognizer does not stop after completing     
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            //reduce background and ambient noise for better accuracy
            _sensor.AudioSource.EchoCancellationMode = EchoCancellationMode.None;
            _sensor.AudioSource.AutomaticGainControlEnabled = false;
        }

        private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            //set recognizer info
            RecognizerInfo ri = GetKinectRecognizer();

            Console.WriteLine("Recodnizer description" + ri.Description);
            Console.WriteLine("Recodnizer id" + ri.Id);
            //create instance of SRE
            sre = new SpeechRecognitionEngine(ri.Id);

            //set culture - language, country/region
            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(_grammar);

            //set up the grammar builder
            var g = new Grammar(gb);
            sre.LoadGrammar(g);

            //Set events for recognizing, hypothesising and rejecting speech
            sre.SpeechRecognized += SreSpeechRecognized;
            sre.SpeechHypothesized += SreSpeechHypothesized;
            sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
            return sre;
        }


        private static RecognizerInfo GetKinectRecognizer()
        {
            #region speech recognizer info
            Console.WriteLine("BEFORE");
            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
            {
                Console.WriteLine(String.Format("Id={0}, Name={1}, Description={2}, Culture={3}", ri.Id, ri.Name, ri.Description, ri.Culture));
                foreach (string key in ri.AdditionalInfo.Keys)
                {
                    Console.WriteLine(string.Format("{0} = {1}", key, ri.AdditionalInfo[key]));
                }
                Console.WriteLine("-");
            }

            Console.WriteLine("AFTER");
            #endregion
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                System.Diagnostics.Debug.Write(recognizer.Culture.Name + "\n\n");
                //string value;
                //recognizer.AdditionalInfo.TryGetValue("KinectDevice",out value);
                if ("en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            return null;
        }

        // REJECTED
        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speach rejected :" + e.Result.Text);
        }


        // HYPOTHESIS
        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.WriteLine("Speach hypothesized: " + e.Result.Text + ", confidence:" + e.Result.Confidence);
        }

    }
}
