using CSCore.CoreAudioAPI;
using System;
using System.Threading;

namespace ListenToMixerForVolume
{
    public partial class Program
    {
        public class DisplayProcess {

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
            public static void ConsoleDisplayProcessinMixer()
            {
                using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                        foreach (var session in sessionEnumerator)
                        {
                            using (var session2 = session.QueryInterface<AudioSessionControl2>())
                            using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
                            {
                                Console.WriteLine("Process: {0}; Peak: {1:P}",
                                     session2.Process == null ? String.Empty : session2.Process.MainWindowTitle,
                                     audioMeterInformation.GetPeakValue() * 100);
                            }
                        }
                  }
                
            }
            public static void WhileLoopToDisplayMixer(ref bool whileExitbool)
            {
                using (var sessionManager =  GetDefaultAudioSessionManager2(DataFlow.Render))
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    while (whileExitbool)
                    {
                        foreach (var session in sessionEnumerator)
                        {
                            // Assert.IsNotNull(session);

                            using (var session2 = session.QueryInterface<AudioSessionControl2>())
                            using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
                            {
                                Console.WriteLine("Process: {0}; Peak: {1:P}",
                                     session2.Process == null ? String.Empty : session2.Process.MainWindowTitle,
                                     audioMeterInformation.GetPeakValue() * 100);
                            }
                        }
                        Thread.Sleep(100);
                    }
                }
            }

        }
    }
}
