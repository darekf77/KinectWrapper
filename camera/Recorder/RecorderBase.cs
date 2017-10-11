using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.Recorder
{
    public abstract class RecorderBase
    {
        public RecorderBase(BinaryWriter writer, KinectSensor sensor)
        {
            this.writer = writer;
            this.sensor = sensor;
            referenceTime = DateTime.Now;
        }
        public abstract void update(IDisposable frame);

        protected KinectSensor sensor;
        protected BinaryWriter writer; // TODO should be readonly ????
        protected DateTime referenceTime;

        protected void writeTimestamp()
        {
            var timeSpan = DateTime.Now.Subtract(referenceTime);
            referenceTime = DateTime.Now;
            writer.Write((long)timeSpan.TotalMilliseconds);
        }

    }
}
