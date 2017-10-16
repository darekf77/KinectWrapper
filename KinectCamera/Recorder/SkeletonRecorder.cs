using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Recorder
{
    class SkeletonRecorder : RecorderBase
    {
        internal SkeletonRecorder(BinaryWriter writer, KinectSensor sensor) : base(writer, sensor)
        {

        }

        private static IDictionary<JointType, ColorImagePoint> bodyPoints = new Dictionary<JointType, ColorImagePoint>();
        public static JointType[] neeededJoints = {
            JointType.HandLeft,
            JointType.HandRight,
            JointType.WristLeft,
            JointType.WristRight,
            JointType.ShoulderLeft,
            JointType.ShoulderRight,
            JointType.ElbowRight,
            JointType.ElbowLeft,
            JointType.Spine,
            JointType.ShoulderCenter,
            JointType.Head,
            JointType.HipCenter
        };

        private static void save(Skeleton skeleton, JointType joint, BinaryWriter writer, KinectSensor sensor)
        {
            var point = sensor.CoordinateMapper.
                    MapSkeletonPointToColorPoint(
                    skeleton.Joints[joint].Position,
                    ColorImageFormat.RgbResolution640x480Fps30);
            writer.Write(point.X);
            writer.Write(point.Y);
        }

        public override void update(IDisposable f)
        {
            var frame = f as SkeletonFrame;
            writer.Write((int)FrameType.Skeletons);

            writeTimestamp();

            writer.Write((int)frame.TrackingMode);
            writer.Write(frame.FloorClipPlane.Item1);
            writer.Write(frame.FloorClipPlane.Item2);
            writer.Write(frame.FloorClipPlane.Item3);
            writer.Write(frame.FloorClipPlane.Item4);

            writer.Write(frame.FrameNumber);

            //var skeletons = frame.GetSkeletons();

            frame.CopySkeletonDataTo(totalSkeleton);
            firstSkeleton = (from trackskeleton in totalSkeleton
                             where trackskeleton.TrackingState
                             == SkeletonTrackingState.Tracked
                             select trackskeleton).FirstOrDefault();


            if (firstSkeleton != null &&
                firstSkeleton.Joints[JointType.Spine].TrackingState == JointTrackingState.Tracked)
            {
                foreach (var item in neeededJoints)
                {
                    save(firstSkeleton, item, writer, sensor);
                }
                writer.Write(true); // is skleton detected
            }
            else
            {
                foreach (var item in neeededJoints)
                {
                    writer.Write(0);
                    writer.Write(0);
                }
                writer.Write(false); // is skleton detected
            }

            //frame.CopySkeletonDataTo(skeletons);

            //var formatter = new BinaryFormatter();
            //formatter.Serialize(writer.BaseStream, skeletons);
        }

        Skeleton firstSkeleton;
        Skeleton[] totalSkeleton = new Skeleton[6];


    }
}
