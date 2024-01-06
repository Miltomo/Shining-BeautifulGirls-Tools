using System;
using System.Diagnostics;
using System.Threading;

namespace MHTools
{
    public abstract class Base
    {
        public event Action<string>? LogEvent;
        private bool _stoptimer = true;
        private readonly object _timerlock = new();
        private Thread? threadTimer;

        protected virtual void OnLog(string text)
        {
            LogEvent?.Invoke(text);
        }

        /// <summary>
        /// 创建一个超时检测(倒)计时器。当倒计时结束时，触发异常。
        /// </summary>
        /// <param name="maxSecond"></param>
        /// <exception cref="LongTimeNoOperationException"></exception>
        protected void StartOverTimer(int maxSecond = 30)
        {
            lock (_timerlock)
            {
                if (_stoptimer)
                {
                    _stoptimer = false;

                    threadTimer = new(() =>
                    {
                        for (int i = maxSecond; i > 0; i--)
                        {
                            Debug.WriteLine($"{i}s");

                            for (int t = 0; t < 50; t++)
                            {
                                if (_stoptimer)
                                    return;
                                Thread.Sleep(20);
                            }
                        }
                        throw new LongTimeNoOperationException();
                    })
                    {
                        IsBackground = true
                    };
                    threadTimer.Start();
                }
            }
        }

        /// <summary>
        /// 立即停止超时检测倒计时器
        /// </summary>
        protected void StopOverTimer()
        {
            lock (_timerlock)
            {
                _stoptimer = true;
                // thread状态检查
                while (true)
                {
                    if (threadTimer == null || threadTimer.ThreadState == System.Threading.ThreadState.Stopped)
                        break;
                    Thread.Sleep(20);
                }
            }
        }
    }
}
