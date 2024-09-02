using CSCore.CoreAudioAPI;
using Microsoft.AspNet.SignalR.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ListenToMixerForVolume
{
    public partial class Program
    {

        public class TargetPortIp
        {
            public string m_targetIp = "127.0.0.1";
            public int m_targetPortId = 0;
        }

        public static TargetPortIp m_targetPortIp = new TargetPortIp();

        ////Source: https://stackoverflow.com/questions/23999531/cscore-application-audio-mixer-namepeak
        static void Main(string[] args)
        {
            Console.WriteLine("Hello !");
            ConfigurationProjectFromFileAtRoot defaultConfig = new ConfigurationProjectFromFileAtRoot();
            DisplayProcess.ConsoleDisplayProcessinMixer();

            ConfigurationProjectFromFileAtRoot.LoadFileAtRoot
                (out ConfigurationProjectFromFileAtRoot imported);

            m_targetPortIp.m_targetIp = imported.m_targetIp;
            m_targetPortIp.m_targetPortId = imported.m_targetPortId;
            
            Console.WriteLine("Target: " + m_targetPortIp.m_targetIp + ":" + m_targetPortIp.m_targetPortId);



          
            TrackAudioVolumeToBoolean audioTracker = new TrackAudioVolumeToBoolean();


            audioTracker.AddBooleanChangeListener((in string booleanName, in bool isOn) =>
            {
                BooleanCommandToUtility.GetBooleanSetCommand(in booleanName, in isOn, out string cmd);
                //communication.PushMessage( cmd);
            });

            audioTracker.InitWithAudi(imported.m_audioListeners);

            if (!imported.m_skipAreYouReady)
            {
                Console.WriteLine("ready ?");
                Console.ReadLine();
            }

            ulong l = 0;
            while (true)
            {
                audioTracker.RefreshAndPushStateChanged();
                Thread.Sleep(imported.m_timeInMilliseconds);
                l++;
                if (l % 1000 == 0)
                {

                    Console.WriteLine(". "+DateTime.Now);
                }

            }
        }
    }

    public class BooleanCommandToUtility{

        public static void GetBooleanSetCommand(in string booleanName, in bool isOn, out string commandLine) {
            commandLine= "Ḇ" + (isOn ? 1 : 0) + booleanName;
        }
}

    public class TrackAudioVolumeToBoolean {

        public bool m_useDebugLog=false;
        public delegate void VolumeStateChange(in string name, in bool booleanValue);
        public VolumeStateChange m_onChanged = null;
        public Dictionary<string, List<KeepTrackAudioMeter>> m_groupOfObservedPerProcessNameId =  new Dictionary<string, List<KeepTrackAudioMeter>>();
        public Dictionary<string, bool> m_isInRangeOfVolumeRegister = new Dictionary<string, bool>();

        public AudioMeterListener[] m_audioGivenToTrack;

        public void AddBooleanChangeListener(VolumeStateChange p)
        {
            m_onChanged += p;
        }
        public void RemoveBooleanChangeListener(VolumeStateChange p)
        {
            m_onChanged -= p;
        }

        public void InitWithAudi(AudioMeterListener[] AudioMeterListener)
        {
            m_audioGivenToTrack = AudioMeterListener;
            InitTheTrueOrFalseRegisterForProcess();

            m_groupOfObservedPerProcessNameId.Clear();
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
                            //session2.Process.Handle

                            Window32DLL.GetActiveChildOfProcessId(session2.Process.MainWindowHandle, out IntPtr childId);
                            KeepTrackAudioMeter tracked = new KeepTrackAudioMeter(session2, audioMeterInformation);
                            allAudioMeters.Add(tracked);
                            if (!m_groupOfObservedPerProcessNameId.ContainsKey(processId))
                                m_groupOfObservedPerProcessNameId.Add(processId, new List<KeepTrackAudioMeter>());
                            m_groupOfObservedPerProcessNameId[processId].Add(tracked);

                           // if(m_useDebugLog)
                                Console.WriteLine("Process: {0}; Peak: {1:P}; Process:{2} Index: {4} Thread ID: {3}",//- Active child {5} (on {6})",
                            title,
                            audioMeterInformation.GetPeakValue(), processId,
                            (int)session2.Process.MainWindowHandle,
                            m_groupOfObservedPerProcessNameId[processId].Count - 1
                            //(int)childId,
                            //GetProcessIdChildrenWindows((int)session2.Process.MainWindowHandle).Length
                            ); ;

                            //Console.WriteLine("Test C");
                            //Console.WriteLine("<Process: {0}; Peak: {1:P}; Process:{2} Index:{4}  Thread ID: {3}",
                            // title,
                            // audioMeterInformation.GetPeakValue(),
                            // processId,
                            // (int)session2.Process.MainWindowHandle,
                            //0);
                        }
                    }
                }


            }
        }

        private void InitTheTrueOrFalseRegisterForProcess()
        {
            m_isInRangeOfVolumeRegister.Clear();
            for (int i = 0; i < m_audioGivenToTrack.Length; i++)
            {
                string id = m_audioGivenToTrack[i].GetGenerateProcessToBooleanId();
                if (!m_isInRangeOfVolumeRegister.ContainsKey(id))
                    m_isInRangeOfVolumeRegister.Add(id, false);
            }
        }

        public void RefreshAndPushStateChanged() {


            string valueFound = "";
            string valueFoundAsBool = "";
            string valueFoundAsBoolChanged = "";
            {
                foreach (AudioMeterListener audioToConvert in m_audioGivenToTrack)
                {

                    string lookFor = audioToConvert.m_processTitle.ToLower();
                    if (m_groupOfObservedPerProcessNameId.ContainsKey(lookFor))
                    {

                        List<KeepTrackAudioMeter> audioMeter = m_groupOfObservedPerProcessNameId[lookFor];
                        if (audioToConvert.m_processIndex < audioMeter.Count)
                        {
                            var audio = audioMeter[audioToConvert.m_processIndex];
                            float volume = audio.m_audioMeterInformation.GetPeakValue();

                            if (m_useDebugLog)
                                valueFound += "\t" + string.Format("{0:0.00}", volume);

                            bool isOn = volume >= audioToConvert.m_minVolume && volume <= audioToConvert.m_maxVolume;

                            if (m_useDebugLog)
                                valueFoundAsBool += "\t" + isOn;

                            bool isOnPrevious = m_isInRangeOfVolumeRegister[audioToConvert.GetGenerateProcessToBooleanId()];
                            if (isOn != isOnPrevious)
                            {
                                m_isInRangeOfVolumeRegister[audioToConvert.GetGenerateProcessToBooleanId()] = isOn;


                                if (m_useDebugLog)
                                    valueFoundAsBoolChanged += "\t" + audioToConvert.m_toBooleanName;
                                if (m_onChanged != null)
                                    m_onChanged(audioToConvert.m_toBooleanName, isOn);

                                int windowIntPointer = audio.GetWindowIntegerPointer();
                                string cmd = $"{(isOn == true ? '1' : '0')}:{windowIntPointer}:{audioToConvert.m_processIndex}:{audioToConvert.m_toBooleanName}";
                                Console.WriteLine("Pushing: " + cmd);

                                Console.WriteLine("Target: " + Program.m_targetPortIp.m_targetIp + ":" + Program.m_targetPortIp.m_targetPortId);
                                SendMessage(Program.m_targetPortIp.m_targetIp, Program.m_targetPortIp.m_targetPortId, cmd);
                                //communication.PushMessage(cmd);
                            }
                        }

                    }
                }
                if (m_useDebugLog)
                    Console.WriteLine(valueFound + " - " + valueFoundAsBool + " > " + valueFoundAsBoolChanged);
               
            }

        }


        public static void SendMessage(string ipAddress, int port, string message)
        {
            // Create a UDP client
            using (UdpClient client = new UdpClient())
            {
                // Convert the message to bytes
                byte[] bytes = Encoding.UTF8.GetBytes(message);

                // Send the message to the specified IP address and port
                client.Send(bytes, bytes.Length, ipAddress, port);

            }
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
