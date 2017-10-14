using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using System.ComponentModel;
using System.Threading;

namespace Kinect_Wrapper.wrapper
{
    public partial class KinectWrapper
    {
        static readonly object _lockerWorkerFrames = new object();
        static readonly object _lockerWorkerState = new object();

        private BackgroundWorker _workerFrames;
        private BackgroundWorker _workerState;

        private void initWorkers()
        {
            _workerFrames = new BackgroundWorker();

            _workerFrames.DoWork += (e, v) =>
            {
                while (true)
                {
                    if (Device != null)
                    {
                        Device.update(DeviceUpdateType.FRAMES);
                        if (Device.Type == DeviceType.NO_DEVICE)
                        {
                            Thread.Sleep(1000);
                        }
                        else if (Device.State == DeviceState.NOT_READY) // if current device not ready set default
                        {
                            Device = _defaultDevice;
                            Device.start();
                        }
                        continue;
                    }

                    lock (_lockerWorkerFrames)
                        while (Device == null)
                            Monitor.Wait(_lockerWorkerFrames);// thread is waiting until new data from kinect                 
                }
            };
            _workerFrames.RunWorkerAsync();


            _workerState = new BackgroundWorker();
            _workerState.DoWork += (e, v) =>
            {
                while (true)
                {
                    if (Device != null)
                    {
                        Device.update(DeviceUpdateType.STATE);
                    }
                    Thread.Sleep(1000);
                    lock (_lockerWorkerState)
                        while (Device == null)
                            Monitor.Wait(_lockerWorkerState);// thread is waiting until new data from kinect   
                }
            };
            _workerState.RunWorkerAsync();

        }


    }
}
