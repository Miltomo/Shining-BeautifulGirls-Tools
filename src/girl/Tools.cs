using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static ComputerVision.ImageRecognition;
using static Shining_BeautifulGirls.World.NP;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        private string MaskScreen(object zone, string name = "mask")
        {
            return Mnt.MaskScreen(zone, name);
        }
        private string CropScreen(object zone, string name = "zone")
        {
            return Mnt.CropScreen(zone, name);
        }

        private int GetHP()
        {
            Rectangle 实值 = new(0, 0, 0, 0);
            int 边界 = int.MaxValue;
            var target = CropScreen(Zone.体力);
            var contours = GetContours(
                ConnectLines(Binary(Laplacian(
                        Gray(new(target)),
                        delta: 350,
                        scale: 2,
                        ksize: 5
                        )))
                );

            // 遍历轮廓信息
            for (int i = 0; i < contours.Length; i++)
            {
                var contour = contours[i];

                // 去除小值
                if (contour.Length < 11)
                    continue;
                int
                    minX = int.MaxValue,
                    minY = int.MaxValue;
                int
                    maxX = int.MinValue,
                    maxY = int.MinValue;

                // 矩形化
                for (int j = 0; j < contour.Length; j++)
                {
                    var x = contour[j].X;
                    var y = contour[j].Y;
                    minX = x > minX ? minX : x;
                    minY = y > minY ? minY : y;
                    maxX = x > maxX ? x : maxX;
                    maxY = y > maxY ? y : maxY;
                }

                //Rectangle rt = new(minX, maxX, minY, maxY);
                /*var target = World.CacheDir + "fg.png";
                CropImage(@out, rt)
                    .SaveImage(target);
                continue;*/

                if (minX > 1)
                {
                    if (minX < 10)
                        实值 = new(minX, maxX, minY, maxY);
                    else
                        边界 = maxX < 边界 ? maxX : 边界;
                }
            }//for

            return (int)Math.Round((实值.maxX - 实值.minX) / (double)(边界 - 实值.minX) * 100);
        }

        private int GetMood()
        {
            var saveFileName = CropScreen(Zone.心情);

            string mood = MoodClassification.Predict(new()
            {
                ImageSource = File.ReadAllBytes(saveFileName),
            }).PredictedLabel;

            return mood switch
            {
                "极佳" => 5,
                "上佳" => 4,
                "普通" => 3,
                "不佳" => 2,
                "极差" => 1,
                _ => 0,
            };
        }

        [Obsolete]
        private int GetIncreaseValue(string property)
        {
            var zone = property + "增加";
            var sz = CropScreen(zone);
            var fg = Mnt.MakeUniqueCacheFile("fg");
            var contours = GetContours(
                Laplacian(
                Gray(new(sz)),
                delta: 100,
                scale: 0.2,
                ksize: 5)
                );

            Dictionary<Rectangle, string> map = [];

            // 遍历轮廓信息
            for (int i = 0; i < contours.Length; i++)
            {
                var contour = contours[i];
                // 去除小值
                if (contour.Length < 11)
                    continue;

                int
                    minX = int.MaxValue,
                    minY = int.MaxValue;
                int
                    maxX = int.MinValue,
                    maxY = int.MinValue;

                // 矩形化
                for (int j = 0; j < contour.Length; j++)
                {
                    var x = contour[j].X;
                    var y = contour[j].Y;
                    minX = x > minX ? minX : x;
                    minY = y > minY ? minY : y;
                    maxX = x > maxX ? x : maxX;
                    maxY = y > maxY ? y : maxY;
                }
                Rectangle rt = new(minX, maxX, minY, maxY);

                // 检测矩形是否合格
                if (rt.height < 30 || rt.height > 40)
                    continue;
                if (rt.width < 15 || rt.width > 30)
                    continue;


                // 跳过重合位置(X分割)
                var keys = map.Keys.ToArray();
                for (int k = 0; k < keys.Length; k++)
                {
                    var dq = keys[k];

                    if (rt.minX > dq.maxX)
                        continue;

                    if (rt.maxX < dq.minX)
                        continue;

                    goto NEXT;
                }

                CropImage(sz, rt)
                    .SaveImage(fg);

                // 利用神经网络分类图片
                var result = DigitalRecognition.Predict(new()
                {
                    ImageSource = File.ReadAllBytes(fg),
                });

                var value = result.PredictedLabel;
                var score = result.Score.ToList().Max();

                if (value == "无" || score < 0.5)
                    continue;

                map.Add(rt, value);
            NEXT:;
            }//for

            var locS = map.Keys.ToArray().ToList();
            string number = "0";
            locS.Sort((a, b) =>
            {
                if (a.minX > b.minX)
                    return 1;
                else if (a.minX < b.minX)
                    return -1;
                else
                    return 0;
            });
            locS.ForEach(x => number += map[x]);

            return int.Parse(number);
        }
        [Obsolete]
        private int GetPropertyValue(string property)
        {
            var zone = property;
            var sz = CropScreen(zone);
            var fg = Mnt.MakeUniqueCacheFile("fg");
            var contours = GetContours(
                Laplacian(
                Gray(new(sz)),
                delta: 100,
                scale: 0.2,//20
                ksize: 5) //3
                );

            Dictionary<Rectangle, string> map = [];

            // 遍历轮廓信息
            for (int i = 0; i < contours.Length; i++)
            {
                var contour = contours[i];
                // 去除小值
                if (contour.Length < 11)
                    continue;

                int
                    minX = int.MaxValue,
                    minY = int.MaxValue;
                int
                    maxX = int.MinValue,
                    maxY = int.MinValue;

                // 矩形化
                for (int j = 0; j < contour.Length; j++)
                {
                    var x = contour[j].X;
                    var y = contour[j].Y;
                    minX = x > minX ? minX : x;
                    minY = y > minY ? minY : y;
                    maxX = x > maxX ? x : maxX;
                    maxY = y > maxY ? y : maxY;
                }
                Rectangle rt = new(minX, maxX, minY, maxY);

                // 检测矩形是否合格
                if (rt.height < 19 || rt.height > 21)
                    continue;
                if (rt.width < 7 || rt.width > 19)
                    continue;


                // 跳过重合位置(X分割)
                var keys = map.Keys.ToArray();
                for (int k = 0; k < keys.Length; k++)
                {
                    var dq = keys[k];

                    if (rt.minX > dq.maxX)
                        continue;

                    if (rt.maxX < dq.minX)
                        continue;

                    goto NEXT;
                }

                CropImage(sz, rt)
                .SaveImage(fg);

                // 利用神经网络分类图片
                var result = PropertyValueRecognition.Predict(new()
                {
                    ImageSource = File.ReadAllBytes(fg),
                });

                var value = result.PredictedLabel;
                var score = result.Score.ToList().Max();

                if (value == "无" || score < 0.5)
                    continue;

                map.Add(rt, value);
            NEXT:;
            }//for

            var locS = map.Keys.ToArray().ToList();
            string number = "0";
            locS.Sort((a, b) =>
            {
                if (a.minX > b.minX)
                    return 1;
                else if (a.minX < b.minX)
                    return -1;
                else
                    return 0;
            });
            locS.ForEach(x => number += map[x]);

            return int.Parse(number);
        }

        private int ExtractValue(Zone zone)
        {
            return (int)Mnt.ExtractZoneNumberS(zone).FirstOrDefault();
        }

        private bool IsZoneContains(Zone zone, object target)
        {
            return Mnt.ExtractZoneAndContains(zone, target.ToString()!);
        }

        private HeadInfo GetHeadInfo()
        {
            HeadInfo head = new();
            for (int i = 1; i < 6; i++)
            {
                var result = HeadDetection.Predict(new()
                {
                    ImageSource = File.ReadAllBytes(CropScreen($"协助{i}")),
                }).PredictedLabel;
                if (result == "有")
                {
                    head.Count += 1;
                    continue;
                }
                break;
            }
            return head;
        }

        private static string GetTodayRecordDir()
        {
            var now = DateTime.Now;
            string year = $"{now:yyyy}";
            string month = $"{now:MMM}";
            string today = $"{now:yyyy-MM-dd}";
            string dir = Path.Combine(RecordDir, year, month, today);
            Directory.CreateDirectory(dir);
            return dir;
        }

        public static int FailPredict(int hp)
        {
            //系数(t0 -> tn)
            double[] t = [
                1.79597106e+02,  //125.073753,
                -7.02621230,     //-4.09449558,
                8.91701854e-02,  //0.0398538719,
                -3.68706166e-04  //-0.000110971931
                ];
            double fail = .0;
            for (int i = 0; i < t.Length; i++)
                fail += t[i] * Math.Pow(hp, i);
            return (int)Math.Round(fail);
        }

        [GeneratedRegex("\\s")]
        private static partial Regex 空白Regex();
    }
}
