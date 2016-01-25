using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.stream.skeleton
{
    public interface ISkeleton
    {
        Point RigthHand { get; set; }
        Point LeftHand { get; set; }
        Point Spine { get; set;    }
 
    }
}
