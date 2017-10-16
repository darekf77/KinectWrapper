using Kinect_Wrapper.camera.Replayer;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
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
            FrameDepthReplayer depthFrame,
            FrameColorReplayer colorFrame,
            FrameSkeletonReplayer skletonFrame,
            Boolean isPauseMode
            );
        void synchronize(
            DepthImageFrame depthFrame,
            ColorImageFrame colorFrame,
            SkeletonFrame skletonFrame,
            CoordinateMapper CoordinateMapper,
            Boolean isPauseMode
            );
        void synchronize(String message, Boolean isVisible, Boolean isPauseMode);

        /// <summary>
        /// For tracker tester purpose
        /// </summary>
        bool IsIgnoringDepthRange { get; set; }

    }
}
