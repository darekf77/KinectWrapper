﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.statistic
{
    public interface IStatistics
    {
        int FramesPerSecond(StatFrameType frameType);
        void commitFrame(StatFrameType frameType);
    }
}
