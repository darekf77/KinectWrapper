using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using System.ComponentModel;
using System.Threading;

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
                    while (Device == null)
                        Monitor.Wait(_locker);// thread is waiting until new data from kinect                 
            }
        }

    }
}
