using Kinect.Replay.Replay;
using Kinect.Replay.Replay.Color;
using Kinect.Replay.Replay.Depth;
using Kinect.Replay.Replay.Skeletons;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Kinect_Wrapper.frame
{
    public partial class KinectFrame 
    {
        private static byte getDepth(int Depth)
        {
            var d = Depth * 0.064 - 1;
            return (byte)(d >= 0 && d <= 255 ? d : 0);
        }

        public void synchronize(
            ReplayDepthImageFrame depthFrame,
            ReplayColorImageFrame colorFrame,
            ReplaySkeletonFrame skletonFrame,
            Boolean isPauseMode
            )
        {
            IsPauseMode = isPauseMode;
            colorFrame.CopyPixelDataTo(_colorByte);
            depthFrame.CopyPixelDataTo(_depthShort);
            for (int i = 0; i < _pixelDepthDataLength; i++)
            {
                _depthByte[i] = getDepth(_depthShort[i]);
            }

            _isCreation = true;
            IsSkeletonDetected = skletonFrame.IsSkeletonDetected;
            
            if (skletonFrame.IsSkeletonDetected)
            {
                UserSkeleton[SkeletonDataType.RIGHT_HAND] = new Point(
                    skletonFrame.RightHandPositionX,
                    skletonFrame.RightHandPositionY
                    );

                UserSkeleton[SkeletonDataType.LEFT_HAND] = new Point(
                    skletonFrame.LeftHandPositionX,
                    skletonFrame.LeftHandPositionY
                    );

                UserSkeleton[SkeletonDataType.SPINE] = new Point(
                    skletonFrame.SpinePositionX,
                    skletonFrame.SpinePositionY
                    );
            }
            _isCreation = false;

        }

        
        public void synchronize(
            DepthImageFrame depthFrame,
            ColorImageFrame colorFrame,
            SkeletonFrame skletonFrame,
            Boolean isPauseMode
            )
        {
            IsPauseMode = isPauseMode;
            colorFrame.CopyPixelDataTo(_colorByte);

            //Console.WriteLine("max depth: "+depthFrame.MaxDepth);
            depthFrame.CopyDepthImagePixelDataTo(_depthPixels);

            _sensor.CoordinateMapper.MapColorFrameToDepthFrame(
                ColorImageFormat.RgbResolution640x480Fps30,
                DepthImageFormat.Resolution640x480Fps30,
                _depthPixels,
                _depthPoint
                );

            for (int i = 0; i < _pixelDepthDataLength; i++)
            {
                _depthShort[i] = (short)_depthPoint[i].Depth;
                _depthShort[i] = getDepth(_depthPoint[i].Depth);
            }

            skletonFrame.CopySkeletonDataTo(totalSkeleton);            
            Skeleton firstSkeleton = (from trackskeleton in totalSkeleton
                                      where trackskeleton.TrackingState == SkeletonTrackingState.
                                      Tracked
                                      select trackskeleton).FirstOrDefault();

            _isCreation = true;
            if (firstSkeleton != null)
            {
                if (firstSkeleton.Joints[JointType.Spine].TrackingState == JointTrackingState.Tracked)
                {
                    IsSkeletonDetected = true;
                    UserSkeleton[SkeletonDataType.RIGHT_HAND] =
                        ScalePosition(firstSkeleton.Joints[JointType.HandRight].Position);
                    UserSkeleton[SkeletonDataType.LEFT_HAND] =
                        ScalePosition(firstSkeleton.Joints[JointType.HandLeft].Position);
                    UserSkeleton[SkeletonDataType.SPINE] =
                        ScalePosition(firstSkeleton.Joints[JointType.Spine].Position);
                    return;
                }
            }
            IsSkeletonDetected = false;
            _isCreation = false;
        }
        
        public void synchronize(String message, Boolean isVisible, Boolean isPauseMode)
        {
            //_isCreation = true;
            //IsPauseMode = isPauseMode;
            if (isVisible)
            {
                byte fontsize = 20;
                RectangleF rectBitmap = new RectangleF(0,0,_bitmap.Width,_bitmap.Height);
                RectangleF rectf = new RectangleF(200, _bitmap.Height /2 - fontsize , _bitmap.Width, 3*fontsize);
                Bitmap bmp = _bitmap.Clone(rectBitmap, _bitmap.PixelFormat);
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString(message, new Font("Segoe UI Light", Helpers.getResponsiveFontSize(fontsize)), Brushes.White, rectf);
                g.Flush();
                prepareColorPixelsFrom(bmp);
            }
            else
            {
                prepareColorPixelsFrom(_bitmap);
            }
            //_isCreation = false;            
        }

        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            ColorImagePoint depthPoint = _sensor.CoordinateMapper.
            MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RgbResolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
        

    }
}
