using Kinect.Replay.Replay;
using Kinect_Wrapper.user;
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
        private byte[] _colorByte;
        private byte[] _depthByte;
        private int _videoWith = 640;
        private int _videoHeight = 480;
        public int MaxDepth = 0;
        public int MinDepth = 0;
        private short[] _depthShort;
        private DepthImagePixel[] _depthPixels;
        private DepthImagePoint[] _depthPoint;
        private Skeleton[] totalSkeleton = new Skeleton[6];
        private long _pixelColorDataLength;
        private long _pixelDepthDataLength;
        private KinectSensor _sensor;
        private KinectReplay _replay;
        private Bitmap _bitmap;

        public KinectFrame()
        {
            
        }

        public KinectFrame(KinectSensor sensor)
        {
            _sensor = sensor;
            _pixelColorDataLength = sensor.ColorStream.FramePixelDataLength;
            _pixelDepthDataLength = sensor.DepthStream.FramePixelDataLength;
            MaxDepth = _sensor.DepthStream.MaxDepth;
            MinDepth = _sensor.DepthStream.MinDepth;
            init();
        }

        public KinectFrame(KinectReplay replay)
        {
            _replay = replay;
            _pixelColorDataLength = replay.ColorDataPixelLength;
            _pixelDepthDataLength = replay.DepthDataPixelLength;
            MaxDepth = replay.MaxDepth;
            MinDepth = replay.MinDepth;
            init();
        }

        public KinectFrame(Bitmap bitmap)
        {
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

        
        void init()
        {
            _colorByte = new byte[_pixelColorDataLength];
            _depthByte = new byte[_pixelColorDataLength];
            _depthShort = new short[_pixelDepthDataLength];
            _depthPixels = new DepthImagePixel[_pixelDepthDataLength];
            _depthPoint = new DepthImagePoint[_pixelDepthDataLength];
            IsSkeletonDetected = false;
        }

        #region implementation

        public IKinectUser[] Users
        {
            get
            {
                return KinectUser.TempInstance;
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
                _doNotTrasformDepthShortBytes = false;
                _depthShort = value;
            }
        }


        private Boolean _doNotTrasformDepthShortBytes = false;
        public Byte[] DepthColor
        {
            get {
                if (!_doNotTrasformDepthShortBytes)
                {
                    int colorPixelIndex = 0;
                    for (int i = 0; i < this._depthPixels.Length; ++i)
                    {
                        byte intensity = (byte)(_depthShort[i] >= MinDepth
                            && _depthShort[i] <= MaxDepth ? _depthShort[i] / 16 : 0);
                        _depthByte[colorPixelIndex++] = intensity;
                        _depthByte[colorPixelIndex++] = intensity;
                        _depthByte[colorPixelIndex++] = intensity;
                        _depthByte[colorPixelIndex++] = 0;
                    }
                }
                _doNotTrasformDepthShortBytes = false;
                return _depthByte;                 
            }
            set
            {
                _doNotTrasformDepthShortBytes = true;
                _depthByte = value;
            }
        }
        
        public int VideoWidth
        {
            get
            {
                return _videoWith;
            }
        }

        public int VideoHeight
        {
            get
            {
                return _videoHeight;
            }
        }

        #endregion



        public Boolean IsSkeletonDetected { get; private set; }
    }
}
