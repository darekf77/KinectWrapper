using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Recorder
{
    public class ColorRecorder : RecorderBase
    {

        internal ColorRecorder(BinaryWriter writer, KinectSensor sensor) : base(writer, sensor)
        {
        }


        public override void update(IDisposable f)
        {
            var frame = f as ColorImageFrame;

            writer.Write((int)FrameType.Color);

            writeTimestamp();

            writer.Write(frame.BytesPerPixel);
            writer.Write((int)frame.Format);
            writer.Write(frame.Width);
            writer.Write(frame.Height);

            writer.Write(frame.FrameNumber);

            writer.Write(frame.PixelDataLength);
            var bytes = new byte[frame.PixelDataLength];
            frame.CopyPixelDataTo(bytes);
            writer.Write(bytes);
        }
    }
}
