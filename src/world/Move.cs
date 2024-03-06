using MHTools;
using NumSharp.Utilities;
using System.Threading;

namespace Shining_BeautifulGirls
{
    partial class World
    {
        private bool _stop = true;
        private string _lastClick = string.Empty;
        private static readonly Random _random = new();


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

                if (FastCheck(symbol, sim: sim))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 通过点击一组按钮，移动到某页面（先点击再检查）。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sec"></param>
        public void MoveTo(object[] data, int sec = 1, double sim = 0.9) =>
            MoveTo(() => FastCheck(data[0], sim: sim), bts: data[1..], sec: sec);

        public void MoveTo(Enum zone, Enum ptext, Enum button, int sec = 1) =>
            MoveTo(() => ExtractZoneAndContains(zone, ptext), button, sec);

        public void MoveTo(Func<bool> condition, Enum button, int sec = 1) =>
            MoveTo(condition, [button], sec);

        /// <summary>
        /// (原始定义) 点击一组按钮，判断是否满足条件，若满足条件则结束（先点击再检查）
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="bts"></param>
        /// <param name="sec"></param>
        public void MoveTo(Func<bool> condition, object[] bts, int sec = 1)
        {
            MoveControl.Builder
                .SetButtons(bts)
                .AddTarget(condition)
                .StartAsMoveTo(this, sec: sec);
        }

        /// <summary>
        /// 确保一定点击的是某页面下的按钮；也可用于确保跳转到某页面（当按钮置空时）。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sim"></param>
        public void PageDown(object[] data, double sim = 0.9)
            => PageDown(() => FastCheck(data[0], sim: sim), data[1..]);

        public void PageDown(Enum zone, Enum ptext, params Button[] buttons) =>
            PageDown(() => ExtractZoneAndContains(zone, ptext), [.. buttons]);

        /// <summary>
        /// (原始定义) 不断检查是否满足条件，当满足条件时，点击一组按钮然后结束 (不满足条件时不会产生点击行为)
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="bts"></param>
        public void PageDown(Func<bool> condition, params object[] bts)
        {
            MoveControl.Builder
                .SetButtons(bts)
                .AddTarget(condition)
                .StartAsPageDown(this, step: 400);
        }


        /// <summary>
        /// 获取屏幕最新状态
        /// </summary>
        /// <exception cref="LongTimeNoOperationException"></exception>
        public void Refresh()
        {
            lock (_screenlock)
            {
                if (ADB.CopyScreen(DeviceID))
                    return;
            }

            throw new LongTimeNoOperationException();
        }

        /// <summary>
        /// (刷新) 暂停等待一段时间，结束后刷新
        /// </summary>
        /// <param name="time"></param>
        public void Pause(int time = 200)
        {
            if (time < 1)
                return;
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
                    StartOverTimer(60);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            // 获取最新界面
            Refresh();
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

        public void Click(object[]? bts, int pauseTime = 200)
        {
            if (bts is not null)
            {
                for (int i = 0; i < bts.Length; i++)
                    Click(bts[i], pauseTime);
            }
        }

        public void ClickLast()
        {
            Click(_lastClick);
        }

        public void Scroll(double[] start, double distance)
        {
            int time = 1000;
            if (Math.Abs(distance) > 1999)
                time = 200;
            else
            {
                int L = (int)distance;
                int size = 300;
                while (L > size)
                {
                    L -= size;
                    time += 1000;
                }
            }
            ADB.Swipe(start, [start[0], start[1] - distance], time);
        }

        public void Scroll(double[] data)
        {
            Scroll(data.Slice(0, 2), data[^1]);
        }
    }
}
