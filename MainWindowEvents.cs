using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kinect_Wrapper
{
    public partial class MainWindow 
    {

        private void play(object sender, RoutedEventArgs e)
        {
            kinect.Device = (IDevice)ListViewDevices.SelectedItem;
            kinect.Device.start();
        }

        private void stop(object sender, RoutedEventArgs e)
        {
            if (kinect.Device.State == DeviceState.IS_WORKING_RECORDING)
            {
                kinect.Device.Video.stopRecord();
            }
            kinect.Device.stop();
        }

        private void pause(object sender, RoutedEventArgs e)
        {
            kinect.Device.Video.PausePlay();
        }
    }
}
