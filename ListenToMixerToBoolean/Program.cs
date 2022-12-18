using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MemoryFileConnectionUtility;
using UDPPusherDLL;

namespace ListenToMixerForVolume
{
    public class Program
    {

        static void Main(string[] args)
        {


        }


        //    public class CommunicationWithOutsideApp { 

        //    public  IMemoryFileConnectionSetGet m_memoryFileCom;
        //    public  IUdpMessagePusher m_udpPusherCom;

        //        public void PushMessage(in string cmd)
        //        {
        //            m_udpPusherCom.PushCommandLine(cmd);
        //            m_memoryFileCom.AppendTextAtEnd(cmd);
        //        }
        //    }
        //    public  static CommunicationWithOutsideApp m_communicationGates = new CommunicationWithOutsideApp();
        //    public static ConfigurationProjectFromFile m_defaultConfig= new ConfigurationProjectFromFile();

        //    //Source: https://stackoverflow.com/questions/23999531/cscore-application-audio-mixer-namepeak
        //    static void Main(string[] args)
        //    {
        //        ConfigurationProjectFromFile imported;
        //        AudioMeterListener[] audioListeners;
        //        ExtractImporter(out imported, out audioListeners);

        //        UdpMessagePusherBuilder.Connect(imported.m_targetIp, imported.m_targetPortId, out m_communicationGates.m_udpPusherCom);

        //        DisplayInformationLoop();

        //        Dictionary<string, List<KeepTrackAudioMeter>> similarProcessName = new Dictionary<string, List<KeepTrackAudioMeter>>();



        //        Dictionary<string, bool> isOnRegister = new Dictionary<string, bool>();
        //        for (int i = 0; i < audioListeners.Length; i++)
        //        {
        //            isOnRegister.Add(audioListeners[i].GetId(), false);
        //        }

        //        List<KeepTrackAudioMeter> allAudioMeters = new List<KeepTrackAudioMeter>();
        //        using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
        //        using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
        //        {

        //            allAudioMeters.Clear();
        //            foreach (var session in sessionEnumerator)
        //            {

        //                {
        //                    // Assert.IsNotNull(session);

        //                    var session2 = session.QueryInterface<AudioSessionControl2>();
        //                    var audioMeterInformation = session.QueryInterface<AudioMeterInformation>();
        //                    {
        //                        string processId = session2.Process == null ? String.Empty :
        //                            session2.Process.ProcessName.ToLower();
        //                        string title = session2.Process == null ?
        //                       String.Empty : session2.Process.MainWindowTitle.ToLower();
        //                        ;
        //                        //session2.Process.Handle

        //                        GetActiveChildOfProcessId(session2.Process.MainWindowHandle, out IntPtr childId);
        //                        KeepTrackAudioMeter tracked = new KeepTrackAudioMeter(session2, audioMeterInformation);
        //                        allAudioMeters.Add(tracked);
        //                        if (!similarProcessName.ContainsKey(processId))
        //                            similarProcessName.Add(processId, new List<KeepTrackAudioMeter>());
        //                        similarProcessName[processId].Add(tracked);

        //                        Console.WriteLine("Process: {0}; Peak: {1:P}; Process:{2} Index: {4} Thread ID: {3}",//- Active child {5} (on {6})",
        //                         title,
        //                         audioMeterInformation.GetPeakValue(), processId,
        //                         (int)session2.Process.MainWindowHandle,
        //                         similarProcessName[processId].Count - 1
        //                         //(int)childId,
        //                         //GetProcessIdChildrenWindows((int)session2.Process.MainWindowHandle).Length
        //                         ); ;

        //                        //Console.WriteLine("Test C");
        //                        //Console.WriteLine("<Process: {0}; Peak: {1:P}; Process:{2} Index:{4}  Thread ID: {3}",
        //                        // title,
        //                        // audioMeterInformation.GetPeakValue(),
        //                        // processId,
        //                        // (int)session2.Process.MainWindowHandle,
        //                        //0);
        //                    }
        //                }
        //            }

        //            if (imported.m_skipAreYouReady)
        //            {
        //                Console.WriteLine("Ready ?");
        //                Console.ReadLine();
        //            }
        //            string valueFound = "";
        //            string valueFoundAsBool = "";
        //            string valueFoundAsBoolChanged = "";
        //            while (true)
        //            {
        //                valueFound = "";
        //                valueFoundAsBool = "";
        //                valueFoundAsBoolChanged = "";
        //                foreach (AudioMeterListener audioToConvert in audioListeners)
        //                {

        //                    string lookFor = audioToConvert.m_processTitle.ToLower();
        //                    if (similarProcessName.ContainsKey(lookFor))
        //                    {

        //                        List<KeepTrackAudioMeter> audioMeter = similarProcessName[lookFor];
        //                        if (audioToConvert.m_processIndex < audioMeter.Count)
        //                        {
        //                            var audio = audioMeter[audioToConvert.m_processIndex];
        //                            float volume = audio.m_audioMeterInformation.GetPeakValue();
        //                            valueFound += "\t" + string.Format("{0:0.00}",
        //                             volume);
        //                            bool isOn = volume >= audioToConvert.m_minVolume && volume <= audioToConvert.m_maxVolume;
        //                            valueFoundAsBool += "\t" + isOn;

