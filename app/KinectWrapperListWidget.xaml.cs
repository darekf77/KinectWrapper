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
        private IKinectWrapper kinect;

        public PageKinectWrapperList(IKinectWrapper kinect)
        {
            InitializeComponent();
            this.kinect = kinect;
            this.DataContext = new
            {
                kinectWrap = kinect,
                camera = kinect.Camera,
                manager = kinect.Manager,
                audio = kinect.Camera.Audio,
                gestures = kinect.Gestures,
                recognizer = kinect.Camera.Audio.Recognizer
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
        
        private void gestureEntererd(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var t = int.Parse(InputResize.Text);
                if (t > 0) t /= 100;
                kinect.Gestures.triggerResizeGesture(t);
            }
        }
    }
}
