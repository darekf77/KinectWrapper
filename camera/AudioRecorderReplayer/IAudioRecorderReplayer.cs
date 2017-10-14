﻿using Kinect_Wrapper.device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera
{
    public interface IAudioRecorderReplayer
    {
        void init(IDevice device);
        void record(string toFile);
        void replay(string replayFile);
        void stop();
        void pause();
        bool IsRecordingPossible { get; }
        bool IsPreparingAudio { get; }
        CameraState State { get; set; }
        ObservableCollection<IAudioSourceDevice> Devices { get; }
        IAudioSourceDevice SelectedDevice { get; set; }

    }
}
