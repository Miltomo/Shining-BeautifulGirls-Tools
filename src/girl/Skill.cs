using System.Collections.Generic;
using System.IO;
using static ComputerVision.ImageRecognition;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        public static string SkillDir { get; set; } = @"./skill/";

        public List<List<string>> PrioritySkillList
        {
            get =>
                UserConfig?.PrioritySkillList ??
                [
                    ["弯道恢复", "圆弧艺术家", "直线恢复", "喘口气"],//["弯道恢复", "圆弧艺术家", "营养补给", "大胃王"],
                    ["弯道能手", "弧线大师", "领跑直线", "领跑弯道"],//["弯道能手", "弧线大师"],
                    ["疾步", "逃脱术", "领头心得", "直线能手", "领跑诀窍"],//["轻盈步伐", "回避失速优俊少女", "良场地", "最后冲刺", "直线能手", "直线加速", "准备突围"],
                    ["决不让出领头的景色", "绚丽演习", "翘尾巴", "准备压制", "逃亡者", "直线加速", "良场地"],
                ];
        }
        [ToSave]
        private List<string> SkList { get; set; } = [];
        [ToSave]
        private int SkIndex { get; set; } = 0;
        [ToSave]
        private int SkTurns { get; set; } = 0;
        private int SkPoints { get; set; } = 0;

        private string? LearnName;
        private bool IsNecessarySkill(string skillBackground)
        {
            for (int i = 0; i < SkList.Count; i++)
            {
                if (MatchImage(Path.Combine(SkillDir, $"{SkList[i]}.png"), skillBackground, out _) > 0.95)
                {
                    LearnName = SkList[i];
                    return true;
                }
            }
            return false;
        }

        private void SkillScroll(int distance)
        {
            Mnt.Scroll([360, 1000, distance]);
            Mnt.Pause(1000);
            Mnt.Refresh();
        }

        private void UpdateLearningList()
        {
            bool canUpdate = false;
            if (SkList.Count == 0)
                canUpdate = true;
            else
                switch (SkIndex)
                {
                    case 1: if (SkTurns > 2) canUpdate = true; break;
                    case 2: if (SkTurns > 5) canUpdate = true; break;
                }
            if (canUpdate && SkIndex < PrioritySkillList.Count)
                SkList.AddRange(PrioritySkillList[SkIndex++]);
        }
    }
}
