namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        /// <summary>
        /// (刷新) 点击屏幕，停顿一段时间
        /// </summary>
        /// <param name="button"></param>
        /// <param name="wait"></param>
        private void Click(Enum button, int wait = 200) => Mnt.Click(button, wait);

        /// <summary>
        /// (不刷新) 快速检查背景图是否包含目标
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        private bool FastCheck(string targetPath, double sim = 0.9) => Mnt.FastCheck(targetPath, sim: sim);

        /// <summary>
        /// (不刷新) 匹配目标图，获得目标所在位置
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="file"></param>
        /// <param name="bg"></param>
        /// <returns></returns>
        private double Match(out OpenCvSharp.Point loc, string file, string? bg = default) => Mnt.Match(out loc, file, bg);

        /// <summary>
        /// (刷新) 暂停等待一段时间，结束后刷新
        /// </summary>
        /// <param name="time"></param>
        private void Pause(int time = 200) => Mnt.Pause(time);

        /// <summary>
        /// (不刷新) 检测按钮是否处于无效状态
        /// </summary>
        /// <param name="bt"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private bool IsDimmed(Enum bt, int limit = 155) => Mnt.IsNoLight(bt, limit);


        private void MoveTo(object[] data, int sec = 1, double sim = 0.9) => Mnt.MoveTo(data, sec, sim);
        private void MoveTo(Func<bool> condition, Enum button, int sec = 1) => Mnt.MoveTo(condition, [button], sec);
        private void MoveTo(Enum zone, Enum Ptext, Enum button, int sec = 1) => Mnt.MoveTo(zone, Ptext, button, sec);

        private void PageDown(object[] data) => Mnt.PageDown(data);
        private void PageDown(Func<bool> condition, params Enum[] buttons) =>
             Mnt.PageDown(condition, [.. buttons]);
        private void PageDown(Zone zone, Enum ptext, params Enum[] buttons) =>
            PageDown(() => Mnt.ExtractZoneAndContains(zone, ptext), buttons);
    }
}
