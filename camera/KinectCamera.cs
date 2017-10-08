using Kinect_Wrapper.camera.Recorder;
using Microsoft.Kinect;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera
{
    public class KinectCamera : IKinectCamera
    {
        KinectSensor sensor;
        BinaryWriter writer;
        WaveFileWriter waveWrite;
        Stream stream;
        public KinectCamera(KinectSensor sensor)
        {
            this.sensor = sensor;
        }


        #region state
        private CameraState _state = CameraState.UNACTIVE;
        public CameraState State
        {
            get { return _state; }
            private set { _state = value; }
        }
        #endregion

        public void replay(string replayFile)
        {
            State = CameraState.PLAYING;
        }


        #region record
        public void record(string toFile)
        {
            stream = File.Create(toFile);
            writer = new BinaryWriter(stream);

            // writer.Write((int)Options); //write version

            colorRecorder = new ColorRecorder(writer, sensor);
            depthRecorder = new DepthRecorder(writer, sensor);
            skeletonRecorder = new SkeletonRecorder(writer, sensor);
            audioRecorder = new AudioRecorder(waveWrite);

            previousFlushDate = DateTime.Now;
            State = CameraState.RECORDING;
        }
        #endregion

        #region update record data
        private SkeletonRecorder skeletonRecorder;
        private ColorRecorder colorRecorder;
        private DepthRecorder depthRecorder;
        private AudioRecorder audioRecorder;
        public void update(ColorImageFrame color, DepthImageFrame depth, SkeletonFrame skeleton, KinectSensor sensor)
        {
            if (skeletonRecorder == null || colorRecorder == null || depthRecorder == null)
                return;
            if (writer == null)
            {
                Console.WriteLine("Write should ont be NULL !!!");
                return;
            }

            skeletonRecorder.update(skeleton);
            Flush();
            colorRecorder.update(color);
            Flush();
            depthRecorder.update(depth);
            Flush();
        }
        #endregion

        #region flush date
        private DateTime previousFlushDate;
        private void Flush()
        {
            var now = DateTime.Now;

            if (now.Subtract(previousFlushDate).TotalSeconds > 60)
            {
                previousFlushDate = now;
                writer.Flush();
            }
        }
        #endregion

        #region stop
        void stopRecord()
        {
            writer.Close();
            stream.Close();
            writer.Dispose();
            stream.Dispose();
            State = CameraState.UNACTIVE;
        }

        public void stop()
        {
            if (State == CameraState.RECORDING) stopRecord();
        }

        #endregion

    }
}
