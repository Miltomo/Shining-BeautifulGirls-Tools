using ComputerVision;
using System.IO;
using System.Text.RegularExpressions;
using static ComputerVision.ImageRecognition;

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

        public void UpdateHP()
        {
            Vitality = GetHP();
            Vitality = Vitality > 95 && _lastHP < 50 && _lastAction != "休息" ? 0 : Vitality;
            _lastHP = Vitality;
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

        public void UpdateMood()
        {
            Mood = GetMood();
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

        private int ExtractValue(Zone zone)
        {
            return (int)ExtractInfo(zone).NumericLines.FirstOrDefault();
        }

        PaddleOCR.Result ExtractInfo(Enum zone) => Mnt.ExtractZone(zone);

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

        private int GetFans(PaddleOCR.Result result)
        {
            var fs = result.Like(RaceFansRegex()).FirstOrDefault();
            if (fs != null && int.TryParse(RaceFansTransRegex().Replace(fs, ""), out var fans))
                return fans;
            return 0;
        }

        private RaceInfo[] GetRaceInfos()
        {
            List<RaceInfo> races = [];

            foreach (var zones in RaceZoneTable)
            {
                RaceInfo info = new();
                var r = ExtractInfo(zones[0]);
                info.Ground = r.FirstIn(AllGround) ?? "";
                info.Distance = r.FirstIn(AllDistance) ?? "";

                // 如果类型适合
                if (IsSuitableRace(info))
                {
                    // 获取比赛名称
                    info.Name = ExtractInfo(zones[1]).Text;

                    // 获取粉丝数
                    info.Fans = GetFans(ExtractInfo(zones[2]));
                }

                races.Add(info);
            }

            return [.. races];
        }

        private string GetAdaptability(Zone zone)
        {
            return RankClassification.Predict(new()
            {
                ImageSource = File.ReadAllBytes(CropScreen(zone)),
            }).PredictedLabel;
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


        //==============================================================

        [GeneratedRegex(@"^\+?\d+.+人$")]
        private static partial Regex RaceFansRegex();
        [GeneratedRegex("[\\+,\\.人]")]
        private static partial Regex RaceFansTransRegex();

        [GeneratedRegex("\\s")]
        private static partial Regex 空白Regex();
    }
}
