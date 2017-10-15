using Kinect_Wrapper.device;
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
                camera = _kinect.Camera,
                audio = _kinect.Camera.Audio,
                gestures = kinect.Gestures
            };
            kinect.Gestures.onResizeGesture += (e, v) =>
             {
                 App.Current.Dispatcher.BeginInvoke(new Action(() =>
                 {
                     if (float.Parse(InputResize.Text) != v)
                     {
                         InputResize.Text = v.ToString();
                     }
                 }));

             };
        }

        private void ListViewDevices_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _kinect.SelectedDevice = (Device)ListViewDevices.SelectedItem;
        }

        private void add_device_from_hard_drive(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };
            if (openFileDialog.ShowDialog() != true) return;
            _kinect.Devices.Add(new Device(openFileDialog.FileName));
        }

        private void remove_device_from_list(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult =
               MessageBox.Show("Are you sure?", "Delete device from list",
               MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                _kinect.Devices.Remove(_kinect.SelectedDevice);
        }

        private void gestureEntererd(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var t = int.Parse(InputResize.Text);
                if (t > 0) t /= 100;
                _kinect.Gestures.triggerResizeGesture(t);
            }
        }
    }
}
