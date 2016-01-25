using Kinect_Wrapper.frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.device.stream
{
    public interface IStream
    {
        /// <summary>
        /// Name of stream
        /// </summary>
        String Name {get; set;}

        /// <summary>
        /// Descriptio of stream
        /// </summary>
        String Description { get; set; }

        Boolean IsCustom { get; }

        /// <summary>
        /// Updat stream
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        ImageSource update(IKinectFrame frame);
        ImageSource update(ImageSource frame);
    }
}
