using Kinect_Wrapper.device;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.statistic;
using Kinect_Wrapper.structures;
using Kinect_Wrapper.wrapper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        private IKinectWrapper _kinect;
        
        public PageKinectWrapperControl(IKinectWrapper kinect)
        {
            InitializeComponent();            
            _kinect = kinect;
            ComboboxStreams.SelectedIndex = 0;
            this.DataContext = new
            {
                kinectWrap = _kinect,
                video = _kinect.Device.Video
            };
        }
     

        private void play(object sender, RoutedEventArgs e)
        {
            _kinect.Device = _kinect.SelectedDevice; // TODO again press play - > Complete !!! ballls containter
            _kinect.Device.start();
        }

        private void stop(object sender, RoutedEventArgs e)
        {
            _kinect.Device.stop();
        }

        private String _labelDefaultPause;
        private void pause(object sender, RoutedEventArgs e)
        {
            if (_kinect.Device.Video.IsPaused)
            {
                ButtonPause.Content = _labelDefaultPause;
            }
            else
            {
                _labelDefaultPause = ButtonPause.Content.ToString();
                ButtonPause.Content = "START";
            }
            _kinect.Device.Video.pausePlay();
        }

        private String _labelDefaultRecord;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(_kinect.Device.Video.IsRecording) {
                _kinect.Device.Video.stopRecord();
                ButtonRecordAndSave.Content = _labelDefaultRecord;
            }
            else {
                var saveFileDialog = new SaveFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };
                if (saveFileDialog.ShowDialog() != true) return;
                _kinect.Device.Video.startRecordAndSaveTo(saveFileDialog.FileName);
                _labelDefaultRecord = ButtonRecordAndSave.Content.ToString();
                ButtonRecordAndSave.Content = "STOP REC.";
            }
        }
        
        private void ComboboxStreams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _kinect.Stream = (StreamBase)ComboboxStreams.SelectedItem;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _kinect.Device.Video.nextFrame();
        }



    }
}
