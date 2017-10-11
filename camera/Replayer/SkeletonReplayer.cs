using Kinect_Wrapper.camera.Recorder;
using Microsoft.Kinect;
using SharedLibJG.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Replayer
{
    public class SkeletonReplayer : ReplayerBase
    {
        public Tuple<float, float, float, float> FloorClipPlane { get; private set; }
        public SkeletonTrackingMode TrackingMode { get; set; }
        public IDictionary<SkeletonDataType, int> jointsY = new Dictionary<SkeletonDataType, int>();
        public IDictionary<SkeletonDataType, int> jointsX = new Dictionary<SkeletonDataType, int>();
        public bool IsSkeletonDetected { get; set; }

        #region constructor
        public SkeletonReplayer(SkeletonFrame frame)
        {
            FloorClipPlane = frame.FloorClipPlane;
            FrameNumber = frame.FrameNumber;
            TimeStamp = frame.Timestamp;
            //Skeletons = frame.GetSkeletons();
            TrackingMode = frame.TrackingMode;
        }
        #endregion

        #region create skeleton from reader
        internal override void CreateFromReader(BinaryReader reader)
        {
            TimeStamp = reader.ReadInt64();
            TrackingMode = (SkeletonTrackingMode)reader.ReadInt32();
            FloorClipPlane = new Tuple<float, float, float, float>(
                reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle());

            FrameNumber = reader.ReadInt32();

            foreach (var joint in SkeletonRecorder.neeededJoints)
            {
                jointsX[(SkeletonDataType)joint] = reader.ReadInt32();
                jointsY[(SkeletonDataType)joint] = reader.ReadInt32();
            }

            IsSkeletonDetected = reader.ReadBoolean();
        }
        #endregion

        public static implicit operator SkeletonReplayer(SkeletonFrame frame)
        {
            return new SkeletonReplayer(frame);
        }
    }
}
