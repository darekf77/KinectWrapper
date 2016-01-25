using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.statistic
{
    public interface IStatistics
    {
        int FramesPerSecond(FrameType frameType);
        void commitFrame(FrameType frameType);
    }
}
