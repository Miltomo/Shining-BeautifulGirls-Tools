namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        private TrainInfo _plan = new() { Subject = "休息" };
        private TrainInfo PlanToDo
        {
            get => _plan;
            set
            {
                CurrentFail = value.Fail;
                _plan = value;
            }
        }
        private int CurrentFail { get; set; } = 0;

        private void PrimaryLogic()
        {
            if (Turn > 25)
            {
                PrimaryLogicPart2();
                return;
            }

            SortByHead();
            PlanToDo = T[0];
            var headScore = PlanToDo.HeadInfo.Count;
            var failLine = 25;

            switch (headScore)
            {
                case < 2:
                    if (Mood < 5)
                        去外出();
                    break;
                case < 3:
                    if (Mood < 4 && Vitality < 90)
                        去外出();
                    break;
            }


            if (CurrentFail > failLine)
            {
                if (headScore > 2 && (CurrentFail < 5 * (headScore - 2) + failLine))
                    return;

                去休息();
                switch (CurrentFail)
                {
                    case < 31:
                        if (Mood < 5)
                            去外出();
                        break;
                    case < 41:
                        if (Mood < 4)
                            去外出();
                        break;
                }
                /*switch (headScore)
                {
                    case < 3:
                        if (Mood < 5)
                            去外出();
                        else
                            去休息();
                        break;
                    case > 2:
                        if (CurrentFail > 5 * (headScore - 2) + failLine)
                            去休息();
                        break;
                }*/
            }
        }

        private void PrimaryLogicPart2()
        {
            var targetV = UserConfig?.TargetProperty ??
                [1100, 1000, 700, 360, 400];
            List<double> w = [];
            double s = targetV.Sum();
            for (int i = 0; i < targetV.Length; i++)
                w.Add(targetV[i] / s);

            // 当体力较低时，增加智力的基础得分
            if (Vitality < 65)
                T[^1].Score += 2;

            for (int i = 0; i < Property.Length; i++)
            {
                var dq = Property[i];
                var tg = targetV[i];

                if (dq > 1000)
                    T[i].Score -= 5;
                if (dq > tg)
                    T[i].Score -= 10;
                else
                    T[i].Score += Math.Ceiling(10 * ((tg - dq) / (double)tg));
            }

            for (int i = 0; i < T.Count; i++)
            {
                var dq = T[i];
                var upS = dq.UpS;
                for (int k = 0; k < upS.Length; k++)
                    dq.Score += upS[k] * w[k];
                dq.Score += dq.HeadInfo.Count * 2.8;
            }

            SortByScore();

            PlanToDo = T[0];
            var baseScore = 18;
            var currentScore = PlanToDo.Score;
            var failLine = 30;

            // 与心情惩罚值比较
            if (currentScore - (5 - Mood) * 9 < 0.1)
                去外出();
            // 与基准得分比较
            else if (currentScore < baseScore)
            {
                var maxUp = PlanToDo.UpS.Max();
                // 目标为智力的情况
                if (PlanToDo.Subject == "智力")
                {
                    if (Vitality < 41 && maxUp < 25)
                        去休息();
                    return;
                }

                // 训练其他项目的情况
                if (maxUp < 21 && Vitality < 61)
                    去休息();
            }
            // 与失败率比较
            else if (CurrentFail > 1 * (currentScore - baseScore) + failLine)
            {
                if (Mood < 4)
                    去外出();
                else
                    去休息();
            }
        }

        private void SortByHead()
        {
            T.Sort((a, b) =>
            {
                var v1 = a.HeadInfo.Count;
                var v2 = b.HeadInfo.Count;
                return v1 > v2 ? -1 : v1 < v2 ? 1 : 0;
            });
            foreach (var t in T)
                t.Score = t.HeadInfo.Count;
        }
        private void SortByScore()
        {
            T.Sort((a, b) =>
            {
                var v1 = a.Score;
                var v2 = b.Score;
                return v1 > v2 ? -1 : v1 < v2 ? 1 : 0;
            });
        }

        private void SortByIndex()
        {
            int L = SubjectS.Count - 1;
            for (int i = 0; i < L; i++)
            {
                var orin = T.FindIndex(x => x.Subject == SubjectS[i]);
                (T[i], T[orin]) = (T[orin], T[i]);
            }
        }

        private void 去休息()
        {
            PlanToDo = new()
            {
                Subject = "休息",
                Fail = CurrentFail
            };
        }
        private void 去外出()
        {
            PlanToDo = new()
            {
                Subject = "外出",
                Fail = CurrentFail
            };
        }
    }
}
