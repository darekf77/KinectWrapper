using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera
{
    public enum CameraState
    {
        UNACTIVE = 0,
        PLAYING = 10,
        PLAYING_PAUSE = 11,
        PLAYING_STOPPING = 12,
        RECORDING = 22,
        RECORDING_PAUSE = 23,
        RECORDING_STOPPING = 24,
        RECORDING_CANCEL = 25
    }

}


static class EnumRange
{
    public static SharedLibJG.Helpers.Range EnumGroupRange(this Kinect_Wrapper.camera.CameraState state)
    {
        var playingRange = new SharedLibJG.Helpers.Range();
        switch (state)
        {
            case Kinect_Wrapper.camera.CameraState.UNACTIVE:
                return new SharedLibJG.Helpers.Range(0, 9);
            case Kinect_Wrapper.camera.CameraState.PLAYING:
                return new SharedLibJG.Helpers.Range(10, 19);
            case Kinect_Wrapper.camera.CameraState.PLAYING_PAUSE:
                return new SharedLibJG.Helpers.Range(10, 19);
            case Kinect_Wrapper.camera.CameraState.PLAYING_STOPPING:
                return new SharedLibJG.Helpers.Range(10, 19);
            case Kinect_Wrapper.camera.CameraState.RECORDING:
                return new SharedLibJG.Helpers.Range(20, 29);
            case Kinect_Wrapper.camera.CameraState.RECORDING_PAUSE:
                return new SharedLibJG.Helpers.Range(20, 29);
            case Kinect_Wrapper.camera.CameraState.RECORDING_STOPPING:
                return new SharedLibJG.Helpers.Range(20, 29);
            case Kinect_Wrapper.camera.CameraState.RECORDING_CANCEL:
                return new SharedLibJG.Helpers.Range(20, 29);
        }
        throw new Exception("bad enum state");
    }
}

