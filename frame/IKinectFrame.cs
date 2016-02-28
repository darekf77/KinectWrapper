
using Kinect.Replay.Replay.Color;
using Kinect.Replay.Replay.Depth;
using Kinect.Replay.Replay.Skeletons;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.frame
{
    /// <summary>
    /// Full synchronized frame with depth,color and skeleton data
    /// </summary>
    public interface IKinectFrame
    {
        Dictionary<SkeletonDataType, Point> UserSkeleton { get; set; }

        Boolean IsSkeletonDetected { get; set; }

        /// <summary>
        /// get rgba color stream byte
        /// </summary>
        Byte[] Color { get; set;  }

        /// <summary>
        /// get gray pixel 
        /// </summary>
        short[] Depth { get; set; }

        /// <summary>
        /// Gray pixel ready to displa
        /// You can set this property once and get proper value after once
        /// otherwiese this value will be calculated from Depth
        /// </summary>
        Byte[] DepthColor { get; set; }
        

        void synchronize(ReplayDepthImageFrame depthFrame, ReplayColorImageFrame colorFrame, ReplaySkeletonFrame skletonFrame);
        void synchronize(DepthImageFrame depthFrame, ColorImageFrame colorFrame, SkeletonFrame skletonFrame);
        void synchronize(String message,Boolean isVisible);

    }
}
