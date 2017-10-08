using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera.AudioRecordDevice
{
    public interface IAudioRecordDevice
    {
        int Id { get; }
        string Name { get; }

    }
}
