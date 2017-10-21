using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using Microsoft.Kinect;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using SharedLibJG.Helpers;

namespace Kinect_Wrapper.camera
{
    public class AudioRecognition : IAudioRecognition
    {
        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region grammar list with detection 
        public ObservableCollection<GrammarTest> ListGrammar { get; private set; }
        void rebuildGrammarList()
        {
            ListGrammar.Clear();
            foreach (var g in Grammar)
            {
                var lg = new GrammarTest();
                lg.Name = g;
                lg.WasJustSayed = "no";
                ListGrammar.Add(lg);
            }
        }
        #endregion

        private SpeechRecognitionEngine SpeechRecognizer { get; set; }
        public event EventHandler<IAudioMessage> UserSaying;

        #region constructor
        public AudioRecognition()
        {
            Grammar = new ObservableCollection<String>();
            ListGrammar = new TrulyObservableCollection<GrammarTest>();
            Grammar.CollectionChanged += (e, v) =>
            {
                SpeechRecognizer?.LoadGrammarAsync(GetCurrentGrammar());
                rebuildGrammarList();
            };
            Grammar.Add("test");
            #region grammar list with detection
            rebuildGrammarList();
            UserSaying += (e, v) =>
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    GrammarTest t = null;
                    foreach (var g in ListGrammar)
                    {
                        if (v.Message == g.Name)
                        {
                            t = g;
                            Console.WriteLine("start");
                            g.WasJustSayed = "yes";
                            break;
                        }
                    }
                    if (t != null)
                    {
                        Helpers.SetTimeout(() =>
                        {
                            App.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                Console.WriteLine("stop");
                                t.WasJustSayed = "no";
                            }));
                        }, 2000);
                    }
                }));
            };
            #endregion
        }
        #endregion

        #region init thread stream + recognizer
        Task AudioThread;
        #region cancelation token
        private CancellationToken getToken()
        {
            tokenSource = new CancellationTokenSource();
            return tokenSource.Token;
        }
        CancellationTokenSource tokenSource;
        CancellationToken cancelationTaskToken;
        #endregion
        public void init(IAudioSourceDevice source)
        {
            destroy();
            tokenSource?.Cancel();
            cancelationTaskToken = getToken();
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
                            audioSource.EchoCancellationMode = EchoCancellationMode.CancellationAndSuppression;
                            audioSource.AutomaticGainControlEnabled = true;

                            var kinectStream = audioSource.Start();
                            //configure incoming audio stream
                            SpeechRecognizer.SetInputToAudioStream(
                                kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                            #endregion
                            SpeechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                            break;
                        case AudioSourceType.File:
                            #region speach recognizer + wav file
                            SpeechRecognizer.SetInputToAudioStream(
                              File.OpenRead(source.Path),
                              new SpeechAudioFormatInfo(
                                44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono));
                            SpeechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                            #endregion
                            break;
                        case AudioSourceType.OtherDevice:
                            #region speach recognition + other audio devices

                            // TODO how to get live device stream

                            //source.Player.onStreamReady += (e, v) =>
                            //{
                            //    SpeechRecognizer.SetInputToWaveStream(source.Stream);
                            //    SpeechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                            //};
                            break;
                        #endregion
                        default:
                            break;
                    }
                    SpeechRecognizer?.LoadGrammarAsync(GetCurrentGrammar());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }, cancelationTaskToken);
        }
        #endregion

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
            if (SpeechRecognizer == null) return;
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

        #region get microsoft speach recognizer from stystem
        private RecognizerInfo recognizerInfo = GetRecognizer();
        private static RecognizerInfo GetRecognizer()
        {
            #region speech recognizer info
            //Console.WriteLine("BEFORE");
            //foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
            //{
            //    Console.WriteLine(String.Format("Id={0}, Name={1}, Description={2}, Culture={3}", ri.Id, ri.Name, ri.Description, ri.Culture));
            //    foreach (string key in ri.AdditionalInfo.Keys)
            //    {
            //        Console.WriteLine(string.Format("{0} = {1}", key, ri.AdditionalInfo[key]));
            //    }
            //    Console.WriteLine("-");
            //}

            //Console.WriteLine("AFTER");
            #endregion
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                System.Diagnostics.Debug.Write(recognizer.Culture.Name + "\n\n");
                //string value;
                //recognizer.AdditionalInfo.TryGetValue("KinectDevice",out value);
                if ("en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Recodnizer description" + recognizer.Description);
                    //Console.WriteLine("Recodnizer id" + recognizer.Id);
                    return recognizer;
                }
            }

            return null;
        }
        #endregion

        #region grammar
        public ObservableCollection<string> Grammar { get; set; }
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
            UserSaying?.Invoke(this, new AudioMessage(e.Result.Text, e.Result.Confidence));
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
