﻿using MHTools;
using NumSharp.Utilities;
using System;
using System.Threading;

namespace Shining_BeautifulGirls
{
    //TODO 继续重构M/P方法。增加原子操作
    partial class World
    {
        private bool _stop = false;
        private string _lastClick;
        private static readonly Random _random = new();
        private static readonly int refreshGAP = 500;

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

                    if (FastSymbol(symbol, sim))
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
        /// 尝试移动到目标场所，等待数秒
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sim"></param>
        /// <param name="maxWait"></param>
        /// <returns>true,成功移动到目标; false,未能移动到目标</returns>
        public bool WaitTo(string[] data, double sim = 0.9, int maxWait = 5)
        {
            var symbol = data[0];
            var bts = data.Slice(1, data.Length);
            var count = maxWait * 2;

            for (int i = 0; i < count; i++)
            {
                Click(bts);
                Pause(300);
                Refresh();
                if (FastSymbol(symbol, sim))
                    return true;
            }
            return false;
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

            while (true)
            {
                // 点击按钮(组)
                Click(bts);

                // 判断是否到达目标界面
                int waitting = -1;
                while (true)
                {
                    Pause();
                    Refresh();

                    foreach (var symbol in symbols)
                        if (FastSymbol(symbol, sim))
                            return true;
                    waitting += refreshGAP;
                    if (waitting > maxTime)
                        break;
                }

                // 检查是否出现意外情况
                if (occurs is not null)
                {
                    for (int i = 0; i < occurs.Length; i++)
                    {
                        var dq = occurs[i];
                        if (FastSymbol(dq[0], sim))
                        {
                            if (dq.Length == 1)
                                return false;
                            Click(dq.Slice(1, dq.Length));
                        }
                    }
                }//if occurs
            }//while(true)
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
                Pause(500);
                Refresh();

                if (FastSymbol(symbol, sim))
                {
                    Click(bt, 20);
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
                Pause(500);
                Refresh();

                // 检测目标象征图
                if (FastSymbol(symbol, sim))
                {
                    Click(bt, 20);
                    return true;
                }

                // 若出现意外象征图
                for (int i = 0; i < ex.Length; i++)
                    if (FastSymbol(ex[i], sim))
                        return false;

                waitting += refreshGAP;
                if (waitting > 10000)
                {
                    Click(_lastClick);
                    waitting = 0;
                }
            }
        }

        public void Refresh()
        {
            ADB.GetScreen(DeviceID);
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
                    throw new StopException();

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

        public void Click(object? bt, int pauseTime = 200)
        {
            string? buttonName = bt?.ToString();
            if (string.IsNullOrWhiteSpace(buttonName))
                return;
            _lastClick = buttonName;
            Click(
                Width * ButtonLocation[buttonName][0],
                Height * ButtonLocation[buttonName][1],
                pauseTime
                );
        }

        public void Click(object[] bts)
        {
            for (int i = 0; i < bts.Length; i++)
                Click(bts[i]);
        }

        public void ClickEx(object bt, string occur, object[] bts)
        {
            Click(bt.ToString(), 1000);
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
