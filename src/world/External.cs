using ComputerVision;
using MHTools;
using OpenCvSharp;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using static ComputerVision.ImageRecognition;

namespace Shining_BeautifulGirls
{
    //TODO 更换检测方法 => 重构Symbol图像
    partial class World
    {
        //========================
        //========图像匹配========
        //========================
        /// <summary>
        /// 图像匹配函数的最原始定义：先刷新，再用"模板匹配"方法进行匹配。
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="file"></param>
        /// <param name="bg"></param>
        /// <returns>相关度</returns>
        public double Match(out Point loc, string file, string? bg = default)
        {
            Refresh();
            return MatchImage(
                file,
                bg == default ? Screen : bg,
                out loc);
        }

        public double Match(string file, string? bg = default)
        {
            return Match(out _, file, bg);
        }

        /*public bool Feature(string file, string? background)
        {
            return FeatureJudge(file, background ?? Screen);
        }*/

        /// <summary>
        /// (刷新) 检测目标物是否存在: 使用模板匹配
        /// </summary>
        /// <param name="file"></param>
        /// <param name="background"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        private bool Check(string file, string? background = default, double delta = 0.9)
        {
            if (Match(file, background) > delta)
            {
                Debug.WriteLine("包含" + Path.GetFileName(file));
                return true;
            }
            Debug.WriteLine("不包含" + Path.GetFileName(file));
            return false;
        }

        /// <summary>
        /// (不刷新) 快速检测目标物
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="bgPath"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        public bool FastCheck(object targetPath, string? bgPath = default, double sim = 0.9)
        {
            return MatchImage(
                FileManagerHelper.ToPath(targetPath),
                bgPath ?? Screen,
                out _)
                >
                sim;
        }

        /// <summary>
        /// (不刷新) 检测按钮是否处于无效状态
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public bool IsNoLight(Enum zone, int limit = 155)
        {
            return AvgBrightness(CropScreen(zone, "brightness")) < limit;
        }

        //========================
        //========屏幕裁剪========
        //========================

        public Mat CropScreen(object zone)
        {
            return CropImage(Screen, GetRectangle(zone));
        }

        public string CropScreen(object zone, string saveFileName)
        {
            string path = MakeUniqueCacheFile(saveFileName);
            CropScreen(zone).SaveImage(path);
            return path;
        }

        public Mat MaskScreen(object zone)
        {
            var rt = GetRectangle(zone);
            var bg = new Mat(Screen);
            return ApplyRectangleMask(bg, new(0, rt.minY, bg.Width, rt.height));
        }

        public string MaskScreen(object zone, string saveFileName)
        {
            string path = MakeUniqueCacheFile(saveFileName);
            MaskScreen(zone).SaveImage(path);
            return path;
        }

        /// <summary>
        /// (刷新) 保存当前屏幕截图
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        public void SaveScreen(string? saveDir = default, string? fileName = default)
        {
            Refresh();
            File.Copy(
                Screen,
                Path.Combine(saveDir ?? ScreenshotDir, $"{fileName ?? DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png"),
                true
                );
        }

        public Rectangle GetRectangle(object zone)
        {
            var name = zone.ToString();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("这不是正确的Zone");
            var rates = ZoneLocation[name];
            return new(
                (int)(rates[0] * Width),
                (int)(rates[1] * Width),
                (int)(rates[2] * Height),
                (int)(rates[3] * Height));
        }

        //========================
        //========文字识别========
        //========================

        public PaddleOCR.Result ExtractZone(Enum zone)
        {
            return PaddleOCR
                .SetImage(CropScreen(zone, "extract"))
                .Extract();
        }

        public string ExtractZoneText(Enum zone)
        {
            return PaddleOCR
                .SetImage(CropScreen(zone, "extract"))
                .Extract()
                .Text;
        }

        public string[] ExtractZoneLike(Enum zone, Regex regex)
        {
            return PaddleOCR
                .SetImage(CropScreen(zone, "extract"))
                .Extract()
                .Like(regex);
        }

        public string[] ExtractZoneLineS(Enum zone)
        {
            return PaddleOCR
                .SetImage(CropScreen(zone, "extract"))
                .Extract()
                .TextAsLines;
        }

        public double[] ExtractZoneNumberS(Enum zone)
        {
            return PaddleOCR
                .SetImage(CropScreen(zone, "extractNumber"))
                .Extract()
                .NumericLines;
        }

        public bool ExtractZoneAndContains(Enum zone, Enum ptext)
        {
            return PaddleOCR
                .SetImage(CropScreen(zone, "extractAc"))
                .Extract()
                .Contains(ptext);
        }
    }
}
