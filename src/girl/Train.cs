namespace Shining_BeautifulGirls
{
    //TODO 着手设计自选比赛功能
    partial class ShiningGirl
    {
        private static ZButton[] TrainingButtons { get; } =
            [ZButton.速度, ZButton.耐力, ZButton.力量, ZButton.毅力, ZButton.智力];
        private Algorithm Core { get; init; }

        private bool GotoTrainPage()
        {
            if (AtTrainPage())
                return true;

            if (!GotoMainPage())
                return false;

            Click(Button.训练);

            return MC.Builder
                .AddTarget(AtTrainPage)
                .StartAsWaitTo(Mnt, 200);
        }

        /// <summary>
        /// 确保从任何位置能前往训练场地并获取训练信息
        /// </summary>
        /// <returns>true，成功获取训练信息；false，无法移至训练界面</returns>
        private bool 获取训练信息()
        {
            if (GotoTrainPage())
            {
                if (Core.IsNeedInfo)
                {
                    Subject巡视();
                    Log(Core.Print());
                }
                return true;
            }

            return false;
        }
        private void StartPlan(Plan t)
        {
            var pName = t.Name;
            if (Plan.IsTrainningPlan(t))
            {
                GotoTrainPage();

                Log($"选择「{pName}」 {(t.Fail > 9 ? $"(训练失败率：{t.Fail}%)" : "")}");
                var bt = TrainingButtons[Plan.GetIndexOfTrainning(t)];
                while (FastCheck(Symbol.返回, 0.7))
                    Click(bt);

                LastAction = ActionEnum.训练;
            }
            else
            {
                switch (pName)
                {
                    case PlanName.休息:
                        Log($"训练失败率高，选择休息");
                        System__relex__();
                        break;

                    case PlanName.外出:
                        Log($"放弃训练，恢复心情");
                        System__out__();
                        break;

                    case PlanName.比赛:
                        Log($"训练增益值过低，准备参加日常比赛......");
                        if (!DailyRaceProcess(Core.TryRace))
                            StartPlan(Core.WhenNoRace());
                        break;

                    default:
                        System__relex__();
                        break;
                }
            }
        }

        private void Subject巡视()
        {
            Match(out OpenCvSharp.Point point, Symbol.决定);
            var index = point.X switch
            {
                < 100 => 0,
                < 220 => 1,
                < 350 => 2,
                < 480 => 3,
                < 600 => 4,
                _ => -1
            };
            var start = index;
            do
            {
                Read训练信息(Plan.GetTrainningItemByIndex(index));

                var next = (index + 1) % Plan.TrainningItemsCount;
                var zbt = TrainingButtons[next];

                // 判断是否可以跳过剩下的
                if (next == start || IsDimmed(zbt, 125) || Core.Skip())
                    break;

                Click(TrainingButtons[next], 50);
                index = next;
            } while (true);
        }

        private void Read训练信息(PlanName subject)
        {
            var 等效体力 = InSummer ? Vitality - 2 : Vitality;
            if (subject == PlanName.智力)
                等效体力 += 10;

            // 记录训练信息
            Plan train = new()
            {
                Name = subject,
                HeadInfo = GetHeadInfo(),
                Fail = FailPredict(等效体力)
            };

            // 记录增益值信息
            if (Core.IsReadUpValue)
            {
                train.UpS = GetUpsInfoByOCR();
            }

            if (UserConfig?.SaveHighLight ?? false)
            {
                // 判断并记录高光时刻
                foreach (var v in train.UpS)
                {
                    if (v > 49 || (v > 29 && train.HeadInfo.Count == 5))
                    {
                        Mnt.SaveScreen(HighLightDir);
                        break;
                    }
                }
            }

            // 载入算法
            Core.Add(train);
        }

        private struct HeadInfo()
        {
            public int Count { get; set; } = 0;
        }
    }
}