        //                            bool isOnPrevious = isOnRegister[audioToConvert.GetId()];
        //                            if (isOn != isOnPrevious)
        //                            {
        //                                isOnRegister[audioToConvert.GetId()] = isOn;
        //                                valueFoundAsBoolChanged += "\t" + audioToConvert.m_toBooleanName;
        //                                //string cmd = "bool:" + audioToConvert.m_toBooleanName + ":" + isOn;
        //                                //string cmd = "ᛒ" + (isOn ? 1 : 0) + audioToConvert.m_toBooleanName;
        //                                string cmd = "Ḇ" + (isOn ? 1 : 0) + audioToConvert.m_toBooleanName;
        //                                m_communicationGates.PushMessage(cmd);
        //                            }
        //                        }

        //                    }
        //                }
        //                Console.WriteLine(valueFound + " - " + valueFoundAsBool + " > " + valueFoundAsBoolChanged);
        //                Thread.Sleep(10);
        //            }

        //        }
        //    }

        //    private static void DisplayInformationLoop()
        //    {
        //        using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
        //        using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
        //        {
        //            while (true)
        //            {
        //                foreach (var session in sessionEnumerator)
        //                {
        //                    // Assert.IsNotNull(session);

        //                    using (var session2 = session.QueryInterface<AudioSessionControl2>())
        //                    using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
        //                    {
        //                        Console.WriteLine("Process: {0}; Peak: {1:P}",
        //                             session2.Process == null ? String.Empty : session2.Process.MainWindowTitle,
        //                             audioMeterInformation.GetPeakValue() * 100);
        //                    }
        //                }
        //                Thread.Sleep(100);
        //            }
        //        }
        //    }

        //    private static void ExtractImporter(out ConfigurationProjectFromFile imported, out AudioMeterListener[] audioListeners)
        //    {
        //        string exePath = Directory.GetCurrentDirectory();
        //        string jsonImportPath = exePath + "/Configuration.json";
        //        string json = JsonConvert.SerializeObject(m_defaultConfig, Formatting.Indented);
        //        if (!File.Exists(jsonImportPath))
        //            File.WriteAllText(jsonImportPath, json);
        //        else json = File.ReadAllText(jsonImportPath);

        //        imported = JsonConvert.DeserializeObject<ConfigurationProjectFromFile>(json);
        //        audioListeners = imported.m_audioListeners;
        //        m_communicationGates = new CommunicationWithOutsideApp();

        //        MemoryFileConnectionFacade.CreateConnection(MemoryFileConnectionType.MemoryFileLocker,
        //            imported.m_memoryFileName, out m_communicationGates.m_memoryFileCom, 1000000);

        //        if (imported.m_targetPortId == 0)
        //            imported.m_targetPortId = UDPPusherDefault.m_defaultIpPort;
        //        if (imported.m_targetIp.Trim().Length == 0)
        //            imported.m_targetIp = UDPPusherDefault.m_defaultIpAddress;
        //    }

        //    private static void GetActiveChildOfProcessId(IntPtr mainWindowHandle, out IntPtr childId)
        //    {
        //        FetchFirstChildrenThatHasDimension(mainWindowHandle, out bool found, out childId);
        //    }



        //    private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        //    {
        //        using (var enumerator = new MMDeviceEnumerator())
        //        {
        //            using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
        //            {
        //                Console.WriteLine("DefaultDevice: " + device.FriendlyName);
        //                var sessionManager = AudioSessionManager2.FromMMDevice(device);
        //                return sessionManager;
        //            }
        //        }
        //    }





        //    public static void FetchFirstChildrenThatHasDimension(IntPtr intPtr, out bool foundChild, out IntPtr target)
        //    {
        //        RectPadValue rect = new RectPadValue();
        //        IntPtr[] ptrs = GetProcessIdChildrenWindows((int)intPtr);
        //        for (int i = 0; i < ptrs.Length; i++)
        //        {
        //            GetWindowRect(ptrs[i], ref rect);
        //            if (rect.IsNotZero())
        //            {
        //                foundChild = true;
        //                target = ptrs[i];
        //                return;
        //            }
        //        }
        //        foundChild = false;
        //        target = IntPtr.Zero;
        //    }

        //    public static IntPtr[] GetProcessIdChildrenWindows(int process)
        //    {
        //        IntPtr[] apRet = (new IntPtr[256]);
        //        int iCount = 0;
        //        IntPtr pLast = IntPtr.Zero;
        //        do
        //        {
        //            pLast = FindWindowEx(IntPtr.Zero, pLast, null, null);
        //            int iProcess_;
        //            GetWindowThreadProcessId(pLast, out iProcess_);
        //            if (iProcess_ == process) apRet[iCount++] = pLast;
        //        } while (pLast != IntPtr.Zero);
        //        System.Array.Resize(ref apRet, iCount);
        //        return apRet;
        //    }


        //    [DllImport("user32.dll")]
        //    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        //    [DllImport("user32.dll")]
        //    public static extern bool GetWindowRect(IntPtr hwnd, ref RectPadValue rectangle);

        //    [DllImport("user32.dll")]
        //    public static extern IntPtr FindWindowEx(IntPtr parentWindow, IntPtr previousChildWindow, string windowClass, string windowTitle);


        //    public struct RectPadValue
        //    {
        //        //DONT CHANGE THE ORDER Of THE INT left, top, right, bot
        //        public int Left;
        //        public int Top;
        //        public int Right;
        //        public int Bottom;

        //        public bool IsNotZero()
        //        {
        //            return Left != 0 ||
        //                 Top != 0 ||
        //                 Right != 0 ||
        //                 Bottom != 0;
        //        }
        //        public bool IsEqualZero()
        //        {
        //            return Left == 0 &&
        //                Top == 0 &&
        //                Right == 0 &&
        //                Bottom == 0;
        //        }

        //    }
        //}




    }
}