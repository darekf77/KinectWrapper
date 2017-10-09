using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.device.video
{
    public partial class Video
    {
        private IDevice _device;
        private IKinectFrame _frame;


        public IDevice CurrentDevice
        {
            set
            {
                if (value == null)
                {
                    if (_device != null)
                    {
                        if (_device.Type == DeviceType.KINECT_1)
                        {
                            _device.sensor.Stop();
                        }
                        else
                        if (_device.Type == DeviceType.RECORD_FILE_KINECT_1)
                        {
                            _device.replay.AllFramesReady -= ReplayAllFramesReady;
                            _device.replay.ReplayFinished -= _replay_ReplayFinished;
                            _device.replay.Stop();
                        }
                    }
                    _device = null;
                    StreamingFilePath = "";
                    IsRecordingPossible = false;
                    return;
                }

                IsStreaming = false;
                IsRecording = false;
                IsPaused = false;

                _device = value;
                StreamingFilePath = _device.Path;
                if (_device.Type == DeviceType.KINECT_1)
                {
                    _frame = new KinectFrame(_device.sensor);
                    IsRecordingPossible = true;
                }
                else
                if (_device.Type == DeviceType.RECORD_FILE_KINECT_1)
                {
                    _frame = new KinectFrame(_device.replay);
                    _device.replay.AllFramesReady += ReplayAllFramesReady;
                    _device.replay.ReplayFinished += _replay_ReplayFinished;
                    _device.replay.Start();
                }
                else
                if (_device.Type == DeviceType.NO_DEVICE)
                {
                    String exePath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                    string dir = Path.GetDirectoryName(exePath);
                    var url = dir + "\\..\\..\\assets\\img\\nosignalw640.png";
                    if (File.Exists(url))
                    {
                        var bitmap = new Bitmap(url);
                        _frame = new KinectFrame(bitmap);
                    }
                    else
                    {
                        Console.WriteLine("bad url: " + url);
                    }

                }

            }
            get { return _device; }
        }

    }
}
