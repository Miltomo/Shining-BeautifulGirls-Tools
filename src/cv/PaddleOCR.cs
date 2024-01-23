using PaddleOCRSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComputerVision
{
    public class PaddleOCR
    {
        private PaddleOCREngine Engine { get; init; }
        private string TargetImage { get; set; }

        private static PaddleOCR? instance;

        /// <summary>
        /// 设置即将要识别的目标图片[文件路径]
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static PaddleOCR SetImage(string imagePath)
        {
            instance ??= new PaddleOCR();
            instance.TargetImage = imagePath;
            return instance;
        }
#pragma warning disable CS8618 // 禁止类成员的构造null检查
        private PaddleOCR()
        {
            Engine = new PaddleOCREngine();
        }
#pragma warning restore CS8618 // 恢复类成员的构造null检查
        public Result Extract()
        {
            return new Result(Engine.DetectText(TargetImage));
        }

        public class Result
        {
            private OCRResult Orin { get; set; }
            private IEnumerable<TextBlock> TB { get; set; }

            /// <summary>
            /// 识别结果的全部文本
            /// </summary>
            public string Text => Orin.Text;

            /// <summary>
            /// 识别结果的分行文本
            /// </summary>
            public string[] TextAsLines => TB.Select(x => x.Text).ToArray();

            /// <summary>
            /// 识别结果的所有纯数字行
            /// </summary>
            public double[] NumericLines
            {
                get
                {
                    List<double> result = [];
                    foreach (var line in TB)
                        if (double.TryParse(line.Text, out double sz))
                            result.Add(sz);

                    return [.. result];
                }
            }
            internal Result(OCRResult result)
            {
                Orin = result;
                TB = Orin.TextBlocks;
            }

            /// <summary>
            /// 转换为原始数据
            /// </summary>
            /// <returns>PaddleOCR原始识别结果数据</returns>
            public OCRResult ToOriginalData() { return Orin; }

            /// <summary>
            /// 判断识别结果中是否存在某行包含目标文本
            /// </summary>
            /// <param name="targetText"></param>
            /// <returns></returns>
            public bool Contains(string targetText)
            {
                foreach (var b in TB)
                {
                    if (b.Text.Contains(targetText))
                        return true;
                }
                return false;
            }

            /// <summary>
            /// 获取识别结果中所有满足给定规则的文本行
            /// </summary>
            /// <param name="regex"></param>
            /// <returns></returns>
            public string[] SelectLike(Regex regex)
            {
                return TB
                    .Select(tb => tb.Text)
                    .Where(s => regex.IsMatch(s))
                    .ToArray();
            }
        }
    }
}
