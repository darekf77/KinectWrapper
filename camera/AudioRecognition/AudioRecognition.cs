using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kinect_Wrapper.device.audio.message;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using Microsoft.Kinect;
using System.IO;

namespace Kinect_Wrapper.camera
{
    public class AudioRecognition : IAudioRecognition
    {


        private SpeechRecognitionEngine SpeechRecognizer { get; set; }

        public event EventHandler<IAudioMessage> UserSaying;

        public AudioRecognition()
        {

        }

        Task AudioThread;
        public void init(IAudioSourceDevice source)
        {
            destroy();
            if (AudioThread != null) AudioThread.Dispose();
            AudioThread = Task.Factory.StartNew(() =>
            {
                initialize();
                try
                {
                    switch (source.Type)
                    {
                        case AudioSourceType.Kinect:
                            #region speeach recognizer + kienct
                            var audioSource = source.KinectAudio;
                            audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
                            audioSource.EchoCancellationMode = EchoCancellationMode.CancellationOnly;
                            audioSource.AutomaticGainControlEnabled = true;
                            var kinectStream = audioSource.Start();
                            //configure incoming audio stream
                            SpeechRecognizer.SetInputToAudioStream(
                                kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                            #endregion
                            break;
                        case AudioSourceType.File:
                            #region speach recognizer + wav file
                            SpeechRecognizer.SetInputToAudioStream(
                              File.OpenRead(source.Path),
                              new SpeechAudioFormatInfo(
                                44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono));
                            #endregion
                            break;
                        case AudioSourceType.OtherDevice:
                            #region speach recognition + other audio devices
                            SpeechRecognizer.SetInputToAudioStream(source.Stream, new SpeechAudioFormatInfo(
                                44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono));
                            break;
                        #endregion
                        default:
                            break;
                    }
                    SpeechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        #region initializer / destroy
        private void initialize()
        {
            SpeechRecognizer = new SpeechRecognitionEngine(recognizerInfo.Id);
            SpeechRecognizer.LoadGrammar(GetCurrentGrammar());
            SpeechRecognizer.SpeechRecognized += SreSpeechRecognized;
            SpeechRecognizer.SpeechHypothesized += SreSpeechHypothesized;
            SpeechRecognizer.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
        }

        private void destroy()
        {
            SpeechRecognizer.UnloadAllGrammars();
            SpeechRecognizer.SpeechRecognized -= SreSpeechRecognized;
            SpeechRecognizer.SpeechHypothesized -= SreSpeechHypothesized;
            SpeechRecognizer.SpeechRecognitionRejected -= SreSpeechRecognitionRejected;
            SpeechRecognizer.RecognizeAsyncCancel();
            SpeechRecognizer = null;
        }
        #endregion

        #region is enable
        private bool _IsEnable = true;

        public bool IsEnable
        {
            get { return _IsEnable; }
            set { _IsEnable = value; }
        }
        #endregion

        #region microsoft speach recognizer 
        private RecognizerInfo recognizerInfo = GetRecognizer();
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
        #endregion

        #region grammar
        public ObservableCollection<string> Grammar { get; set; }
        void initGrammar()
        {
            Grammar = new ObservableCollection<String>();
            Grammar.CollectionChanged += Grammar_CollectionChanged;
            Grammar.Add("test");
        }
        private void Grammar_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SpeechRecognizer.LoadGrammarAsync(GetCurrentGrammar());
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
        #endregion

        #region user voice recognized
        // GOOD SPEACH
        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (!IsEnable) return;
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
        #endregion

    }
}
