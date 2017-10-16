using Kinect_Wrapper.camera;
using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.devicemanager
{
    public interface IDeviceManager : INotifyPropertyChanged
    {
        IKinectCamera Camera { get; }
        ObservableCollection<IDevice> Devices { get; }

        IDevice Device { get; set; }

        IDevice SelectedDevice { get; set; }

        Boolean IsStopped { get; }

        DeviceType AutopickupDeviceType { get; }

        bool AutopickupDevice { get; set; }

        event EventHandler DeviceChanged;

        event EventHandler OnAutopickupDeviceChanged;
    }
}
