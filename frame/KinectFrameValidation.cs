using Kinect_Wrapper.structures;
using Kinect_Wrapper.wrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Kinect_Wrapper.frame
{
    public partial class KinectFrame
    {
        private int _noDetectedCounter = 0;
        private int _noDetectedThreshold = 60;
        private bool _isSkeletonDetected = false;
        public bool IsSkeletonDetected
        {
            get
            {
                return _isSkeletonDetected;
            }
            set
            {
                if(_isCreation)
                {
                    if (value) _isSkeletonDetected = true;
                    else if (!IsPauseMode) ++_noDetectedCounter;
                    if (_noDetectedCounter == _noDetectedThreshold)
                    {
                        _isSkeletonDetected = false;
                        _noDetectedCounter = 0;
                    }
                }
                else
                {
                    _isSkeletonDetected = value;
                }
            }
        }

        private bool _isCreation = false;
        private Dictionary<SkeletonDataType, Point> skeleton = new Dictionary<SkeletonDataType, Point>();
        public Dictionary<SkeletonDataType, Point> UserSkeleton
        {
            get { return skeleton; }
            set
            {
                if(_isCreation)
                {
                    foreach (var item in value)
                    {
                        if (item.Value.X < 0 || item.Value.Y < 0) continue;
                        skeleton[item.Key] = item.Value;
                    }
                }
                else
                {
                    foreach (var item in value)
                    {
                        if(item.Value.X < 0 || item.Value.Y < 0) continue;
                        skeleton[item.Key] = item.Value;
                    }
                }
                
            }
        }

        private bool IsPauseMode { get; set; }
    }
}
