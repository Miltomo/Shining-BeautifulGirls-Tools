namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        class PrimaryLogic : Algorithm
        {

            private double[] W { get; init; }


            public PrimaryLogic(ShiningGirl girl) : base(girl)
            {
                double s = TargetProperty.Sum();
                W = TargetProperty.Select(x => x / s).ToArray();
            }


            protected override double Score(PlanInfo info)
            {
                if (Turn < 26)
                    return info.HeadInfo.Count;

                var s = .0;
                // 当体力较低时，增加智力的基础得分
                if (Vitality < 65 && info.Plan == PlanEnum.智力)
                    s += 2;

                //(delta) 检测当前项目属性值与目标值差距；并给予相应奖罚措施
                int index = Array.IndexOf(TrainingItems, info.Plan);
                var dq = CurrentProperty[index];
                var tg = TargetProperty[index];

                // 若属性值超过1000
                if (dq > 1000)
                    s -= 5;
                // 若已超过目标值(严重惩罚)
                if (dq > tg)
                    s -= 10;
                else
                    s += Math.Ceiling(10 * ((tg - dq) / (double)tg));

                //(核心) 增益值与权重之积决定得分
                var V = info.UpS;
                for (int k = 0; k < V.Length; k++)
                    s += V[k] * W[k];

                //(核心2) 协助数因子
                s += info.HeadInfo.Count * 2.8;

                return s;
            }

            protected override PlanInfo Select()
            {
                var target = First;
                var score = target.Score;
                var fail = target.Fail;
                int failLine;

                if (Turn < 26)
                {
                    failLine = 25;


                    switch (score)
                    {
                        case < 2:
                            if (Mood < 5)
                                return GoOut;
                            break;
                        case < 3:
                            if (Mood < 4 && Vitality < 90)
                                return GoOut;
                            break;
                    }


                    if (fail > failLine)
                    {
                        if (score > 2 && (fail < 5 * (score - 2) + failLine))
                            return target;

                        switch (fail)
                        {
                            case < 31:
                                if (Mood < 5)
                                    return GoOut;
                                break;
                            case < 41:
                                if (Mood < 4)
                                    return GoOut;
                                break;
                        }

                        return Relex;
                    }

                    return target;
                }

                int baseScore = 18;
                failLine = 30;

                // 与心情惩罚值比较
                if (score - (5 - Mood) * 9 < 0.1)
                    return GoOut;
                // 与基准得分比较
                else if (score < baseScore)
                {
                    var maxUp = target.UpS.Max();
                    // 目标为智力的情况
                    if (target.Plan == PlanEnum.智力)
                    {
                        if (Vitality < 41 && maxUp < 25)
                            return Relex;
                        return target;
                    }

                    // 训练其他项目的情况
                    if (maxUp < 21 && Vitality < 61)
                        return Relex;
                }
                // 与失败率比较
                else if (target.Fail > 1 * (score - baseScore) + failLine)
                {
                    if (Mood < 4)
                        return GoOut;
                    else
                        return Relex;
                }

                return target;
            }
        }
    }
}
