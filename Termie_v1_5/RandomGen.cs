using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace Termie
{
    public sealed class RandomGen
    {
        Thread _genThread;
        volatile bool _bIsGenerating;
        volatile bool _bKeepAlive;
        volatile bool _bZero;

        //begin Singleton pattern
        static readonly RandomGen instance = new RandomGen();

        static RandomGen()
        {
        }

        RandomGen()
        {
            _bIsGenerating = false;
            _bZero = true;
        }

        public static RandomGen Instance
        {
            get
            {
                return instance;
            }
        }
        public bool IsGenerating
        {
            get
            {
                return _bIsGenerating;
            }
        }
        public void ChangeGenStat()
        {
            _bIsGenerating = !_bIsGenerating;
        }
        public void ChangToZero()
        {
            _bZero = !_bZero;
        }

        public void CreateThread()
        {
            _bKeepAlive = true;
            _genThread = new Thread(RandomGenerate);
            _genThread.Start();
        }
        public class RealPacket
        {
            public float breath = 0, pressure = 0;
            public int LRPM = 0, RRPM = 0;
        }

        private void RandomGenerate()
        {
            // 스톱워치로 초당 5번 or Sleep함수로 강제 스탑? or 쓰레드 대기?
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            CommPort com = CommPort.Instance;

            //string strRandomString = "";
            byte[] packet= new byte[18];
            Random rRand = new Random();

            try
            {
                packet[0] = 2;
                packet[17] = 3;
                float[] packetValue = new float[4];
                while (_bKeepAlive)
                {
                    if (_bIsGenerating)// &&sw.ElapsedMilliseconds > 1.0F)
                    {
                        sw.Restart();
                        if (_bZero)
                        {
                            for(int i = 0; i < 4; ++i)
                            {
                                packetValue[i] = (float)rRand.NextDouble() * 100.0F;
                            }
                            //a = (float)rRand.NextDouble() * 100.0F;
                            //b = (float)rRand.NextDouble() * 100.0F;
                            //c = (float)rRand.NextDouble() * 100.0F;
                            //d = (float)rRand.NextDouble() * 100.0F;
                        }
                        else
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                packetValue[i] = 0;
                            }
                        }

                        for(int i = 0; i < 4; i++)
                        {
                            byte[] FloatToByte = System.BitConverter.GetBytes(packetValue[i]);
                            Buffer.BlockCopy(packetValue, 0, packet, 1, 16);
                        }
                        //strRandomString = start.ToString() + a.ToString() + b.ToString() + c.ToString() + d.ToString() + end.ToString();
                        //strRandomString = rRand.Next(0, 1000).ToString() + Token.seriesToken + rRand.Next(0, 20).ToString() + Token.seriesToken + Token.lineToken;
                        com.Send(packet);
                    }
                    else
                    {
                        TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 50);
                        Thread.Sleep(waitTime);
                    }
                }
            }
            catch (TimeoutException) { }
        }


        public void Close()
        {
            if (_bIsGenerating || _genThread.IsAlive)
            {
                _bKeepAlive = false;
                _bIsGenerating = false;
                _genThread.Join();
                _genThread = null;
            }
        }

    }
}
