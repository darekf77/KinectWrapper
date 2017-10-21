using Apex.MVVM;
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
        IDevice Device { get; }

        ObservableCollection<IDevice> Devices { get; }

        IDevice SelectedDevice { get; set; }

        DeviceType AutopickupDeviceType { get; }

        Command AddFromHardDrive { get; }

        Command RemoveFromDevices { get; }

        bool IsSelectedDevice { get; }

        bool IsSelectedDeviceReadyToPlay { get; }

        event EventHandler DeviceChanged;

        event EventHandler OnAutopickupDeviceChanged;
    }
}
