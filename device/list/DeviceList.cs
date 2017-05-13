using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.video;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.list
{
    public class DevicesList
    {
        public static string AppPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private ObservableCollection<DeviceBase> Devices;
        private AudioBase audio;
        private VideoBase video;
        private bool isLoadingFiles = false;

        public DevicesList(ObservableCollection<DeviceBase> Devices, AudioBase audio, VideoBase video)
        {
            this.Devices = Devices;
            this.video = video;
            this.audio = audio;
            Devices.CollectionChanged += (e, v) =>
            {
                if(!isLoadingFiles) save();
            };

            this.load();
        }



        #region load
        private void load()
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
                    if(File.Exists(replayPath)) {
                        FileInfo f = new FileInfo(replayPath);
                        if (f.Length == 0) continue;
                        Devices.Add(new Device(audio, video, replayPath));
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

    }
    #endregion

    //}

}
