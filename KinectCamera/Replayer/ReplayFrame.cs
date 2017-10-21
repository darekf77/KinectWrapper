using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Replayer
{
    public class ReplayFrame : ReplayFrameBase
    {
        public FrameColorReplayer Color { get; set; }
        public FrameDepthReplayer Depth { get; set; }
        public FrameSkeletonReplayer Skeleton { get; set; }

        public override long TimeStamp
        {
            get
            {
                return isCorrect ? ((Color.TimeStamp + Depth.TimeStamp + Skeleton.TimeStamp) / 3) : 0;
            }
        }

        public bool isCorrect
        {
            get { return (Color != null && Depth != null && Skeleton != null); }
        }
    }
}
