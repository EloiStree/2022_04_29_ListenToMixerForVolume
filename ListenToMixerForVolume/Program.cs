using CSCore.CoreAudioAPI;
using Microsoft.AspNet.SignalR.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ListenToMixerForVolume
{
    public class ConfigurationFile {
        public int m_targetPortId=2506;
        public string m_targetIp = "127.0.0.1";
        public float m_minSound = 0.05f;
        public float m_maxSound = 1f;
        public AudioMeterListener[] audioListeners = new AudioMeterListener[] {
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


    class Program
    {
    public static ConfigurationFile m_defaultConfig= new ConfigurationFile();
        //Source: https://stackoverflow.com/questions/23999531/cscore-application-audio-mixer-namepeak
        static void Main(string[] args)
        {

            string exePath = Directory.GetCurrentDirectory();
            string jsonImportPath = exePath + "/Configuration.json";

            string json = JsonConvert.SerializeObject(m_defaultConfig, Formatting.Indented);
            if (!File.Exists(jsonImportPath))
                File.WriteAllText(jsonImportPath, json);
            else json = File.ReadAllText(jsonImportPath);

            ConfigurationFile imported = JsonConvert.DeserializeObject<ConfigurationFile>(json);
            float minAudio = imported.m_minSound;
            float maxAudio = imported.m_maxSound;
            m_ipTarget = imported.m_targetIp;
            m_ipPort = imported.m_targetPortId;
            AudioMeterListener[] audioListeners = imported.audioListeners;
            //using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
            //using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
            //{
            //    while (true)
            //    {
            //        foreach (var session in sessionEnumerator)
            //        {
            //            // Assert.IsNotNull(session);

            //            using (var session2 = session.QueryInterface<AudioSessionControl2>())
            //            using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
            //            {
            //                Console.WriteLine("Process: {0}; Peak: {1:P}",
            //                     session2.Process == null ? String.Empty : session2.Process.MainWindowTitle,
            //                     audioMeterInformation.GetPeakValue() * 100);
            //            }
            //        }
            //        Thread.Sleep(100);
            //    }
            //}


            Dictionary<string, List<KeepTrackAudioMeter>> similarProcessName = new Dictionary<string, List<KeepTrackAudioMeter>>();

            

            Dictionary<string, bool> isOnRegister = new Dictionary<string, bool>();
            for (int i = 0; i < audioListeners.Length; i++)
            {
                isOnRegister.Add(audioListeners[i].GetId(), false);
            }

            List<KeepTrackAudioMeter> allAudioMeters = new List<KeepTrackAudioMeter>();
            using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
            using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
            {

                allAudioMeters.Clear();
                foreach (var session in sessionEnumerator)
                {

                    {
                        // Assert.IsNotNull(session);

                        var session2 = session.QueryInterface<AudioSessionControl2>();
                        var audioMeterInformation = session.QueryInterface<AudioMeterInformation>();
                        {
                            string processId = session2.Process == null ? String.Empty :
                                session2.Process.ProcessName.ToLower();
                            string title = session2.Process == null ?
                           String.Empty : session2.Process.MainWindowTitle.ToLower();
                            ;
                            Console.WriteLine("Process: {0}; Peak: {1:P}; ProcessID:{2}",
                             title,
                             audioMeterInformation.GetPeakValue() , processId);
                            KeepTrackAudioMeter tracked = new KeepTrackAudioMeter(session2, audioMeterInformation);
                            allAudioMeters.Add(tracked);
                            if (!similarProcessName.ContainsKey(processId))
                                similarProcessName.Add(processId, new List<KeepTrackAudioMeter>());
                            similarProcessName[processId].Add(tracked);


                        }
                    }
                }

                Console.WriteLine("Ready ?");
                Console.ReadLine();
                string valueFound = "";
                string valueFoundAsBool = "";
                string valueFoundAsBoolChanged = "";
                while (true)
                {
                    valueFound = "";
                    valueFoundAsBool = "";
                    valueFoundAsBoolChanged = "";
                    foreach (AudioMeterListener audioToConvert in audioListeners)
                    {

                        string lookFor = audioToConvert.m_processTitle.ToLower();
                        if (similarProcessName.ContainsKey(lookFor))
                        {

                            List<KeepTrackAudioMeter> audioMeter = similarProcessName[lookFor];
                            if (audioToConvert.m_processIndex < audioMeter.Count) {
                              var audio = audioMeter[audioToConvert.m_processIndex];
                                float volume = audio.m_audioMeterInformation.GetPeakValue();
                                valueFound += "\t" + string.Format("{0:0.00}",
                                 volume);
                                bool isOn = volume >= audioToConvert.m_minVolume && volume <= audioToConvert.m_maxVolume;
                                valueFoundAsBool += "\t" + isOn;

                                bool isOnPrevious = isOnRegister[audioToConvert.GetId()];
                                if (isOn != isOnPrevious) {
                                    isOnRegister[audioToConvert.GetId()] = isOn;
                                    valueFoundAsBoolChanged += "\t"+ audioToConvert.m_toBooleanName;
                                    SendMessageToUDP("bool:"+ audioToConvert.m_toBooleanName+ ":" + isOn);
                                }
                            }

                        }
                    }
                    Console.WriteLine(valueFound + " - " + valueFoundAsBool+" > "+ valueFoundAsBoolChanged);
                    Thread.Sleep(10);
                }

            }
        }

        public static string m_ipTarget="127.0.0.1";
        public static int    m_ipPort=2506;

        public static IPEndPoint m_destinationEndPoint;
        public static Socket m_destinationSock;
        private static void SendMessageToUDP(string message) {
            if (m_destinationSock == null)
            {
                m_destinationSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress serverAddr = IPAddress.Parse(m_ipTarget);
                m_destinationEndPoint = new IPEndPoint(serverAddr, m_ipPort);
            }
            m_destinationSock.SendTo(Encoding.ASCII.GetBytes(message), m_destinationEndPoint);
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    Console.WriteLine("DefaultDevice: " + device.FriendlyName);
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }
    }


}
