using Kinect_Wrapper.frame;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.device.stream
{
    public abstract partial class StreamBase : IStream
    {
        protected WriteableBitmap _bitmapaColor;
        protected WriteableBitmap _bitmapaDepth;
        protected Bitmap _imageSource;
        public StreamBase()
        {
            Name = "Stream";
            Description = "Stream description";
            _bitmapaColor = new WriteableBitmap(
                640,
                480,
                96.0, 96.0, PixelFormats.Bgr32, null);
            _bitmapaDepth = new WriteableBitmap(
                640,
                480,
                96.0, 96.0, PixelFormats.Bgr32, null);
            IsCustom = true;

            // TODO what bitmap
            _imageSource = new Bitmap(640,480,System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        }

        public string Name { get; set; }

        public string Description { get; set; }


        public virtual ImageSource update(ImageSource image)
        {
            return image;
        }


        public virtual ImageSource update(IKinectFrame frame)
        {   
            _bitmapaColor.WritePixels(
               new System.Windows.Int32Rect(0, 0,
                   _bitmapaColor.PixelWidth,
                   _bitmapaColor.PixelHeight),
               frame.Color,
               _bitmapaColor.PixelWidth * sizeof(int),
               0);
            return _bitmapaColor;
        }
        

        public bool IsCustom { get; protected set; }
    }
}
