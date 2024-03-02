namespace Shining_BeautifulGirls
{
    //TODO 连续参赛时的处理
    //TODO 增加"训练-赛事"权衡决断定义
    //TODO 着手设计自选比赛功能
    partial class ShiningGirl
    {
        private static PlanEnum[] TrainingItems { get; } =
            [PlanEnum.速度, PlanEnum.耐力, PlanEnum.力量, PlanEnum.毅力, PlanEnum.智力];
        private static Button[] TrainingButtons { get; } =
            [Button.速度, Button.耐力, Button.力量, Button.毅力, Button.智力];
        private Algorithm Core { get; set; }
        public bool TryTrain()
        {
            if (GotoTrainPage())
            {
                Core = new PrimaryLogic(this);
                Subject巡视();
                StartPlan();
                return true;
            }

            return false;
        }

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

        private void StartPlan()
        {
            Log(Core.Print());
            var t = Core.PlanToDo();
            var plan = t.Plan;

        计划匹配:
            if (TrainingItems.Contains(plan))
            {
                GotoTrainPage();

                Log($"选择「{plan}」 {(t.Fail > 9 ? $"(训练失败率：{t.Fail}%)" : "")}");
                var bt = TrainingButtons[Array.IndexOf(TrainingItems, plan)];
                while (FastCheck(Symbol.返回, 0.7))
                    Click(bt);

            }
            else
            {
                switch (plan)
                {
                    case PlanEnum.休息:
                        Log($"训练失败率高，选择休息");
                        System__relex__();
                        break;

                    case PlanEnum.外出:
                        Log($"放弃训练，恢复心情");
                        System__out__();
                        break;

                    case PlanEnum.比赛:
                        // 夏天不比赛
                        if (InSummer)
                        {
                            plan = Core.WhenNoRace().Plan;
                            goto 计划匹配;
                        }

                        Log($"训练增益值过低，准备参加日常比赛......");
                        if (Core.FindRace())
                        {
                            Log($"已找到合适比赛：");
                            Log(TargetRace);
                            比赛处理(比赛过程Enum.进入);
                            break;
                        }
                        Log($"未找到合适比赛");
                        plan = Core.WhenNoRace().Plan;
                        GotoMainPage();

                        goto 计划匹配;

                    default:
                        System__relex__();
                        break;
                }
            }

            _lastAction = plan.ToString();
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
                Read训练信息(TrainingItems[index]);
                // 判断是否可以跳过剩下的
                if (Core.Skip())
                    return;
                var next = (index + 1) % TrainingItems.Length;
                if (next == start)
                    break;
                Click(TrainingButtons[next], 50);
                index = next;
            } while (true);
        }

        private void Read训练信息(PlanEnum subject)
        {
            var 等效体力 = InSummer ? Vitality - 2 : Vitality;
            if (subject == PlanEnum.智力)
                等效体力 += 10;

            // 记录训练信息
            PlanInfo train = new()
            {
                Plan = subject,
                HeadInfo = GetHeadInfo(),
                Fail = FailPredict(等效体力)
            };

            // 定义读值次数：通过多次判断纠正错误
            int count = 2;
            List<int[]> vList = [];
            while (true)
            {
                count -= 1;
                vList.Add(Once增益读值(subject));

                if (count < 1)
                    break;
                Mnt.Refresh();
            }

            List<int> ups = [];
            for (int i = 0; i < TrainingItems.Length; i++)
            {
                int real;
                var table = vList.Select(x => x[i]);
                var max = table.Max();
                var min = table.Min();
                if (max > 100 && min > 0)
                    real = min;
                else
                    real = max;
                ups.Add(real);
            }

            // 记录增益值信息
            train.UpS = [.. ups];

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

        private int[] Once增益读值(PlanEnum subject)
        {
            int[] V = TrainingItems.Select(x => 0).ToArray();
            int[] rCode = subject switch
            {
                PlanEnum.速度 => [0, 2],
                PlanEnum.耐力 => [1, 3],
                PlanEnum.力量 => [1, 2],
                PlanEnum.毅力 => [0, 2, 3],
                PlanEnum.智力 => [0, 4],
                _ => []
            };

            for (int i = 0; i < rCode.Length; i++)
            {
                int dq = rCode[i];
                V[dq] = ExtractValue(dq switch
                {
                    0 => Zone.速度增加,
                    1 => Zone.耐力增加,
                    2 => Zone.力量增加,
                    3 => Zone.毅力增加,
                    4 => Zone.智力增加,
                    _ => throw new KeyNotFoundException()
                });
            }

            return V;
        }


        private class PlanInfo()
        {
            public PlanEnum Plan { get; set; }
            public int[] UpS { get; set; }
            public HeadInfo HeadInfo { get; set; }
            public int Fail { get; set; }
            public double Score { get; set; } = 0d;
        }

        private enum PlanEnum
        {
            速度,
            耐力,
            力量,
            毅力,
            智力,
            外出,
            休息,
            比赛,
        }

        private struct HeadInfo()
        {
            public int Count { get; set; } = 0;
        }
    }
}
