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
                audio = Kinect.Camera.Audio,
                manager = Kinect.Manager
            };
        }




    }
}
