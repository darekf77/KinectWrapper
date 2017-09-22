using Kinect_Wrapper.app;
using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using Kinect_Wrapper.wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Kinect_Wrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKinectWrapper kinect = KinectWrapper.Instance();
        private PageKinectWrapperList _devicesList;
        private PageKinectWrapperControl _kinectControls;
        private Thread _worker;

        public MainWindow()
        {
            InitializeComponent();
            _worker = new Thread(new ThreadStart(initApp));
            _worker.IsBackground = true;
            _worker.SetApartmentState(ApartmentState.STA);
            _worker.Start();
            _devicesList = new PageKinectWrapperList(kinect);
            _kinectControls = new PageKinectWrapperControl(kinect);
            KinectWrapperDevicesListWidget.NavigationService.Navigate(_devicesList);
            KinectWrapperControlsWidget.NavigationService.Navigate(_kinectControls);
        }

        void initApp()
        {
            kinect.Video.FrameReady += kinect_ReadyFrame;
            kinect.DisplayImageReady += kinect_ReadyDisplayImage;
        }


        void kinect_ReadyFrame(object sender, frame.IKinectFrame e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                kinect.commit(e);
            }));
        }

        void kinect_ReadyDisplayImage(object sender, ImageSource e)
        {
            MainImage.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainImage.Source = e;
            }));

        }


    }
}
