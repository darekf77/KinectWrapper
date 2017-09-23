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
using SharedLibJG.Helpers;

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
        private Dictionary<PlayerGestures, int> debouceTimes = new Dictionary<PlayerGestures, int>();
        Action<int> gestureAction;
        Action<int> debouceWrapp = null;
        bool debouceTime = false;

        public GesturesDetector()
        {
            State = GesturesDetectorState.UNACTIVE;
            LastDetectedGesture = PlayerGestures.__NOTHING;

            #region debuce gestures

            debouceTimes[PlayerGestures.__NOTHING] = 0;
            debouceTimes[PlayerGestures.RESIZING] = 100;
            debouceTimes[PlayerGestures.HANDS_ABOVE_HEAD] = 1000;
            debouceTimes[PlayerGestures.HANDS_AEROPLAN] = 1000;
            //debouceTimes[PlayerGestures.LEFT_HAND_ABOVE_HEAD] = 500;
            //debouceTimes[PlayerGestures.RIGHT_HAND_ABOVE_HEAD] = 500;
            debouceTimes[PlayerGestures.SWIPE_LEFT] = 1000;
            debouceTimes[PlayerGestures.SWIPE_RIGHT] = 1000;


            gestureAction = (arg) =>
            {
                LastDetectedGesture = PlayerGestures.__NOTHING;
                debouceTime = false;
            };
            debouceWrapp = gestureAction.Debounce<int>();

            #endregion
        }

        #region start / stpop

        public void start()
        {
            State = GesturesDetectorState.ACTIVE;
            LastDetectedGesture = PlayerGestures.__NOTHING;
        }

        public void stop()
        {
            State = GesturesDetectorState.UNACTIVE;
        }

        #endregion

        #region manual trigger
        public void trigger(PlayerGestures gesture)
        {
            gestureDetectedAction(gesture);
        }
        #endregion

        #region is gesture detected function        
        private bool gestureDetected()
        {
            return LastDetectedGesture != PlayerGestures.__NOTHING && !debouceTime;
        }
        #endregion

        #region update
        private Dictionary<SkeletonDataType, Point> Skeleton;
        public void update(IKinectFrame frame)
        {
            if (gestureDetected() || !frame.IsSkeletonDetected) return;

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
        #endregion

        private void gestureDetectedAction(PlayerGestures gesture)
        {
            if (debouceTime && LastDetectedGesture != gesture) return;
            LastDetectedGesture = gesture;
            if (gesture == PlayerGestures.RESIZING)
            {
                triggerResizeGesture();
                Helpers.SetTimeout(() =>
                {
                    LastDetectedGesture = PlayerGestures.__NOTHING;
                }, 100);
            }
            else if (gesture == PlayerGestures.LEFT_HAND_ABOVE_HEAD || gesture == PlayerGestures.RIGHT_HAND_ABOVE_HEAD)
            {
                onGesture?.Invoke(this, LastDetectedGesture);
                Helpers.SetTimeout(() =>
                {
                    LastDetectedGesture = PlayerGestures.__NOTHING;
                }, 300);
            }
            else
            {
                if (!debouceTime)
                {
                    debouceTime = true;
                    onGesture?.Invoke(this, LastDetectedGesture);
                }
                debouceWrapp(debouceTimes[gesture]);
            }
        }


        #region last detected gesture

        private PlayerGestures _lastDetectedGesture;

        public PlayerGestures LastDetectedGesture
        {
            get { return _lastDetectedGesture; }
            set
            {
                _lastDetectedGesture = value;

                LastDetectedGestureName = (value == PlayerGestures.__NOTHING ? "-" : value.ToString());
            }
        }




        private string _lastDetectedGestureName;

        public string LastDetectedGestureName
        {
            get { return _lastDetectedGestureName; }
            private set
            {
                OnPropertyChanged();
                _lastDetectedGestureName = value;
            }
        }

        #endregion last detected gesture



    }
}






