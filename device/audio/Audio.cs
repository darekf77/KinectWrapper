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
        private BackgroundWorker worker;     
        
        public SpeechRecognitionEngine SpeechRecognizer { get; private set; }
        public ObservableCollection<String> Grammar { get; set; }

        private KinectAudioSource audioSource;
        private RecognizerInfo recognizerInfo = GetRecognizer();

        private Boolean audioStarted = false;

        public Audio() {
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


    }
}
