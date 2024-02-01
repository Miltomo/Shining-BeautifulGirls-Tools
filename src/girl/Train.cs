namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        public static List<string> SubjectS { get; } =
            ["速度", "耐力", "力量", "毅力", "智力"];
        private List<TrainInfo> T { get; } = [];
        public bool TryTrain()
        {
            if (Mnt.WaitTo([Symbol.返回, Button.训练], 0.8))
            {
                Subject巡视();

                PrimaryLogic();

                StartPlan();

                return true;
            }

            return false;
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
                while (FastCheck(Symbol.返回, 0.7))
                    Click(plan);
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
            T.Clear();
            do
            {
                Record训练信息(SubjectS[index]);
                var next = (index + 1) % SubjectS.Count;
                if (next == start)
                    break;
                Click(SubjectS[next], 50);
                index = next;
            } while (true);
            SortByIndex();
        }

        private void Record训练信息(string subject)
        {
            Mnt.Refresh();

            var 等效体力 = InSummer ? Vitality - 2 : Vitality;
            if (subject == "智力")
                等效体力 += 10;

            // 记录训练信息
            TrainInfo train = new()
            {
                Subject = subject,
                HeadInfo = GetHeadInfo(),
                Fail = FailPredict(等效体力)
            };

            // 定义读值次数：通过多次判断纠正错误
            int count = 2; // 一次约0.35s
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
            for (int i = 0; i < SubjectS.Count; i++)
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

        private int[] Once增益读值(string subject)
        {
            int[] V = SubjectS.Select(x => 0).ToArray();
            int[] rCode = subject switch
            {
                "速度" => [0, 2],
                "耐力" => [1, 3],
                "力量" => [1, 2],
                "毅力" => [0, 2, 3],
                "智力" => [0, 4],
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


        private class TrainInfo()
        {
            public string Subject { get; set; }
            public int[] UpS { get; set; }
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
