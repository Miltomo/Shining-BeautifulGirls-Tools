using ComputerVision;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        private void Click(object button, int wait = 200) => Mnt.Click(button, wait);

        /// <summary>
        /// (刷新) 检查是否包含目标图
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="bg"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        private bool Check(string targetPath, string? bg = default, double sim = 0.9) => Match(targetPath, bg) > sim;

        /// <summary>
        /// (不刷新) 直接检查背景图是否包含目标
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        private bool FastCheck(string targetPath, double sim = 0.9) => Mnt.FastCheck(targetPath, sim: sim);

        /// <summary>
        /// (刷新) 匹配目标图
        /// </summary>
        /// <param name="file"></param>
        /// <param name="bg"></param>
        /// <returns>相关度</returns>
        private double Match(string file, string? bg = default) => Match(out _, file, bg);
        private double Match(out OpenCvSharp.Point loc, string file, string? bg = default) => Mnt.Match(out loc, file, bg);

        /// <summary>
        /// (不刷新) 检测按钮是否处于无效状态
        /// </summary>
        /// <param name="bt"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private bool IsDimmed(Enum bt, int limit = 155) => Mnt.IsNoLight(bt, limit);


        private void MoveTo(object[] data, int sec = 1, double sim = 0.9) => Mnt.MoveTo(data, sec, sim);
        private void MoveTo(Func<bool> condition, Button button, int sec = 1) => Mnt.MoveTo(condition, [button], sec);

        private void PageDown(object[] data) => Mnt.PageDown(data);
        private void PageDown(Func<bool> condition, params Button[] buttons) =>
             Mnt.PageDown(condition, [.. buttons]);
        private void PageDown(Zone zone, Enum ptext, params Button[] buttons) =>
            PageDown(() => IsZoneContains(zone, ptext), buttons);

        /// <summary>
        /// (不刷新) 获取上部位置的解析结果
        /// </summary>
        /// <returns></returns>
        private PaddleOCR.Result Extract上部() => Mnt.ExtractZone(Zone.上部);

        /// <summary>
        /// (不刷新) 获取中部位置的解析结果
        /// </summary>
        /// <returns></returns>
        private PaddleOCR.Result Extract中部() => Mnt.ExtractZone(Zone.中部);

        /// <summary>
        /// (不刷新) 获取下部位置的解析结果
        /// </summary>
        /// <returns></returns>
        private PaddleOCR.Result Extract下部() => Mnt.ExtractZone(Zone.下部);
    }
}
