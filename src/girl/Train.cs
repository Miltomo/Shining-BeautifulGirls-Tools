using System.Collections.Generic;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        public static List<string> SubjectS { get; } =
            ["速度", "耐力", "力量", "毅力", "智力"];
        private List<TrainInfo> T { get; } = [];
        public void TryTrain()
        {
            MoveTo("训练", ["返回", "选择末尾", "训练"]);

            Subject巡视();

            PrimaryLogic();

            StartPlan();
        }

        private void StartPlan()
        {
            Log(训练信息);

            var plan = PlanToDo.Subject;
            Log($"选择「{plan}」 {(CurrentFail > 9 ? $"(训练失败率：{CurrentFail}%)" : "")}");
            var index = SubjectS.IndexOf(plan);
            if (index > -1)
            {
                _lastAction = plan;
                while (Match("返回") > 0.7)
                    Choose(plan);
            }
            else
            {
                switch (plan)
                {
                    case "休息":
                        System__relex__();
                        break;
                    case "外出":
                        System__out__();
                        break;
                }
            }
        }

        private void Subject巡视()
        {
            Match(out OpenCvSharp.Point point, "决定");
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
            T.Clear();
            do
            {
                Record训练信息(SubjectS[index]);
                var next = (index + 1) % SubjectS.Count;
                if (next == start)
                    break;
                Choose(SubjectS[next]);
                index = next;
            } while (true);
            SortByIndex();
        }

        private void Record训练信息(string subject)
        {
            var 等效体力 = InSummer ? Vitality - 2 : Vitality;
            if (subject == "智力")
                等效体力 += 10;
            TrainInfo train = new()
            {
                Subject = subject,
                HeadInfo = GetHeadInfo(),
                Fail = FailPredict(等效体力)
            };
            for (int i = 0; i < SubjectS.Count; i += 1)
                train.UpS[i] = GetIncreaseValue(SubjectS[i]);
            T.Add(train);
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

        private class TrainInfo()
        {
            public string Subject { get; set; }
            public int[] UpS { get; } = new int[5];
            public HeadInfo HeadInfo { get; set; }
            public int Fail { get; set; }
            public double Score { get; set; } = 0d;
        }

        private struct HeadInfo()
        {
            public int Count { get; set; } = 0;
        }
    }
}
