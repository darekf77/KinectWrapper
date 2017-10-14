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
    public static SharedLibJG.Helpers.Range EnumGroupRange(this Kinect_Wrapper.camera.CameraState s1)
    {
        var playingRange = new SharedLibJG.Helpers.Range();
        switch (s1)
        {
            case Kinect_Wrapper.camera.CameraState.UNACTIVE:
                break;
            case Kinect_Wrapper.camera.CameraState.PLAYING:
                break;
            case Kinect_Wrapper.camera.CameraState.PLAYING_PAUSE:
                break;
            case Kinect_Wrapper.camera.CameraState.PLAYING_STOPPING:
                break;
            case Kinect_Wrapper.camera.CameraState.RECORDING:
                break;
            case Kinect_Wrapper.camera.CameraState.RECORDING_PAUSE:
                break;
            case Kinect_Wrapper.camera.CameraState.RECORDING_STOPPING:
                break;
            case Kinect_Wrapper.camera.CameraState.RECORDING_CANCEL:
                break;
            default:
                break;
        }
        return new SharedLibJG.Helpers.Range();
    }
}

