using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kinect_Wrapper.device.stream
{
    public class DepthStream : StreamBase
    {
        public DepthStream()
            : base()
        {
            Name = "Depth Stream";
            Description = "Depth stream from kinect";
        }

        public override ImageSource update(frame.IKinectFrame frame)
        {
            if (frame.DepthColor != null)
            {
                _bitmapaDepth.WritePixels(
                new System.Windows.Int32Rect(0, 0,
                    _bitmapaDepth.PixelWidth,
                    _bitmapaDepth.PixelHeight),
                frame.DepthColor,
                _bitmapaDepth.PixelWidth,
                0);
            }
            return _bitmapaDepth;
        }
    }
}
