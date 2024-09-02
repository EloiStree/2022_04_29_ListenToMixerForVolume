using Newtonsoft.Json;
using System.IO;

namespace ListenToMixerForVolume
{
    public class ConfigurationProjectFromFileAtRoot {
        public bool m_useUdp = true;
        public int m_targetPortId=2507;
        public string m_targetIp = "127.0.0.1";
        public bool m_skipAreYouReady=true;
        public int m_timeInMilliseconds=5;
        public AudioMeterListener[] m_audioListeners = new AudioMeterListener[] {
                new AudioMeterListener("wow",0, 0.05f,0.15f,"wowAudio0Low"),
                new AudioMeterListener("wow",0, 0.15f,0.7f,"wowAudio0medium"),
                new AudioMeterListener("wow",0, 0.7f,1f,"wowAudio0Hight"),
                new AudioMeterListener("wow",0, 0.05f,1f,"wowAudio0"),
                new AudioMeterListener("wow",1, 0.05f,1f,"wowAudio1"),
                new AudioMeterListener("wow",2, 0.05f,1f,"wowAudio2"),
                new AudioMeterListener("wow",3, 0.05f,1f,"wowAudio3"),
                new AudioMeterListener("wow",4, 0.05f,1f,"wowAudio4"),
                new AudioMeterListener("wow",5, 0.05f,1f,"wowAudio5"),
                new AudioMeterListener("wow",6, 0.05f,1f,"wowAudio6"),
                new AudioMeterListener("wow",7, 0.05f,1f,"wowAudio7"),
                new AudioMeterListener("wow",8, 0.05f,1f,"wowAudio8"),
                new AudioMeterListener("wow",9, 0.05f,1f,"wowAudio9"),
                new AudioMeterListener("firefox",0, 0.05f,0.15f,"firefoxAudio0Low"),
                new AudioMeterListener("firefox",0, 0.15f,0.7f,"firefoxAudio0medium"),
                new AudioMeterListener("firefox",0, 0.7f,1f,"firefoxAudio0Hight")
            };


        public static ConfigurationProjectFromFileAtRoot Default= new ConfigurationProjectFromFileAtRoot();
        public static void LoadFileAtRoot(out ConfigurationProjectFromFileAtRoot fileFound) {
            string exePath = Directory.GetCurrentDirectory();
            string jsonImportPath = exePath + "/Configuration.json";
            string json = JsonConvert.SerializeObject(Default, Formatting.Indented);
            if (!File.Exists(jsonImportPath))
                File.WriteAllText(jsonImportPath, json);
            else json = File.ReadAllText(jsonImportPath);

            fileFound = JsonConvert.DeserializeObject<ConfigurationProjectFromFileAtRoot>(json);


        }
    }




}
