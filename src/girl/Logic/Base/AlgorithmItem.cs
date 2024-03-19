namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        public class AlgorithmItem
        {
            /// <summary>
            /// 在UI面板上显示的名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 算法的简单介绍，显示在Tooltip里
            /// </summary>
            public string Description { get; set; }


            public 代号Enum 代号 { get; set; }

            public enum 代号Enum
            {
                PL,
                PB,
            }

            public static AlgorithmItem Build(string name, string description, 代号Enum id) =>
                new() { Name = name, Description = description, 代号 = id };
        }
        public static AlgorithmItem[] Algorithms { get; } =
            [
            AlgorithmItem.Build("基本逻辑","通用的适中性算法，兼顾数值和比赛。注重综合结果。",AlgorithmItem.代号Enum.PL),
            AlgorithmItem.Build("基本赛事特化","基于基本逻辑的历战算法，赛事权重大，会进行所有G1比赛。注重粉丝数。",AlgorithmItem.代号Enum.PB),
            ];
    }
}
