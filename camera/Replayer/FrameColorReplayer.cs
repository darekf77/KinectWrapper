using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Replayer
{
    public class FrameColorReplayer : ReplayFrameBase
    {
        private readonly ColorImageFrame internalFrame;
        public ColorImageFormat Format { get; private set; }

        #region constructor
        public FrameColorReplayer(BinaryReader reader)
        {
            TimeStamp = reader.ReadInt64();
            BytesPerPixel = reader.ReadInt32();
            Format = (ColorImageFormat)reader.ReadInt32();
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            FrameNumber = reader.ReadInt32();

            PixelDataLength = reader.ReadInt32();

            stream = reader.BaseStream;
            streamPosition = stream.Position;

            stream.Position += PixelDataLength;
        }
        public FrameColorReplayer(ColorImageFrame frame)
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

        #region copy color pixel data
        public void CopyPixelDataTo(byte[] pixelData)
        {
            if (internalFrame != null)
            {
                internalFrame.CopyPixelDataTo(pixelData);
                return;
            }

            var savedPosition = stream.Position;
            stream.Position = streamPosition;

            stream.Read(pixelData, 0, PixelDataLength);

            stream.Position = savedPosition;
        }
        #endregion

        public static implicit operator FrameColorReplayer(ColorImageFrame frame)
        {
            return new FrameColorReplayer(frame);
        }

    }
}
