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
    public abstract partial class AudioBase:IAudio,INotifyPropertyChanged
    {        
        private BackgroundWorker worker;     
        
        public SpeechRecognitionEngine SpeechRecognizer { get; private set; }
        public ObservableCollection<String> Grammar { get; set; }

        private bool _isAvailable = false;
        public bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }
            private set
            {
                _isAvailable = value;
                OnPropertyChanged("IsAvailable");
            }
        }

        private bool _isRecordingPossible = false;
        public bool IsRecordingPossible
        {
            get
            {
                return _isRecordingPossible;
            }
             private set
            {
                _isRecordingPossible = value;
                OnPropertyChanged("IsRecordingPossible");
            }
        }

        private bool _isEnable = true;
        public bool IsEnable
        {
            get
            {
                return _isEnable;
            }

            set
            {
                _isEnable = value;
                OnPropertyChanged("IsEnable");
            }
        }

        private bool _isRecordingEnable = false;
        public bool IsRecordingEnable
        {
            get
            {
                return _isRecordingEnable;
            }
            set
            {
                _isRecordingEnable = value;
                OnPropertyChanged("IsRecordingEnable");
            }
        }

        private KinectAudioSource audioSource;
        private RecognizerInfo recognizerInfo = GetRecognizer();

        private Boolean audioStarted = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public AudioBase() {
            Grammar = new ObservableCollection<String>();
            Grammar.CollectionChanged += Grammar_CollectionChanged;
        }

        private void Grammar_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(audioStarted) SpeechRecognizer.LoadGrammarAsync(GetCurrentGrammar());
        }

        private Grammar GetCurrentGrammar()
        {
            var gb = new GrammarBuilder { Culture = recognizerInfo.Culture };
            var choices = new Choices();
            foreach (var word in Grammar)
            {
                choices.Add(word);
            }
            gb.Append(choices);
            //set up the grammar builder
            return new Grammar(gb);
        }

       

        private static RecognizerInfo GetRecognizer()
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
                    Console.WriteLine("Recodnizer description" + recognizer.Description);
                    Console.WriteLine("Recodnizer id" + recognizer.Id);
                    return recognizer;
                }
            }

            return null;
        }

        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
