﻿using Kinect_Wrapper.camera.Recorder;
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
    public class FrameSkeletonReplayer : ReplayFrameBase
    {
        public Tuple<float, float, float, float> FloorClipPlane { get; private set; }
        public SkeletonTrackingMode TrackingMode { get; set; }
        public IDictionary<SkeletonDataType, int> jointsY = new Dictionary<SkeletonDataType, int>();
        public IDictionary<SkeletonDataType, int> jointsX = new Dictionary<SkeletonDataType, int>();
        public bool IsSkeletonDetected { get; set; }

        #region constructor
        public FrameSkeletonReplayer(BinaryReader reader)
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
        public FrameSkeletonReplayer(SkeletonFrame frame)
        {
            FloorClipPlane = frame.FloorClipPlane;
            FrameNumber = frame.FrameNumber;
            TimeStamp = frame.Timestamp;
            //Skeletons = frame.GetSkeletons();
            TrackingMode = frame.TrackingMode;
        }
        #endregion

        public static implicit operator FrameSkeletonReplayer(SkeletonFrame frame)
        {
            return new FrameSkeletonReplayer(frame);
        }
    }
}
