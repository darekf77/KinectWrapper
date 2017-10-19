using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kinect_Wrapper.devicemanager
{
    public partial class DeviceManager
    {
        static readonly object _lockerWorkerState = new object();

        private BackgroundWorker _workerFrames;
        private BackgroundWorker _workerState;

        private void initWorkers()
        {
            _workerFrames = new BackgroundWorker();

            _workerFrames.DoWork += (e, v) =>
            {
                while (true) Camera.update();
            };
            _workerFrames.RunWorkerAsync();


            _workerState = new BackgroundWorker();
            _workerState.DoWork += (e, v) =>
            {
                while (true)
                {
                    foreach (var device in Devices)
                    {
                        Camera.CurrentDevice.update(Camera.State, Camera.CurrentDevice.Equals(device));
                    }
                    Thread.Sleep(1000);
                    lock (_lockerWorkerState)
                        while (Camera.CurrentDevice == null)
                            Monitor.Wait(_lockerWorkerState);// thread is waiting until new data from kinect   
                }
            };
            _workerState.RunWorkerAsync();

        }


    }
}
