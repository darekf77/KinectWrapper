using Kinect_Wrapper.device;
using Kinect_Wrapper.wrapper;
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
    /// Interaction logic for KinectWrapperListWidget.xaml
    /// </summary>
    public partial class PageKinectWrapperList : Page
    {
        private IKinectWrapper _kinect;
        public PageKinectWrapperList(IKinectWrapper kinect)
        {
            InitializeComponent();
            _kinect = kinect;   
            this.DataContext = new
            {
                kinectWrap = _kinect,
                video = _kinect.Device.Video
            };

        }

        private void ListViewDevices_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {            
            _kinect.SelectedDevice = (IDevice)ListViewDevices.SelectedItem;            
        }

    }
}
