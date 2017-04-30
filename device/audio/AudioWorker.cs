using Kinect_Wrapper.structures;
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
    public abstract partial class AudioBase
    {
        

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = "Kinect audio thread";
            if(_device.Type == DeviceType.KINECT_1)
            {
                try
                {
                    SpeechRecognizer = new SpeechRecognitionEngine(recognizerInfo.Id);
                    SpeechRecognizer.LoadGrammar(GetCurrentGrammar());
                    SpeechRecognizer.SpeechRecognized += SreSpeechRecognized;
                    SpeechRecognizer.SpeechHypothesized += SreSpeechHypothesized;
                    SpeechRecognizer.SpeechRecognitionRejected += SreSpeechRecognitionRejected;

                    //set sensor audio source to variable
                    audioSource = _device.sensor.AudioSource;
                    //Set the beam angle mode - the direction the audio beam is pointing
                    //we want it to be set to adaptive
                    audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
                    //start the audiosource 
                    var kinectStream = audioSource.Start();
                    //configure incoming audio stream
                    SpeechRecognizer.SetInputToAudioStream(
                        kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                    //make sure the recognizer does not stop after completing     
                    SpeechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                    //reduce background and ambient noise for better accuracy
                    _device.sensor.AudioSource.EchoCancellationMode = EchoCancellationMode.None;
                    _device.sensor.AudioSource.AutomaticGainControlEnabled = false;
                    audioStarted = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
            Console.WriteLine("END OF WORKER AUDIO");            
        }      

        

    }
}
