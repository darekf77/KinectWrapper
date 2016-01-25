using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.stream.skeleton
{
    public partial class Skeleton:ISkeleton
    {
        public Skeleton()
        {
            RigthHand = new Point();
            LeftHand = new Point();
            Spine = new Point();
        }
        public Point RigthHand { get; set; }
        public Point LeftHand { get; set; }
        public Point Spine { get; set; }
    }
}
