using PaddleOCRSharp;
using System.Text.RegularExpressions;
namespace ComputerVision
{
    /// <summary>
    /// (单例|工厂)PaddleOCRSharp的进一步封装。全局管理一个引擎实例。
    /// 纯函数操作，无需在意构建过程。对识别结果也进行了封装，非常方便处理。<br/>
    /// </summary>
    public class PaddleOCR
    {
        private PaddleOCREngine Engine { get; init; }
        private string TargetImage { get; set; }

        private static PaddleOCR? _instance;
        private static PaddleOCR Instance
        {
            get
            {
                _instance ??= new PaddleOCR();
                return _instance;
            }
        }
        private static OCRParameter? Parameter { get; set; }

        /// <summary>
        /// 设置即将要识别的目标图片[文件路径]
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static PaddleOCR SetImage(string imagePath)
        {
            Instance.TargetImage = imagePath;
            return Instance;
        }

        public static void ChangeModel()
        {
        }
        public static void CustomPara()
        {
        }

        public static void UseGPU(int gpu_id = 0, int gpu_mem = 4000)
        {
            Parameter ??= new OCRParameter();
            Parameter.use_gpu = true;
            Parameter.gpu_id = gpu_id;
            Parameter.gpu_mem = gpu_mem;
        }

#pragma warning disable CS8618 // 禁止类成员的构造null检查
        private PaddleOCR()
        {
            Engine = new PaddleOCREngine(null, Parameter ?? new OCRParameter()
            {
                use_gpu = false,
                cpu_math_library_num_threads = 10
            });
        }
#pragma warning restore CS8618 // 恢复类成员的构造null检查
        public Result Extract()
        {
            return new Result(Engine.DetectText(TargetImage));
        }

        public OCRStructureResult Structure()
        {
            return Engine.DetectStructure(System.Drawing.Image.FromFile(TargetImage));
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
            /// <param name="target"></param>
            /// <returns></returns>
            public bool Contains(object target)
            {
                var text = target.ToString();
                if (string.IsNullOrWhiteSpace(text))
                    return false;
                foreach (var b in TB)
                {
                    if (b.Text.Contains(text))
                        return true;
                }
                return false;
            }

            /// <summary>
            /// 判断识别结果中是否存在某行等于目标文本
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public override bool Equals(object? target)
            {
                if (target != null)
                {
                    var text = target.ToString();
                    if (string.IsNullOrWhiteSpace(text))
                        return false;
                    foreach (var b in TB)
                    {
                        if (b.Text.Equals(text))
                            return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 找到结果中第一个在目标集合中的文本
            /// </summary>
            /// <param name="finds"></param>
            /// <returns></returns>
            public string? FirstIn(object[] finds)
            {
                foreach (var f in finds)
                {
                    var s = f.ToString();
                    if (string.IsNullOrWhiteSpace(s))
                        continue;
                    if (Equals(s))
                        return s;
                }
                return null;
            }

            /// <summary>
            /// 找到结果中所有存在于目标集合中的文本
            /// </summary>
            /// <param name="finds"></param>
            /// <returns></returns>
            public string[] FindIn(object[] finds)
            {
                List<string> result = [];

                foreach (var f in finds)
                {
                    var s = f.ToString();
                    if (string.IsNullOrWhiteSpace(s))
                        continue;
                    if (Equals(s))
                        result.Add(s);
                }

                return [.. result];
            }

            /// <summary>
            /// 获取识别结果中所有满足给定规则的文本行
            /// </summary>
            /// <param name="regex"></param>
            /// <returns></returns>
            public string[] Like(Regex regex)
            {
                return TB
                    .Select(tb => tb.Text)
                    .Where(s => regex.IsMatch(s))
                    .ToArray();
            }
        }
    }
}
