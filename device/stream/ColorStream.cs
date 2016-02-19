using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kinect_Wrapper.device.stream
{
    public class ColorStream : StreamBase
    {
        public ColorStream()
            : base()
        {
            Name = "Color Stream";
            Description = "Color stream from kinect";
        }

        public ColorStream(String name, String description)
            : base()
        {
            Name = name;
            Description = description;
        }

    }
}
