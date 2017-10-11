using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Replayer
{
    public class FrameDepthReplayer : ReplayFrameBase
    {
        private readonly DepthImageFrame internalFrame;
        public DepthImageFormat Format { get; private set; }

        #region constructor
        public FrameDepthReplayer(BinaryReader reader)
        {
            TimeStamp = reader.ReadInt64();
            BytesPerPixel = reader.ReadInt32();
            Format = (DepthImageFormat)reader.ReadInt32();
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            FrameNumber = reader.ReadInt32();

            PixelDataLength = reader.ReadInt32();

            stream = reader.BaseStream;
            streamReader = reader;
            streamPosition = stream.Position;

            stream.Position += PixelDataLength * 2;
        }
        public FrameDepthReplayer(DepthImageFrame frame)
        {
            Format = frame.Format;
            BytesPerPixel = frame.BytesPerPixel;
            FrameNumber = frame.FrameNumber;
            TimeStamp = frame.Timestamp;
            Width = frame.Width;
            Height = frame.Height;

            PixelDataLength = frame.PixelDataLength;
            internalFrame = frame;
        }
        #endregion

        #region copy depth pixel data
        public void CopyPixelDataTo(short[] pixelData)
        {
            if (internalFrame != null)
            {
                internalFrame.CopyPixelDataTo(pixelData);
                return;
            }

            var savedPosition = stream.Position;
            stream.Position = streamPosition;

            for (var index = 0; index < PixelDataLength; index++)
                pixelData[index] = streamReader.ReadInt16();

            stream.Position = savedPosition;
        }
        #endregion

        public static implicit operator FrameDepthReplayer(DepthImageFrame frame)
        {
            return new FrameDepthReplayer(frame);
        }
    }
}
