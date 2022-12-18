namespace ListenToMixerForVolume
{
    public class ConfigurationProjectFromFile {
        public int m_targetPortId=2507;
        public string m_targetIp = "127.0.0.1";
        public string m_memoryFileName= "OMICommandLines";
        public bool m_skipAreYouReady=true;
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
                new AudioMeterListener("wow",9, 0.05f,1f,"wowAudio9")
            };
    }




}
