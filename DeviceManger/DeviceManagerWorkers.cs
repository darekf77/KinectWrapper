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
        static readonly object _lockerWorkerUpdate = new object();

        private BackgroundWorker workerState;
        private BackgroundWorker workerUpdate;

        private void initWorkers()
        {
            workerUpdate = new BackgroundWorker();
            workerUpdate.DoWork += (e, v) =>
            {
                while (true)
                {
                    Camera.update();
                    lock (_lockerWorkerUpdate)
                        while (Camera.CurrentDevice == null)
                            Monitor.Wait(_lockerWorkerUpdate);// thread is waiting until new data from kinect 
                }

            };
            workerUpdate.RunWorkerAsync();

            workerState = new BackgroundWorker();
            workerState.DoWork += (e, v) =>
            {
                while (true)
                {
                    foreach (var dev in Devices)
                    {
                        dev.update(Camera.State, Camera.CurrentDevice != null && Camera.CurrentDevice.Equals(dev));
                    }
                    Thread.Sleep(1000);
                    lock (_lockerWorkerState)
                        while (Camera.CurrentDevice == null)
                            Monitor.Wait(_lockerWorkerState);// thread is waiting until new data from kinect   
                }
            };
            workerState.RunWorkerAsync();
        }


    }
}
