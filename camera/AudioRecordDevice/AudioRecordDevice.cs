using Kinect_Wrapper.device;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera
{

    public class AudioRecordDevice : IAudioRecordDevice
    {
        public static ObservableCollection<IAudioRecordDevice> Devices = new ObservableCollection<IAudioRecordDevice>();

        public static void refresList(IDevice device = null)
        {
            Devices.Clear();
            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                var product = WaveIn.GetCapabilities(n);
                var dev = new AudioRecordDevice(product);
                Devices.Add(dev);
            }
        }


        public AudioRecordDevice(WaveInCapabilities c)
        {
            this.Id = Devices.Count + 1;
            this.Name = c.ProductName;
        }


        public int Id { get; set; }
        public int guid { get; set; }

        public string Name { get; set; }



    }
}
