using MHTools;
using NumSharp.Utilities;
using System;
using System.Threading;
using static ComputerVision.ImageRecognition;

namespace Shining_BeautifulGirls
{
    partial class World
    {
        private bool _stop = false;
        private string _lastClick;
        private static readonly Random _random = new();
        private static readonly int refreshGAP = 200;

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
                bool Aw = false;
                while (waitting < maxTime)
                {
                    Pause();
                    Refresh();

                    if (FastSymbolCheck(symbol, sim))
                    {
                        Aw = true;
                        break;
                    }
                    waitting += refreshGAP;
                }
                if (Aw)
                    break;
            }
        }

        /// <summary>
        /// 通过点击一组按钮，移动到某页面（先点击再检查）。并指定可能出现的意外情况。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="occurs"></param>
        /// <param name="sec"></param>
        /// <param name="sim"></param>
        /// <returns>true，当成功移动到目标页面时；false，当意外情况出现时。</returns>
        public bool MoveToEx(string[][] data, string[][]? occurs = default, int sec = 1, double sim = 0.9)
        {
            var symbols = data[0];
            var bts = data[1];

            int maxTime = sec * 1000;
            bool exit = false;

            while (true)
            {
                // 点击按钮(组)
                Click(bts);

                // 判断是否到达目标界面
                int waitting = -1;
                bool Aw = false;
                while (true)
                {
                    Pause();
                    Refresh();

                    foreach (var symbol in symbols)
                        if (FastSymbolCheck(symbol, sim))
                        {
                            Aw = true;
                            break;
                        }
                    waitting += refreshGAP;
                    if (Aw || waitting > maxTime)
                        break;
                }
                if (Aw)
                    break;

                // 检查是否出现意外情况
                if (occurs is not null)
                {
                    for (int i = 0; i < occurs.Length; i++)
                    {
                        var dq = occurs[i];
                        if (FastSymbolCheck(dq[0], sim))
                        {
                            if (dq.Length == 1)
                            {
                                exit = true;
                                break;
                            }
                            Click(dq.Slice(1, dq.Length));
                        }
                    }
                }//if occurs

                if (exit) return false;
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
            while (true)
            {
                Pause();
                Refresh();

                if (FastSymbolCheck(symbol, sim))
                {
                    Pause();
                    Click(bt, 1);
                    break;
                }
                waitting += refreshGAP;
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
            while (true)
            {
                Pause();
                Refresh();

                if (FastSymbolCheck(symbol, sim))
                {
                    Pause();
                    Click(bt, 1);
                    break;
                }

                for (int i = 0; i < ex.Length; i++)
                    if (FastSymbolCheck(ex[i], sim))
                        return false;

                waitting += refreshGAP;
                if (waitting > 10000)
                {
                    Click(_lastClick);
                    waitting = 0;
                }
            }
            return true;
        }

        /// <summary>
        /// (无刷新) 直接开始匹配目标象征物
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        public bool FastSymbolCheck(string symbol, double sim = 0.9)
        {
            /*return FeatureJudge(
                FileManagerHelper.SetDir(SymbolDir).Find(symbol)!,
                Screen);*/

            return
                MatchImage(FileManagerHelper.SetDir(SymbolDir).Find(symbol)!,
                Screen,
                out _)
                >
                sim;
        }


        public void Refresh()
        {
            ADB.GetScreen(DeviceID);
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
        public void Pause(int time = 200)
        {
            // 关闭超时计时器
            StopOverTimer();
            int remain = time;
            while (remain > 0)
            {
                if (_stop)
                    throw new UserStopException();

                int step = remain > 1000 ? 1000 : remain;
                remain -= step;

                // 等待时间波动
                if (step > 100)
                    step += _random.Next(200) - 100;
                Thread.Sleep(step);
            }
            // 启动超时计时器
            try
            {
                StartOverTimer();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Click(double x, double y, int pauseTime)
        {
            while (true)
            {
                if (ADB.Click(x, y))
                    break;
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
                Click(bts);
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
