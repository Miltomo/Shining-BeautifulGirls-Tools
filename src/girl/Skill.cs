using ComputerVision;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        //TODO 重构整体技能列表访问方式 => 更加灵活、稳定、全面
        public string[][] PrioritySkillList => UserConfig?.PrioritySkillList ?? [
            [],
            [],
            [],
            []
            ];
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
            if (canUpdate && SkIndex < PrioritySkillList.Length)
                SkList.AddRange(PrioritySkillList[SkIndex++]);
        }


        private void RefreshSkillPoint()
        {
            SkPoints = ExtractValue(Zone.技能点2);
        }

        private IOCRResult ReadLastSkill()
        {
            return ExtractInfo(Zone.技3);
        }
    }
}
