using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Recorder
{
    class DepthRecorder : RecorderBase
    {
        private DepthImagePixel[] _tmpDepthPixels;
        private DepthImagePoint[] _tmpDepthPoints;


        internal DepthRecorder(BinaryWriter writer, KinectSensor sensor) : base(writer, sensor)
        {
            this._tmpDepthPixels = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
            this._tmpDepthPoints = new DepthImagePoint[sensor.DepthStream.FramePixelDataLength];
        }


        public override void update(IDisposable f)
        {
            var frame = f as DepthImageFrame;
            writer.Write((int)FrameType.Depth);

            writeTimestamp();


            writer.Write(frame.BytesPerPixel);
            writer.Write((int)frame.Format);
            writer.Write(frame.Width);
            writer.Write(frame.Height);

            writer.Write(frame.FrameNumber);

            var shorts = new short[frame.PixelDataLength];
            //frame.CopyPixelDataTo(shorts);

            frame.CopyDepthImagePixelDataTo(this._tmpDepthPixels);
            sensor.CoordinateMapper.MapColorFrameToDepthFrame(
                ColorImageFormat.RgbResolution640x480Fps30,
                DepthImageFormat.Resolution640x480Fps30,
                this._tmpDepthPixels,
                this._tmpDepthPoints
                );

            for (int i = 0; i < shorts.Length; i++)
            {
                shorts[i] = (short)this._tmpDepthPoints[i].Depth;
            }

            writer.Write(shorts.Length);
            foreach (var s in shorts)
                writer.Write(s);

        }
    }
}
