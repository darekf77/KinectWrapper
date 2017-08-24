using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kinect_Wrapper.frame;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharedLibJG.models;
using System.Drawing;

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
        public event EventHandler<float> onResizeGesture;

        public GesturesDetector()
        {
            State = GesturesDetectorState.UNACTIVE;


            rmLabel = (arg) =>
            {
                this.LastDetectedGesture = "-";
                debouceWrapp = null;
                gestureDetected = false;
                Console.WriteLine("debouce successs");
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
            gestureDetectedAction(gesture);
        }


        Action<int> debouceWrapp = null;
        private Dictionary<SkeletonDataType, Point> Skeleton;
        private bool gestureDetected = false;
        public void update(IKinectFrame frame)
        {
            if (gestureDetected) return;
            this.Skeleton = frame.UserSkeleton;
            if (State == GesturesDetectorState.ACTIVE)
            {
                foreach (PlayerGestures gesture in Enum.GetValues(typeof(PlayerGestures)))
                {
                    if (checkSkeleton(gesture))
                    {
                        gestureDetectedAction(gesture);
                        return;
                    }
                }
            }
        }

        private void gestureDetectedAction(PlayerGestures gesture)
        {
            if (debouceWrapp == null)
            {
                debouceWrapp = rmLabel.Debounce<int>();
            }
            gestureDetected = true;
            LastDetectedGesture = gesture.ToString();
            onGesture?.Invoke(this, gesture);
            Console.WriteLine("debouce start");
            debouceWrapp(1000);
        }

        

        private string _lastDetectedGesture;

        public string LastDetectedGesture
        {
            get { return _lastDetectedGesture; }
            private set
            {
                OnPropertyChanged();
                _lastDetectedGesture = value;
            }
        }




    }
}






