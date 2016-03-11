using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.statistic;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Controls;

namespace Kinect_Wrapper.wrapper
{
    public partial class KinectWrapper
    {
        private BackgroundWorker _worker;

        private void initWorker()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerAsync();
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (Device != null)
                {
                    Device.update();
                    if (Device.Type == DeviceType.NO_DEVICE)
                    {
                        if (AutoPickUpFirstKinect)
                        {
                            foreach (var dev in Devices)
                            {
                                if (dev.Type == DeviceType.KINECT_1 && dev.State == DeviceState.READY)
                                {
                                    Device = dev;                                    
                                    Device.start();
                                    break;
                                }
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    else if (Device.State == DeviceState.READY || Device.State == DeviceState.NOT_READY)
                    {
                        Device = _defaultDevice;
                        Device.start();
                    }
                    continue;
                }

                lock (_locker)
                    while (Device == null ||
                        Device.Type == DeviceType.RECORD_FILE_KINECT_1)
                        Monitor.Wait(_locker);// thread is waiting until new data from kinect                 
            }
        }

    }
}
