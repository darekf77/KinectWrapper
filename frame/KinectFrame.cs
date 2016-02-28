using Kinect.Replay.Replay;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.frame
{
    public partial class KinectFrame : EventArgs, IKinectFrame
    {
        private byte[] _colorByte = new byte[_pixelColorDataLength];
        private byte[] _depthByte = new byte[_pixelColorDataLength];
        public int MaxDepth = 0;
        public int MinDepth = 0;
        private short[] _depthShort = new short[_pixelDepthDataLength];
        private DepthImagePixel[] _depthPixels = new DepthImagePixel[_pixelDepthDataLength];
        private DepthImagePoint[] _depthPoint = new DepthImagePoint[_pixelDepthDataLength];
        private Skeleton[] totalSkeleton = new Skeleton[6];
        private const long _pixelColorDataLength = 1228800;
        private const long _pixelDepthDataLength = 307200;
        private KinectSensor _sensor;
        private KinectReplay _replay;
        private Bitmap _bitmap;

        public Boolean IsSkeletonDetected { get;  set;}
        public Dictionary<SkeletonDataType, Point> UserSkeleton  {get; set;}

        
        void init() {
            IsSkeletonDetected = false;
            UserSkeleton = new Dictionary<SkeletonDataType,Point>();
            UserSkeleton[SkeletonDataType.LEFT_HAND] = new Point();
            UserSkeleton[SkeletonDataType.RIGHT_HAND] = new Point();
            UserSkeleton[SkeletonDataType.SPINE] = new Point();
        }

        public KinectFrame()
        {
            init();
            
        }

        public KinectFrame(IKinectFrame frame)
        {
            UserSkeleton = new Dictionary<SkeletonDataType, Point>(frame.UserSkeleton);
            IsSkeletonDetected = frame.IsSkeletonDetected;
            frame.Color.CopyTo(Color, 0);
            frame.Depth.CopyTo(Depth, 0);
            frame.DepthColor.CopyTo(DepthColor, 0);
        }

        public KinectFrame(KinectSensor sensor)
        {
            init();
            _sensor = sensor;
            //_pixelColorDataLength = sensor.ColorStream.FramePixelDataLength;
            //_pixelDepthDataLength = sensor.DepthStream.FramePixelDataLength;
            MaxDepth = _sensor.DepthStream.MaxDepth;
            MinDepth = _sensor.DepthStream.MinDepth;
        }

        public KinectFrame(KinectReplay replay)
        {
            init();
            _replay = replay;
            //_pixelColorDataLength = replay.ColorDataPixelLength;
            //_pixelDepthDataLength = replay.DepthDataPixelLength;
            MaxDepth = replay.MaxDepth;
            MinDepth = replay.MinDepth;
        }

        public KinectFrame(Bitmap bitmap)
        {
            init();
            _bitmap = bitmap;
            _colorByte = new byte[4 * (bitmap.Width * bitmap.Height)];
            prepareColorPixelsFrom(bitmap);            
        }

        private void prepareColorPixelsFrom(Bitmap bitmap)
        {
            int i = 0;

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    Color pix = bitmap.GetPixel(x, y);

                    _colorByte[i++] = pix.R;
                    _colorByte[i++] = pix.G;
                    _colorByte[i++] = pix.B;
                    _colorByte[i++] = pix.A;
                }
            }
        }

            
        

        
        public byte[] Color
        {
            get
            {
                return _colorByte;
            }
            set
            {
                _colorByte = value;
            }
        }



        public short[] Depth
        {
            get
            {
                return _depthShort;
            }
            set
            {
                _depthShort = value;
            }
        }


        public Byte[] DepthColor
        {
            get {
                return _depthByte;                 
            }
            set
            {
                _depthByte = value;
            }
        }
        

        
    }
}
