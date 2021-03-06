﻿using Kinect_Wrapper.camera;
using Kinect_Wrapper.camera.Recorder;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.models;
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
        private byte[] _depthByte = new byte[_pixelDepthDataLength];
        private byte[] _playerIsolated = new byte[_pixelColorDataLength];
        private short[] _depthShort = new short[_pixelDepthDataLength];
        private DepthImagePixel[] _depthPixels = new DepthImagePixel[_pixelDepthDataLength];
        private DepthImagePoint[] _depthPoint = new DepthImagePoint[_pixelDepthDataLength];
        private Skeleton[] totalSkeleton = new Skeleton[6];
        private const long _pixelColorDataLength = 1228800;
        private const long _pixelDepthDataLength = 307200;
        private IKinectCamera _camera;
        private Bitmap _bitmap;




        void init()
        {
            IsSkeletonDetected = false;
            UserSkeleton = new Dictionary<SkeletonDataType, Point>();
            foreach (var joint in SkeletonRecorder.neeededJoints)
            {
                UserSkeleton[(SkeletonDataType)joint] = new Point();
            }
        }

        public KinectFrame()
        {
            init();
        }

        public KinectFrame(DimensionalFrame frame)
        {
            UserSkeleton = new Dictionary<SkeletonDataType, Point>(frame.UserSkeleton);
            IsSkeletonDetected = frame.IsSkeletonDetected;
            frame.Color.CopyTo(Color, 0);
            frame.Depth.CopyTo(Depth, 0);
            if (frame.DepthColor != null) frame.DepthColor.CopyTo(DepthColor, 0);
        }

        public KinectFrame(IKinectFrame frame)
        {
            UserSkeleton = new Dictionary<SkeletonDataType, Point>(frame.UserSkeleton);
            IsSkeletonDetected = frame.IsSkeletonDetected;
            frame.Color.CopyTo(Color, 0);
            frame.Depth.CopyTo(Depth, 0);
            if (frame.DepthColor != null) frame.DepthColor.CopyTo(DepthColor, 0);
        }


        public KinectFrame(IKinectCamera camera)
        {
            init();
            _camera = camera;
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


        public byte[] DepthColor
        {
            get
            {
                return _depthByte;
            }
            set
            {
                _depthByte = value;
            }
        }




        bool _IsIgnoringDepthRange = false;
        public bool IsIgnoringDepthRange
        {
            get
            {
                return _IsIgnoringDepthRange;
            }

            set
            {
                _IsIgnoringDepthRange = value;
            }
        }
    }
}
