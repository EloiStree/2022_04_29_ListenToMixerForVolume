﻿using CSCore.CoreAudioAPI;

namespace ListenToMixerForVolume
{
    public class KeepTrackAudioMeter
    {
        public AudioSessionControl2 m_session2;
        public AudioMeterInformation m_audioMeterInformation;
        public string m_trackTitle;
        public KeepTrackAudioMeter(AudioSessionControl2 session2, AudioMeterInformation audioMeterInformation)
        {
            this.m_session2 = session2;
            this.m_audioMeterInformation = audioMeterInformation;
            m_trackTitle = m_session2.Process.MainWindowTitle;
        }

        public int GetWindowIntegerPointer() { 
         return m_session2.Process.MainWindowHandle.ToInt32();
        }
    }
}