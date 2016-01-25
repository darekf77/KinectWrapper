using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
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

namespace Kinect_Wrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        IKinectWrapper kinect = KinectWrapper.Instance;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            kinect.FrameReady += kinect_ReadyFrame;
            kinect.DisplayImageReady += kinect_ReadyDisplayImage;
        }

        void kinect_ReadyFrame(object sender, frame.IKinectFrame e)
        {
            kinect.commit(e);
        }

        void kinect_ReadyDisplayImage(object sender, ImageSource e)
        {
            Console.WriteLine("main image updated");
            MainImage.Dispatcher.BeginInvoke(new Action(() => {
                MainImage.Source = e;
            }));
            
        }


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("How man devices" + kinect.Devices.Count);
            ListViewDevices.ItemsSource = kinect.Devices;
            ComboBoxStreams.ItemsSource = kinect.Streams;
        }

        




        



    }
}
