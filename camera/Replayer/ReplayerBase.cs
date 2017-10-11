using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Replayer
{
    public abstract class ReplayerBase
    {
        public virtual int FrameNumber { get; protected set; }
        public virtual long TimeStamp { get; protected set; }

        internal abstract void CreateFromReader(BinaryReader reader);

        protected Stream stream { get; set; }
        protected BinaryReader streamReader { get; set; }
        protected long streamPosition { get; set; }
        protected int Width { get; set; }
        protected int Height { get; set; }
        protected int PixelDataLength { get; set; }
        protected int BytesPerPixel { get; set; }

    }
}
