using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kinect_Wrapper.frame;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kinect_Wrapper.gestures
{
    public partial class GesturesDetector : IGesturesDetector, INotifyPropertyChanged
    {


        #region propety changed
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public GesturesDetectorState State { get; private set; }
        public event EventHandler<PlayerGestures> onGesture;

        public GesturesDetector()
        {
            State = GesturesDetectorState.UNACTIVE;


            rmLabel = (arg) =>
            {
                this.LastDetectedGesture = "-";
                debouceWrapp = null;
            };

            


        }

        Action<int> rmLabel;


        public void start()
        {
            State = GesturesDetectorState.ACTIVE;
        }

        public void stop()
        {
            State = GesturesDetectorState.UNACTIVE;
        }

        public void trigger(PlayerGestures gesture)
        {
            onGesture?.Invoke(this, gesture);
        }


        Action<int> debouceWrapp = null;
        public void update(IKinectFrame frame)
        {
            if(State == GesturesDetectorState.ACTIVE)
            {
                if(debouceWrapp == null)
                {
                    debouceWrapp = rmLabel.Debounce<int>();
                }
                
                foreach (PlayerGestures gesture in Enum.GetValues(typeof(PlayerGestures)))
                {
                    if (detect(gesture))
                    {
                        LastDetectedGesture = gesture.ToString();
                        onGesture?.Invoke(this, gesture);
                        debouceWrapp(1000);
                    }
                }
            }
        }


        private string _lastDetectedGesture;

        public string LastDetectedGesture
        {
            get { return _lastDetectedGesture; }
            private set {
                OnPropertyChanged();
                _lastDetectedGesture = value;
            }
        }




    }
}






