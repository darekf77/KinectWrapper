using Kinect_Wrapper.camera.Recorder;
using Microsoft.Kinect;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Kinect_Wrapper.device;
using Kinect_Wrapper.camera.Replayer;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Apex.MVVM;
using Kinect_Wrapper.frame;

namespace Kinect_Wrapper.camera
{
    public partial class KinectCamera
    {
        void initFrame(IDevice device)
        {



            //if (device == null) return;
            //{
            //    if (_device != null)
            //    {

            //    }
            //    _device = null;
            //    StreamingFilePath = "";
            //    IsRecordingPossible = false;
            //    return;
            //}

            //IsStreaming = false;
            //IsRecording = false;
            //IsPaused = false;

            //_device = value;
            //StreamingFilePath = _device.Path;
            //if (_device.Type == DeviceType.KINECT_1)
            //{
            //    _frame = new KinectFrame(_device.sensor);
            //    IsRecordingPossible = true;
            //}
            //else
            //if (_device.Type == DeviceType.RECORD_FILE_KINECT_1)
            //{
            //    _frame = new KinectFrame(_device.replay);
            //    _device.replay.AllFramesReady += ReplayAllFramesReady;
            //    _device.replay.ReplayFinished += _replay_ReplayFinished;
            //    _device.replay.Start();
            //}
            //else
            //if (_device.Type == DeviceType.NO_DEVICE)
            //{
            //    String exePath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            //    string dir = Path.GetDirectoryName(exePath);
            //    var url = dir + "\\..\\..\\assets\\img\\nosignalw640.png";
            //    if (File.Exists(url))
            //    {
            //        var bitmap = new Bitmap(url);
            //        _frame = new KinectFrame(bitmap);
            //    }
            //    else
            //    {
            //        Console.WriteLine("bad url: " + url);
            //    }

            //}
        }

    }
}
