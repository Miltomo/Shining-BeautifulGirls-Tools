using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
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
        public int SkTurns { get; private set; } = 0;
        private int SkPoints { get; set; } = 0;

        private string? theLearnName;
        private int theLearnCost = 0;

        private bool IsNecessarySkill(Zone zone)
        {
            var result = ExtractInfo(zone);

            for (int i = 0; i < SkList.Count; i++)
            {
                var skill = SkList[i];
                if (result.Contains(skill))
                {
                    theLearnCost = (int)result.NumericLines.FirstOrDefault();
                    theLearnName = skill;
                    return true;
                }
            }
            return false;
        }

        private bool IsHadSkill(string? background = null)
        {
            return Mnt.FastCheck(Symbol.已获得, background, 0.8);
        }

        private void SkillScroll(int distance = 430)
        {
            Mnt.Scroll([360, 1000, distance]);
            Pause(1000);
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
