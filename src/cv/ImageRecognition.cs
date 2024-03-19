using MHTools;
using OpenCvSharp;
using OpenCvSharp.Features2D;

namespace ComputerVision
{
    public static class ImageRecognition
    {
        /// <summary>
        /// 匹配目标图像
        /// </summary>
        /// <param name="objPath"></param>
        /// <param name="bgPath"></param>
        /// <param name="Loc">位置</param>
        /// <returns>相似度</returns>
        /// <exception cref="Exception"></exception>
        public static double MatchImage(string objPath, string bgPath, out Point Loc)
        {
            // 读取目标图像和背景图像
            using Mat targetImage = Cv2.ImRead(objPath);
            using Mat backgroundImage = Cv2.ImRead(bgPath);

            if (targetImage.Empty() || backgroundImage.Empty())
                throw new ResourcesNotFindException();

            // 使用模板匹配进行图像匹配
            Mat resultImage = new();
            Cv2.MatchTemplate(backgroundImage, targetImage, resultImage, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(resultImage, out _, out double maxVal, out _, out Point maxLoc);

            Loc = maxLoc;
            return maxVal;
        }


        /// <summary>
        /// 以给定相似度匹配目标图像，并找出所有位置
        /// </summary>
        /// <param name="objPath"></param>
        /// <param name="bgPath"></param>
        /// <param name="sim"></param>
        /// <returns></returns>
        public static List<Point> GetLocationS(string objPath, string bgPath, double sim)
        {
            // 读取目标图像和背景图像
            Mat targetImage = new(objPath);
            Mat backgroundImage = new(bgPath);

            List<Point> objectLocations = new();
            while (true)
            {
                if (backgroundImage.Width < targetImage.Width)
                    break;

                // 使用模板匹配方法（TM_CCOEFF_NORMED）来查找目标图像在背景图像中的位置
                Mat result = new();
                Cv2.MatchTemplate(backgroundImage, targetImage, result, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out Point maxLoc);

                if (maxVal > sim)
                {
                    objectLocations.Add(maxLoc);

                    var X = maxLoc.X + targetImage.Width;

                    // 裁剪图像继续匹配
                    backgroundImage = new(backgroundImage, new Rect(
                        X,
                        0,
                        backgroundImage.Width - X,
                        backgroundImage.Height
                        ));

                    continue;
                }
                break;
            }

            return objectLocations;
        }


        public static List<Mat> SegmentObjects(string imagePath)
        {
            Mat image = new(imagePath, ImreadModes.Color);

            // 使用Canny边缘检测来查找图像中的边缘
            Mat edges = new();
            Cv2.Canny(Gray(image), edges, 100, 200); // 调整阈值以适应不同图像

            var path = @"Z:\C#练习\Shining BeautifulGirls\resources\";
            edges.SaveImage(path + "canny.png");

            // 寻找轮廓
            Cv2.FindContours(edges, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            // 创建一个列表来存储分割后的图像
            List<Mat> segmentedObjects = new();

            double lastX = .0;

            // 遍历轮廓并提取每个对象
            for (int i = 0; i < contours.Length; i++)
            {
                var contour = contours[i];
                if (contour.Length < 11)
                    continue;

                /* double sumX = .0;
                 for (int j = 0; j < contour.Length; j++)
                     sumX += contour[j].X;
                 double centerX = sumX / contour.Length;
                 if (centerX - lastX < 3)
                     continue;
                 lastX = centerX;*/


                // 创建一个具有相同大小的黑色背景
                Mat mask = Mat.Zeros(image.Size(), MatType.CV_8UC1);

                // 使用白色填充轮廓区域
                Cv2.FillPoly(mask, new List<Point[]>() { contour }, Scalar.White);

                // 将原始图像与掩码相乘以提取对象
                Mat segmentedObject = new();
                Cv2.BitwiseAnd(image, image, segmentedObject, mask);

                segmentedObject.SaveImage(path + "sg1.png");

                // 添加分割后的对象到列表中
                segmentedObjects.Add(segmentedObject);
            }

            return segmentedObjects;
        }

        public static Mat SegmentImageWithThreshold(string imagePath, int thresholdValue)
        {
            Mat image = new(imagePath, ImreadModes.Grayscale);



            // 创建一个二值图像，将灰度值大于阈值的像素设置为白色，小于等于阈值的像素设置为黑色
            Mat thresholded = new();
            Cv2.Threshold(image, thresholded, thresholdValue, 255, ThresholdTypes.Binary);

            return thresholded;
        }


        public static MatchResult FeatureMatch(string targetImagePath, string backgroundImagePath)
        {
            // 读取目标图像和背景图像
            Mat targetImage = new(targetImagePath);
            Mat backgroundImage = new(backgroundImagePath);

            if (targetImage.Empty() || backgroundImage.Empty())
                throw new Exception();

            // 初始化SIFT
            SIFT sift = SIFT.Create();

            // 检测和计算目标图像的关键点和描述符
            Mat descriptorsTarget = new();
            sift.DetectAndCompute(targetImage, null, out KeyPoint[] keyPointsTarget, descriptorsTarget);

            // 检测和计算背景图像的关键点和描述符
            Mat descriptorsBackground = new();
            sift.DetectAndCompute(backgroundImage, null, out KeyPoint[] keyPointsBackground, descriptorsBackground);

            // 使用FLANN进行特征匹配
            FlannBasedMatcher matcher = new();
            DMatch[] matches;
            try
            {
                matches = matcher.Match(descriptorsTarget, descriptorsBackground);
            }
            catch (Exception)
            {
                return new MatchResult();
            }

            return new MatchResult(matches);
        }

        public static bool FeatureJudge(string targetImagePath, string backgroundImagePath)
        {
            MatchResult result = FeatureMatch(targetImagePath, backgroundImagePath);
            int half = result.Half;

            if (
                result.ZeroCount > 1 ||
                result.Confidence > 0.8 ||
                (result.SmallCount > half && result.Confidence > 0.7)
                )
                return true;
            return false;
        }


        public class MatchResult
        {
            public DMatch[] Matches { get; init; }
            public int Count => Matches.Length;
            public int Half { get; init; }
            public int ZeroCount { get; init; }
            public int SmallCount { get; init; }
            public double Confidence { get; init; }

            public MatchResult(DMatch[] matches)
            {
                Matches = matches;
                double avg = matches.Average(m => m.Distance);
                double max = matches.Max(m => m.Distance);

                Half = (int)Math.Floor(Count / 2.0);
                ZeroCount = matches.Where(x => x.Distance < 10e-5).Count();
                SmallCount = matches.Where(x => x.Distance < avg / 10).Count();
                Confidence = 1.0 - avg / max;
            }
            public MatchResult()
            {
                Matches = [];
                Half = int.MaxValue;
                ZeroCount = 0;
                SmallCount = 0;
                Confidence = 0.0;
            }
        }


        public static Mat ApplyRectangleMask(Mat backgroundImage, Rect rectangle)
        {
            // 创建一个全黑的掩码图像，与背景图像大小相同
            Mat mask = Mat.Zeros(backgroundImage.Size(), MatType.CV_8UC1);

            // 在掩码上绘制白色矩形，矩形的位置与背景图像上的位置相同
            mask[rectangle] = Mat.Ones(rectangle.Size, MatType.CV_8UC1);

            // 创建输出图像，与背景图像大小相同
            Mat result = new(backgroundImage.Size(), MatType.CV_8UC3);

            // 将背景图像和掩码应用于输出图像，只保留矩形位置的内容
            backgroundImage.CopyTo(result, mask);


            return result;
        }


        public static double AvgBrightness(string imagePath)
        {
            // 读取图像
            Mat image = new(imagePath, ImreadModes.Color);

            // 转换图像为灰度图
            Mat grayImage = new();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            // 计算图像的平均亮度
            Scalar averageBrightness = Cv2.Mean(grayImage);

            // 返回平均亮度值
            return averageBrightness.Val0;
        }


        /// <summary>
        /// 获取图像所有的轮廓信息
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static Point[][] GetContours(Mat edgeImage)
        {
            for (int row = 0; row < edgeImage.Height; row++)
                edgeImage.Set(row, 0, new Vec3b(0, 0, 0));

            // 寻找轮廓
            Cv2.FindContours(edgeImage, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            return contours;
        }

        public static Mat CropImage(string inputImage, Rectangle zone)
        {
            try
            {
                return new(new(inputImage), new Rect(
                zone.minX,
                zone.minY,
                zone.width,
                zone.height
                ));
            }
            catch (Exception)
            {
                throw new ResourcesNotFindException();
            }
        }


        /// <summary>
        /// 转换为灰度图像
        /// </summary>
        /// <param name="inputImage"></param>
        /// <returns></returns>
        public static Mat Gray(Mat inputImage)
        {
            Mat @out = new();
            Cv2.CvtColor(inputImage, @out, ColorConversionCodes.BGR2GRAY);
            return @out;
        }

        /// <summary>
        /// 使用拉普拉斯算子进行锐化处理
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="ksize"></param>
        /// <param name="scale"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Mat Laplacian(Mat inputImage,
            int ksize = 1,
            double scale = 1,
            double delta = 0
            )
        {
            Mat @out = new();
            Cv2.Laplacian(inputImage, @out, MatType.CV_8UC1,
                ksize: ksize, scale: scale, delta: delta,
                borderType: BorderTypes.Default);
            return @out;
        }

        public static Mat Binary(Mat inputImage)
        {
            // 创建一个二值图像，将灰度值大于阈值的像素设置为白色，小于等于阈值的像素设置为黑色
            Mat thresholded = new();
            Cv2.Threshold(inputImage, thresholded, 128, 255, ThresholdTypes.Binary);

            return thresholded;
        }

        public struct Rectangle(int x1, int x2, int y1, int y2)
        {
            public int x1 = x1;
            public int x2 = x2;
            public int y1 = y1;
            public int y2 = y2;

            public int width = Math.Abs(x2 - x1);
            public int height = Math.Abs(y2 - y1);
            public int minX = Math.Min(x1, x2);
            public int maxX = Math.Max(x1, x2);
            public int minY = Math.Min(y1, y2);
            public int maxY = Math.Max(y1, y2);
        }



        public static Mat ConnectLines(Mat edgeImage)
        {
            Mat connectedImage = edgeImage.Clone();

            int limit = (int)(edgeImage.Height * 0.8);
            List<int> blackList = new();

            for (int col = 0; col < edgeImage.Width; col++)
            {
                int blackPixelCount = 0;

                for (int row = 0; row < edgeImage.Height; row++)
                {
                    Scalar pixelValue = (Scalar)edgeImage.Get<Vec3b>(row, col);

                    if (pixelValue[0] < 10)
                        blackPixelCount++;
                }

                blackList.Add(blackPixelCount);

                // 如果黑色块的高度超过阈值，将整列涂黑
                if (blackPixelCount > limit)
                {
                    for (int row = 0; row < edgeImage.Height; row++)
                        connectedImage.Set(row, col, new Vec3b(0, 0, 0));
                }
            }

            return connectedImage;
        }
    }
}
