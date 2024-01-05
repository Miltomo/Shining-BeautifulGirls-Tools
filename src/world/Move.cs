using NumSharp.Utilities;
using System;
using System.Threading;

namespace Shining_BeautifulGirls
{
    partial class World
    {
        private bool _stop = false;
        private string _lastClick;
        private static readonly Random _random = new();

        /// <summary>
        /// 通过点击一组按钮，移动到某页面（先点击再检查）。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sec"></param>
        public void MoveTo(string[] data, int sec = 3, double sim = 0.9)
        {
            var symbol = data[0];
            var bts = data.Slice(1, data.Length);

            int maxTime = sec * 1000;

            while (true)
            {
                Click(bts);

                int waitting = -1;
                int gap = 300;
                bool Aw = false;
                while (waitting < maxTime)
                {
                    Pause(gap);
                    if (CheckSymbol(symbol, delta: sim))
                    {
                        Aw = true;
                        break;
                    }
                    waitting += gap;
                }
                if (Aw)
                    break;
            }
        }

        public bool MoveToEx(string[][] data, string[][]? occurs = default, int sec = 1, double sim = 0.9)
        {
            var symbols = data[0];
            var bts = data[1];

            int maxTime = sec * 1000;
            bool exit = false;

            while (true)
            {
                if (occurs is not null)
                    for (int i = 0; i < occurs.Length; i++)
                    {
                        var dq = occurs[i];
                        if (CheckSymbol(dq[0]))
                        {
                            if (dq.Length == 1)
                            {
                                exit = true;
                                break;
                            }
                            Click(dq.Slice(1, dq.Length));
                        }
                    }

                if (exit)
                    return false;

                Click(bts);

                int waitting = -1;
                int gap = 300;
                bool Aw = false;
                while (!Aw && waitting < maxTime)
                {
                    Pause(gap);
                    foreach (var symbol in symbols)
                        if (CheckSymbol(symbol, delta: sim))
                        {
                            Aw = true;
                            break;
                        }
                    waitting += gap;
                }
                if (Aw)
                    break;
            }

            return true;
        }

        /// <summary>
        /// 确保一定点击的是某页面下的按钮；也可用于确保跳转到某页面（当按钮置空时）。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sim"></param>
        public void PageDown(string[] data, double sim = 0.9)
        {
            var symbol = data[0];
            var bt = "";
            if (data.Length > 1)
                bt = data[1];

            int waitting = 0;
            int gap = 300;
            while (true)
            {
                Pause(gap);
                if (CheckSymbol(symbol, delta: sim))
                {
                    Pause(1000);
                    Click(bt, 1);
                    break;
                }
                waitting += gap;
                if (waitting > 10000)
                {
                    Click(_lastClick);
                    waitting = 0;
                }
            }
        }

        public bool PageDownEx(string[] data, string[] ex, double sim = 0.9)
        {
            var symbol = data[0];
            var bt = "";
            if (data.Length > 1)
                bt = data[1];

            int waitting = 0;
            int gap = 300;
            while (true)
            {
                Pause(gap);
                if (CheckSymbol(symbol, delta: sim))
                {
                    Pause(1000);
                    Click(bt, 1);
                    break;
                }

                for (int i = 0; i < ex.Length; i++)
                    if (CheckSymbol(ex[i], delta: sim))
                        return false;

                waitting += gap;
                if (waitting > 10000)
                {
                    Click(_lastClick);
                    waitting = 0;
                }
            }
            return true;
        }

        public void Refresh()
        {
            ADB.GetScreen(DeviceID);
            Pause(20);
            /*if (CheckSymbol("主界面"))
            {
                Location = "主界面";

                Trace.WriteLine("当前在主界面");
            }
            else
                Location = "未知";*/
        }

        /// <summary>
        /// 暂停等待
        /// </summary>
        /// <param name="time"></param>
        public void Pause(int time = 500)
        {
            int remain = time;
            while (remain > 0)
            {
                if (_stop)
                    throw new Exception();

                int step = remain > 1000 ? 1000 : remain;
                //等待时间波动
                if (step > 100)
                    step += _random.Next(200) - 100;
                Thread.Sleep(step);

                remain -= step;
            }
        }

        public void Click(double x, double y, int pauseTime)
        {
            while (true)
            {
                if (ADB.Click(x, y))
                    break;
                Pause(20);
            }
            Pause(pauseTime);
        }

        public void Click(string? buttonName, int pauseTime = 200)
        {
            if (buttonName == null || buttonName == "")
                return;
            _lastClick = buttonName;
            Click(
                Width * ButtonLocation[buttonName][0],
                Height * ButtonLocation[buttonName][1],
                pauseTime
                );
        }

        public void Click(string[] bts)
        {
            for (int i = 0; i < bts.Length; i++)
                Click(bts[i]);
        }

        public void ClickEx(string buttonName, string occur, string[] bts)
        {
            Click(buttonName, 1000);
            if (CheckSymbol(occur))
                QuickClick(bts);
        }

        public void QuickClick(string buttonName)
        {
            Click(buttonName, 100);
        }

        public void QuickClick(string[] bts)
        {
            for (int i = 0; i < bts.Length; i++)
                QuickClick(bts[i]);
        }

        public void Scroll(double[] start, double distance)
        {
            int time = 1000;
            int L = (int)distance;
            int size = 300;
            while (L > size)
            {
                L -= size;
                time += 1000;
            }
            ADB.Swipe(start, [start[0], start[1] - distance], time);
        }

        public void Scroll(double[] data)
        {
            Scroll(data.Slice(0, 2), data[^1]);
        }
    }
}
