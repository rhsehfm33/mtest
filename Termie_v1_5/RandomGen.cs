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
        private void RandomGenerate()
        {
            // 스톱워치로 초당 5번 or Sleep함수로 강제 스탑? or 쓰레드 대기?
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            CommPort com = CommPort.Instance;

            string strRandomString = "";

            Random rRand = new Random();

            try
            {
                float a;
                float b;
                float c;
                while (_bKeepAlive)
                {
                    if (_bIsGenerating &&sw.ElapsedMilliseconds > 200.0F)
                    {
                        sw.Restart();
                        if (_bZero)
                        {
                            a = (float)rRand.NextDouble() * 100.0F;
                            b = (float)rRand.NextDouble() * 100.0F;
                            c = (float)rRand.NextDouble() * 100.0F;
                        }
                        else
                        {
                            a = b = c = 0.0F;
                        }

                        strRandomString = a.ToString() + b.ToString() + c.ToString();
                        //strRandomString = rRand.Next(0, 1000).ToString() + Token.seriesToken + rRand.Next(0, 20).ToString() + Token.seriesToken + Token.lineToken;
                        com.Send(strRandomString);
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
