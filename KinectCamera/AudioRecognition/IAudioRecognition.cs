using SharedLibJG.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera
{
    [Serializable]
    public class GrammarTest : INotifyPropertyChanged
    {
        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private string _wasJustSayed = "no";

        public string WasJustSayed
        {
            get { return _wasJustSayed; }
            set { _wasJustSayed = value; OnPropertyChanged(); }
        }


    }


    public interface IAudioRecognition : INotifyPropertyChanged
    {
        bool IsEnable { get; set; }
        void init(IAudioSourceDevice source);

        event EventHandler<IAudioMessage> UserSaying;
        ObservableCollection<String> Grammar { get; set; }
        TrulyObservableCollection<GrammarTest> ListGrammar { get; }
    }
}
