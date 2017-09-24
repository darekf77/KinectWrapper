
using Kinect.Replay.Replay.Color;
using Kinect.Replay.Replay.Depth;
using Kinect.Replay.Replay.Skeletons;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;
using SharedLibJG.models;
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
    public interface IKinectFrame : DimensionalFrame
    {
        void synchronize(
            ReplayDepthImageFrame depthFrame,
            ReplayColorImageFrame colorFrame,
            ReplaySkeletonFrame skletonFrame,
            Boolean isPauseMode
            );
        void synchronize(
            DepthImageFrame depthFrame,
            ColorImageFrame colorFrame,
            SkeletonFrame skletonFrame,
            BackgroundRemovedColorStream backgroundRemovedColorStream,
            Boolean isPauseMode
            );
        void synchronize(String message, Boolean isVisible, Boolean isPauseMode);

        /// <summary>
        /// For tracker tester purpose
        /// </summary>
        bool IsIgnoringDepthRange { get; set; }

    }
}
