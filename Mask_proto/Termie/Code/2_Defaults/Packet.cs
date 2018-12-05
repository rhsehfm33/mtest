using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Termie
{
    public static class Token       // 토큰을 정의해 놓은 클래스. c++의 #defin 역할.
    {
        public const char seriesToken = '\t';
        public const char lineToken = '\n';
    }
    public enum PacketDataType
    {
        eBreath,
        ePressure,
        eRPM,
        eEND
    }

    public class Packet
    {

        public float[] m_DataIn = new float[(int)PacketDataType.eEND];

        public Packet() { }

        public Packet SetData(byte[] bytes, int index, int count)
        {
            m_DataIn[(int)PacketDataType.eBreath] = BitConverter.ToSingle(bytes, count - sizeof(float));
            m_DataIn[(int)PacketDataType.ePressure] = BitConverter.ToSingle(bytes, count - sizeof(float) * 2);
            m_DataIn[(int)PacketDataType.eRPM] = BitConverter.ToSingle(bytes, count - sizeof(float) * 3);

            return this;
        }
    }
    
}
