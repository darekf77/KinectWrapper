using Kinect_Wrapper.device;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.statistic;
using Kinect_Wrapper.structures;
using Kinect_Wrapper.wrapper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kinect_Wrapper.app
{
    /// <summary>
    /// Interaction logic for KinectWrapperControlWidget.xaml
    /// </summary>
    public partial class PageKinectWrapperControl : Page
    {
        private IKinectWrapper Kinect;

        public PageKinectWrapperControl(IKinectWrapper kinect)
        {
            InitializeComponent();
            Kinect = kinect;
            ComboboxStreams.SelectedIndex = 0;
            this.DataContext = new
            {
                kinect = Kinect,
                camera = Kinect.Camera,
                audio = Kinect.Camera.Audio
            };
        }



        private void play(object sender, RoutedEventArgs e)
        {
            Kinect.Manager.Device = Kinect.Manager.SelectedDevice; // TODO again press play - > Complete !!! ballls containter
            Kinect.Manager.Device.start();
        }

        private void cancel(object sender, RoutedEventArgs e)
        {
            Kinect.Camera.CancelRecord.DoExecute();
        }

        String lastRecordedFilename;
        private void record(object sender, RoutedEventArgs e)
        {
            if (Kinect.Camera.IsRecording)
            {
                Kinect.Camera.Stop.DoExecute();
            }
            else
            {
                var saveFileDialog = new SaveFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };
                if (saveFileDialog.ShowDialog() != true) return;
                Kinect.Camera.record(saveFileDialog.FileName);
            }
        }

        private void next_frame(object sender, RoutedEventArgs e)
        {
            Kinect.Camera.NextFrame.DoExecute();
        }

        private void pause(object sender, RoutedEventArgs e)
        {
            Kinect.Camera.PausePlay.DoExecute();
        }

        private void stop(object sender, RoutedEventArgs e)
        {

        }
    }
}
