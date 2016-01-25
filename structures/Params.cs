using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.structures
{
    [Serializable]
    public class Params
    {
        private int _videoWidth = 640;

        public int VideoWidth
        {
            get { return _videoWidth; }
            set { _videoWidth = value; }
        }

        private int _videoHeight = 480;

        public int VideoHeight
        {
            get { return _videoHeight; }
            set { _videoHeight = value; }
        }

        private int _lengthDataPixelsColor;

        public int LengthDataPixelsColor
        {
            get { return _lengthDataPixelsColor; }
            set { _lengthDataPixelsColor = value; }
        }

        private int _lengthDataPixelsDepth;

        public int LengthDataPixelsDepth
        {
            get { return _lengthDataPixelsDepth; }
            set { _lengthDataPixelsDepth = value; }
        }
        
        


    }
}
