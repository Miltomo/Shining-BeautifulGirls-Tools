using MHTools;
using NumSharp.Utilities;
using System;
using System.Linq;
using System.Threading;

namespace Shining_BeautifulGirls
{
    //TODO 继续重构M/P方法。增加原子操作
    partial class World
    {
        private bool _stop = true;
        private string _lastClick = string.Empty;
        private static readonly Random _random = new();
        private static readonly int refreshGAP = 500;

        /// <summary>
        /// 通过点击一组按钮，移动到某页面（先点击再检查）。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sec"></param>
        public void MoveTo(object[] data, int sec = 3, double sim = 0.9)
        {
            var symbol = data[0];
            var bts = data[1..];

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

                    if (FastCheck(symbol, sim: sim))
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
        public bool WaitTo(object[] data, double sim = 0.9, int maxWait = 5)
        {
            var symbol = data[0];
            var bts = data[1..];
            var count = maxWait * 2;

            for (int i = 0; i < count; i++)
            {
                Click(bts);
                Pause(300);
                Refresh();
                if (FastCheck(symbol, sim: sim))
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
        public bool MoveToEx(object[][] data, object[][]? occurs = default, int sec = 1, double sim = 0.9)
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
                        if (FastCheck(symbol, sim: sim))
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
                        if (FastCheck(dq[0], sim: sim))
                        {
                            if (dq.Length == 1)
                                return false;
                            Click(dq[1..]);
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
        public void PageDown(object[] data, double sim = 0.9)
        {
            var symbol = data[0];
            var bts = data[1..];

            int waitting = 0;
            while (true)
            {
                Pause(500);
                Refresh();

                if (FastCheck(symbol, sim: sim))
                {
                    if (bts.Length < 2)
                        Click(bts.FirstOrDefault(), 20);
                    else
                        Click(bts);
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

        public bool PageDownEx(object[] data, object[] ex, double sim = 0.9)
        {
            var symbol = data[0];
            var bts = data[1..];

            int waitting = 0;
            while (true)
            {
                Pause(500);
                Refresh();

                // 检测目标象征图
                if (FastCheck(symbol, sim: sim))
                {
                    if (bts.Length < 2)
                        Click(bts.FirstOrDefault(), 20);
                    else
                        Click(bts);
                    return true;
                }

                // 若出现意外象征图
                for (int i = 0; i < ex.Length; i++)
                    if (FastCheck(ex[i], sim: sim))
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
            if (ADB.CopyScreen(DeviceID))
                return;
            throw new LongTimeNoOperationException();
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
            bool Aw = false;
            if (remain > 2000)
            {
                Log($"等待剩余：{TimeTool.FormatMS(remain)}");
                Aw = true;
            }
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

                if (Aw)
                    UpdateLog($"等待剩余：{TimeTool.FormatMS(remain)}");
            }
            if (Aw)
                DeleteLog(1);
            // 启动超时计时器
            if (!_stop)
            {
                try
                {
                    StartOverTimer();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void Click(
            double x, double y,
            double dx = 0, double dy = 0,
            int pauseTime = 200
            )
        {
            var rx = _random.Next(2) == 0 ? _random.NextDouble() * dx : -_random.NextDouble() * dx;
            var ry = _random.Next(2) == 0 ? _random.NextDouble() * dy : -_random.NextDouble() * dy;
            ADB.Click(x + rx, y + ry);
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
                Width * ButtonLocation[buttonName][2],
                Height * ButtonLocation[buttonName][3],
                pauseTime
                );
        }

        public void Click(object[] bts)
        {
            for (int i = 0; i < bts.Length; i++)
                Click(bts[i]);
        }

        public void ClickEx(object bt, string occur_file, object[] bts)
        {
            Click(bt.ToString(), 1000);
            if (Check(occur_file))
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
