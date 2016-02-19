using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.helpers
{
    static class Helpers
    {
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        //<summary>
        //Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        //</summary>
        //<param name="image">The Emgu CV Image</param>
        //<returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSourceSlow(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            using (System.Drawing.Bitmap source = bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        public static BitmapSource ToBitmapSourceFast(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height, 96, 96, PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

    }
}
