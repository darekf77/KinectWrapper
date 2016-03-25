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

        public static StreamBase ById(int id)
        {
            return Instances[id];
        }
        public static Dictionary<int, StreamBase> Instances = new Dictionary<int, StreamBase>();

        protected Bitmap _imageSource;
        private static int cid = 0;
        public StreamBase()
        {
            id = cid;
            ++cid;
            StreamBase.Instances[id] = this;

            Name = "Stream";
            Description = "Stream description";
            _bitmapaColor = new WriteableBitmap(
                640,
                480,
                96.0, 96.0, PixelFormats.Bgr32, null);
            _bitmapaDepth = new WriteableBitmap(
                640,
                480,
                96.0, 96.0, PixelFormats.Gray8, null);
            
            _imageSource = new Bitmap(640,480,System.Drawing.Imaging.PixelFormat.Format32bppRgb);            
        }

        public string Name { get; set; }

        public string Description { get; set; }

        
        //public override bool Equals (object pobj)
        //{
        //    var obj = (StreamBase)pobj;
        //    if (obj == null || GetType() != obj.GetType()) 
        //    {
        //        return false;
        //    }        
        //    // TODO: write your implementation of Equals() here
        //    return obj.id == obj.id;
        //}        

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

        internal object update(object p)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Stream base with id:" + id;
        }

        public int id { get; private set; }
    }
}
