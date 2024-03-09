using ComputerVision;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        public SkillCore SkillManager { get; init; }
        private int SkPoints { get; set; } = 0;

        private void LearnSkill(SkillCore.CheckInfo info)
        {
            bool Aw = false;

            // 处理
            if (info.IsTarget)
            {
                var cost = info.Cost;
                var name = info.Name;
                var zone = info.Zone;
                var x = info.ButtonX;
                var y = info.ButtonY;

                // 加速：若不够学习则跳过
                if (cost > SkPoints)
                    return;

                // 若已习得，加入完成队列
                if (IsHadSkill(CropScreen(zone)))
                {
                    CurrentLearns.Add(name);
                    return;
                }
                // 若未习得，点击学习按钮
                else
                {
                    Mnt.Click(x, y, pauseTime: 300);
                    Aw = true;
                }

                // 二次学习检测
                if (!IsHadSkill(CropScreen(zone)) && (SkPoints >= 2 * cost))
                    Mnt.Click(x, y, pauseTime: 200);

                Log($"学习技能「{name}」");
                if (IsHadSkill(CropScreen(zone)))
                    CurrentLearns.Add(name);
            }

            // 加速：只在变化时更新技能点
            if (Aw) RefreshSkillPoint();
        }

        private bool IsHadSkill(string? background = null)
        {
            return Mnt.FastCheck(Symbol.已获得, background, 0.8);
        }

        private void SkillScroll(int distance = 435)
        {
            Mnt.Scroll([360, 1000, distance]);
            Pause(1000);
        }

        private void RefreshSkillPoint() => SkPoints = ExtractValue(Zone.技能点2);

        private IOCRResult ReadLastSkill() => ExtractInfo(Zone.技3);
    }
}
