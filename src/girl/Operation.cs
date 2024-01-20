namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        private void Click(World.NP.Button button, int wait = 200)
        {
            Mnt.Click(button, wait);
        }
        private void Choose(object buttonName, int wait = 200)
        {
            Mnt.Click(buttonName, wait);
            Mnt.Refresh();
        }

        /// <summary>
        /// (刷新) 检查是否包含目标图
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="bg"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        private bool Check(string targetPath, string? bg = default, double sim = 0.9)
        {
            return Match(targetPath, bg) > sim;
        }

        /// <summary>
        /// (不刷新) 直接检查背景图是否包含目标
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        private bool FastCheck(string targetPath, double sim = 0.9)
        {
            return Mnt.FastCheck(targetPath, sim: sim);
        }

        /// <summary>
        /// (刷新) 匹配目标图
        /// </summary>
        /// <param name="file"></param>
        /// <param name="bg"></param>
        /// <returns>相关度</returns>
        private double Match(string file, string? bg = default)
        {
            return Match(out _, file, bg);
        }

        private double Match(out OpenCvSharp.Point loc, string file, string? bg = default)
        {
            return Mnt.Match(out loc, file, bg);
        }

        private void MoveTo(object[] data, int sec = 1, double sim = 0.9)
        {
            Mnt.MoveTo(data, sec, sim);
        }

        private void PageDown(object[] data)
        {
            Mnt.PageDown(data);
        }
    }
}
