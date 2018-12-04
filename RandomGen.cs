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

        //begin Singleton pattern
        static readonly RandomGen instance = new RandomGen();

        static RandomGen()
        {
        }

        RandomGen()
        {
            _bIsGenerating = false;
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
                while (_bKeepAlive)
                {
                    if (_bIsGenerating &&sw.ElapsedMilliseconds > 200.0F)
                    {
                        sw.Restart();
                        strRandomString = 'e' + rRand.Next(0, 20).ToString() + '\n' + rRand.Next(0, 20).ToString();
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
