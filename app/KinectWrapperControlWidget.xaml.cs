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
            kinect.StreamChanged += kinect_StreamChanged;
            this.DataContext = new
            {
                kinect = Kinect,
                video = Kinect.Video,
                audio = Kinect.Audio
            };
        }

       
        

        void kinect_StreamChanged(object sender, StreamBase e)
        {
            Console.WriteLine("Kinect stream changed");
            //App.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    ComboboxStreams.SelectedItem = e; 
            //}));
            
        }
     

        private void play(object sender, RoutedEventArgs e)
        {
            Kinect.Device = Kinect.SelectedDevice; // TODO again press play - > Complete !!! ballls containter
            Kinect.Device.start();
        }

        private void stop(object sender, RoutedEventArgs e)
        {
            if(Kinect.Video.IsRecording)
            {
                Kinect.Video.stopRecord(true);
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                File.Delete(Kinect.Video.RecordingFilePath);
            }
            else  Kinect.Device.stop();
        }
        
        private void record(object sender, RoutedEventArgs e)
        {
            if(Kinect.Video.IsRecording) {
                Kinect.Video.stopRecord();
            }
            else {
                var saveFileDialog = new SaveFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };
                if (saveFileDialog.ShowDialog() != true) return;
                Kinect.Video.startRecordAndSaveTo(saveFileDialog.FileName);
            }
        }
        
        private void next_frame(object sender, RoutedEventArgs e)
        {
            Kinect.Video.nextFrame();
        }

        private void pause(object sender, RoutedEventArgs e)
        {
            Kinect.Video.pausePlay();
        }
    }
}
