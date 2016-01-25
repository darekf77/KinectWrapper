using Kinect_Wrapper.device.stream.skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Kinect_Wrapper.user
{
    public class KinectUser : IKinectUser
    {
        #region singleton
        private static KinectUser[] _instance;
        static readonly object _locker = new object();
        public static KinectUser[] TempInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new KinectUser[2];
                    _instance[0] = new KinectUser();
                    _instance[1] = new KinectUser();
                }
                return _instance;
            }
        }
        #endregion

        private KinectUser()
        {
            _skeleton = new Skeleton();
            _lastPosition = new Vector3D();
            Console.WriteLine("User created");  
        }

        private ISkeleton _skeleton;
        private Vector3D _lastPosition;
        public ISkeleton Skeleton
        {
            get
            {
                return _skeleton;
            }
            set
            {
                _skeleton = value;
            }
        }

        public Vector3D LastPosition
        {
            get
            {
                return _lastPosition;
            }
            set
            {
                _lastPosition = value;
            }
        }

        public event EventHandler Gone;
    }
}
