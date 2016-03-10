using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.structures
{
    public enum DeviceState
    {
        NOT_READY,
        INITIALIZING,
        IS_READY,
        IS_WORKING_PLAYING,
        IS_WORKING_RECORDING,
        STOPPING
    }
}
