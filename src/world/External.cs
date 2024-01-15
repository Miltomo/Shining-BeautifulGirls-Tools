using MHTools;
using OpenCvSharp;
using System;
using System.Diagnostics;
using System.IO;
using static ComputerVision.ImageRecognition;
using static ComputerVision.TextDetection;

namespace Shining_BeautifulGirls
{
    //TODO 更换检测方法 => 需要重构Symbol图像
    //TODO 若要更换，必须得会文字识别才行，否则还是没用
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
        public bool FastCheck(string targetPath, string? bgPath = default, double sim = 0.9)
        {
            return
                MatchImage(targetPath,
                bgPath ?? Screen,
                out _)
                >
                sim;
        }


        public bool CheckSymbol(string name, string? background = default, double delta = 0.9)
        {
            return Check(Path.Combine(SymbolDir, $"{name}.png"), background, delta);
        }

        /// <summary>
        /// (不刷新) 直接开始匹配目标象征物
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        public bool FastSymbol(string symbol, double sim = 0.9)
        {
            return FastCheck(
                FileManagerHelper.SetDir(SymbolDir).Find(symbol)!,
                sim: sim);
        }

        //========================
        //========屏幕裁剪========
        //========================

        public Mat CropScreen(string zone)
        {
            return CropImage(Screen, GetRectangle(zone));
        }

        public string CropScreen(string zone, string saveFileName)
        {
            string path = MakeUniqueCacheFile(saveFileName);
            CropScreen(zone).SaveImage(path);
            return path;
        }

        public Mat MaskScreen(string zone)
        {
            var rt = GetRectangle(zone);
            var bg = new Mat(Screen);
            return ApplyRectangleMask(bg, new(0, rt.minY, bg.Width, rt.height));
        }

        public string MaskScreen(string zone, string saveFileName)
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

        public Rectangle GetRectangle(string name)
        {
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
        public string ExtractZoneText(string zone)
        {
            return ExtractText(CropScreen(zone, "extract"));
        }

        public string ExtractZoneInteger(string zone)
        {
            var target = MakeUniqueCacheFile("extract");
            //得到区域图像(灰度)
            Gray(CropScreen(zone))
                .SaveImage(target);

            return ExtractDigits(target);
        }
    }
}
