using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Termie
{
    //public static class Token       // 토큰을 정의해 놓은 클래스. c++의 #defin 역할.
    //{
    //    public const char seriesToken = '\t';
    //    public const char lineToken = '\n';
    //}
    public static class PacketToken
    {
        public const byte start = 0x2;
        public const byte end = 0x3;
        //public const float breath
        //Pressure,
        //RRPM,
    }
    public static class mDataSize
    {
        public const int size = 4;
    }
    public static class PacketDataTypeIdx
    {
        public const int Breath = 0;
        public const int Pressure = 1;
        public const int LRPM = 2;
        public const int RRPM = 3;
    }
    public class RealPacket
    {
        public float breath = 0, pressure = 0;
        public int LRPM = 0, RRPM = 0;
    }
    public class Packet
    {
        
        public float[] m_DataIn = new float[mDataSize.size];

        public Packet() { }

        public float ConvertBreathToFloat(byte[] startbyte)
        {
            int sign = 1, breathValue = 0;
            if (startbyte[0]== 0)
                sign = -1;
            breathValue = startbyte[1] << 16 | startbyte[2] << 8 | startbyte[3];
            breathValue *= sign;

            return breathValue / 10000.0f;
        }

        public RealPacket SetData(byte[] bytes, int index, int count)
        {
            RealPacket packet = new RealPacket();
            int idx = 4;
            for(; count >= 0 && idx >=0; count--)           // 최적화 필요. 조건문 더 추가해야됨. 현재는 임시방편임.
            {
                if (bytes[count] == PacketToken.end) {
                    if (count < 13)
                        return null;
                    idx--;
                    continue;
                }
                else if (bytes[count] == PacketToken.start) {
                    break;
                }
                
                else
                {
                    switch (idx)
                    {
                        case 3:
                            packet.RRPM = bytes[count - 1] << 8 | bytes[count];        //최적화 필요.
                            count--;
                            idx--;
                            break;
                        case 2:
                            packet.LRPM = bytes[count - 1] << 8 | bytes[count];
                            count--;
                            idx--;
                            break;
                        case 1:
                            packet.pressure = (bytes[count-3] << 24 | bytes[count-2] 
                                << 16 | bytes[count-1]<<8 | bytes[count]) / 1000f;
                            count -= 3;
                            idx--;
                            break;
                        case 0:
                            byte[] startBytes = new byte[4];
                            Buffer.BlockCopy(bytes, count - 3, startBytes, 0, 4);
                            packet.breath = ConvertBreathToFloat(startBytes);
                            idx--;
                            break;
                    }
                    
                }
            }
            return packet;
        }
        public Packet ZeroData()
        {
            m_DataIn[0] = m_DataIn[1] = m_DataIn[2] = 0.0F;
            return this;
        }
    }
    
}
