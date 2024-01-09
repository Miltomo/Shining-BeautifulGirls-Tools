using System.IO;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        private void Click(string buttonName, int wait = 200)
        {
            Mnt.Click(buttonName, wait);
        }

        private void ClickEx(string buttonName, string occur, string[] bts)
        {
            Mnt.ClickEx(buttonName, occur, bts);
        }
        private void Choose(string buttonName, int wait = 200)
        {
            Click(buttonName, wait);
            Mnt.Refresh();
        }

        /// <summary>
        /// (刷新) 检查是否包含目标象征图
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="bg"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        private bool Check(string symbol, string? bg = default, double sim = 0.9)
        {
            return Match(symbol, bg) > sim;
        }

        /// <summary>
        /// (无刷新) 直接检查背景图是否包含目标象征物
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        private bool FastCheck(string symbol, double sim = 0.9)
        {
            return Mnt.FastSymbolCheck(symbol, sim);
        }

        /// <summary>
        /// (刷新) 匹配象征图像
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="bg"></param>
        /// <returns>相关度</returns>
        private double Match(string symbol, string? bg = default)
        {
            return Match(out _, symbol, bg);
        }

        private double Match(out OpenCvSharp.Point loc, string symbol, string? bg = default)
        {
            return Mnt.Match(out loc, Path.Combine(World.SymbolDir, $"{symbol}.png"), bg);
        }

        private void MoveTo(string destination, string[] data)
        {
            if (Location == destination)
                return;
            MoveTo(data, 3, 0.8);
            Location = destination;
        }

        private void MoveTo(string[] data, int sec = 1, double sim = 0.9)
        {
            Mnt.MoveTo(data, sec, sim);
        }

        private void PageDown(string[] data)
        {
            Mnt.PageDown(data);
        }
    }
}
