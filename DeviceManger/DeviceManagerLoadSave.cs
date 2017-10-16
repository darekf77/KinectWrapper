using Kinect_Wrapper.device;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kinect_Wrapper.devicemanager
{
    public partial class DeviceManager
    {

        public static string AppPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private bool isLoadingFiles = false;

        #region check potential files
        private void loadReplayFilesInWorkspace()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string[] potentialReplayfiles = Directory.GetFiles(currentDirectory, "*.replay", SearchOption.AllDirectories);
            foreach (var file in potentialReplayfiles)
            {
                FileInfo f = new FileInfo(file);
                if (f.Length == 0) continue;
                Devices.Add(new Device(file));
            }
        }
        #endregion

        #region check potential sensors
        private void loadSensorsFromSystem()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor == null) continue;
                Devices.Add(new Device(potentialSensor));
            }
        }

        #endregion

        #region load
        private void loadReplayFilesFromConfigFile()
        {
            isLoadingFiles = true;
            var fileScenariosPath = AppPath + "\\devices.json";
            if (!File.Exists(fileScenariosPath))
            {
                isLoadingFiles = false;
                return;
            };
            using (var s = new StreamReader(fileScenariosPath))
            {
                var jsonList = s.ReadLine();
                if (jsonList == null)
                {
                    isLoadingFiles = false;
                    return;
                }
                List<string> replaysList = JsonConvert.DeserializeObject<List<string>>(jsonList);
                foreach (var replayPath in replaysList)
                {
                    if (File.Exists(replayPath))
                    {
                        FileInfo f = new FileInfo(replayPath);
                        if (f.Length == 0) continue;
                        Devices.Add(new Device(replayPath));
                    }

                }
                s.Close();
            }
            isLoadingFiles = false;
        }
        #endregion

        #region save
        private void save()
        {
            var filePath = AppPath + "\\devices.json";
            using (var w = new StreamWriter(filePath))
            {
                var devicesPaths = new List<string>();
                foreach (var device in Devices)
                {
                    if (device.Type == structures.DeviceType.RECORD_FILE_KINECT_1)
                    {
                        devicesPaths.Add(device.Path);
                    }
                }
                var devices = JsonConvert.SerializeObject(devicesPaths);

                w.WriteLine(devices);
                w.Close();
            }

        }
        #endregion

    }
}
