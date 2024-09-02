//using MemoryFileConnectionUtility;
//using UDPPusherDLL;

//namespace ListenToMixerForVolume
//{

//    public class CommunicationWithOutsideApp
//    {
//        public IMemoryFileConnectionSetGet m_memoryFileCom;
//        public IUdpMessagePusher m_udpPusherCom;

//        public void PushMessage(string cmd)
//        {
//            if (m_udpPusherCom != null)
//                m_udpPusherCom.PushCommandLine(cmd);

//            if (m_memoryFileCom != null)
//                m_memoryFileCom.AppendTextAtEnd(cmd);
//        }
//    }



//}
