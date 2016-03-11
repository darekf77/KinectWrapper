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
    public abstract partial class AudioBase
    {
        private IDevice _device;

        public IDevice CurrentDevice
        {
            get
            {
                return _device;
            }

            set
            {
                if (value == null)
                {
                    IsEnable = false;
                    IsAvailable = false;
                    IsRecordingEnable = false;
                    IsRecordingPossible = false;
                    //audioSource.Stop();
                    if (SpeechRecognizer != null)
                    {
                        //SpeechRecognizer.RecognizeAsyncStop();
                        SpeechRecognizer.UnloadAllGrammars();
                        SpeechRecognizer.SpeechRecognized -= SreSpeechRecognized;
                        SpeechRecognizer.SpeechHypothesized -= SreSpeechHypothesized;
                        SpeechRecognizer.SpeechRecognitionRejected -= SreSpeechRecognitionRejected;
                        SpeechRecognizer.RecognizeAsyncCancel();
                        SpeechRecognizer = null;
                    }
                    if (worker != null)
                    {
                        //worker.CancelAsync();
                        worker.DoWork -= new DoWorkEventHandler(worker_DoWork);
                        //worker.Dispose();
                        worker = null;
                    }                    
                    return;
                }
                _device = value;
                if (_device.Type == DeviceType.KINECT_1)
                {
                    IsEnable = true;
                    IsAvailable = true;
                    IsRecordingEnable = true;
                    IsRecordingPossible = true;
                    worker = new BackgroundWorker();
                    worker.DoWork += worker_DoWork;
                    worker.RunWorkerAsync();
                }
                else
                {
                    IsEnable = false;
                    IsAvailable = false;
                    IsRecordingEnable = false;
                    IsRecordingPossible = false;
                }

            }
        }


    }
}
