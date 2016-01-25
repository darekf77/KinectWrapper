using Kinect_Wrapper.device.stream.skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Kinect_Wrapper.user
{
    public interface IKinectUser
    {
        /// <summary>
        /// Skeleton information in frame
        /// </summary>
        ISkeleton Skeleton { get; set; }

        /// <summary>
        /// Last user position
        /// </summary>
        Vector3D LastPosition { get; set; }

        /// <summary>
        /// User not exist anymore
        /// </summary>
        event EventHandler Gone;
    }
}
