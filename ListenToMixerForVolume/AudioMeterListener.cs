namespace ListenToMixerForVolume
{
    public class AudioMeterListener
    {
        public string m_processTitle;
        public int m_processIndex;
        public float m_minVolume;
        public float m_maxVolume;
        public string m_toBooleanName;
        public string m_id;
        public AudioMeterListener(string processTitle, int processIndex, float minVolume, float maxVolume, string boolNameId)
        {
            this.m_processTitle = processTitle;
            this.m_processIndex = processIndex;
            this.m_minVolume = minVolume;
            this.m_maxVolume = maxVolume;
            m_toBooleanName = boolNameId;
            m_id = m_processTitle + processIndex + m_toBooleanName;
        }

        public string GetId() {
            return m_id;
        }
    }
}