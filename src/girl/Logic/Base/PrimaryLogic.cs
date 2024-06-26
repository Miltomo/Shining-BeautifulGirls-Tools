﻿namespace Shining_BeautifulGirls
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


            protected override double Score(Plan info)
            {
                if (Turn < 26)
                    return info.HeadInfo.Count;

                var s = .0;
                // 当体力较低时，增加智力的基础得分
                if (Vitality < 65 && info.Name == PlanName.智力)
                    s += 2;

                //(delta) 检测当前项目属性值与目标值差距；并给予相应奖罚措施
                int index = Plan.GetIndexOfTrainning(info);
                var dq = CurrentProperty[index];
                var tg = TargetProperty[index];

                // 若属性值超过1000
                if (dq > 1000)
                    s -= 2;
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

            protected override Plan Select()
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
                            if (Turn > 11)
                                return Race;
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

                var maxUp = target.UpS.Max();
                var sumUp = target.UpS.Sum();
                int baseScore = 18;
                failLine = 30;

                // 增益过低的情况
                if (maxUp < 25)
                {
                    if (score < baseScore || sumUp < 25)
                    {
                        if (Vitality < 70)
                        {
                            if (Mood < 4) return GoOut;
                            if (target.Is智力 && score > baseScore) return target;
                        }

                        if (Vitality < 40)
                            return Relex;

                        return Race;
                    }
                }

                // 与失败率比较
                if (target.Fail > 1 * (score - baseScore) + failLine)
                {
                    if (Mood < 4)
                        return GoOut;
                    else
                        return Relex;
                }

                return target;
            }

            public override Plan WhenNoRace()
            {
                if (InSummer)
                {
                    if (Vitality > 50)
                        return TheOne(t => t.Is智力) ?? First;
                    else
                        return Relex;
                }

                var m = MaxSum;
                if (m.UpS.Sum() > 24 && m.Fail < 25)
                    return m;

                return base.WhenNoRace();
            }

            public override bool IsReadUpValue => Turn > 25;
        }
    }
}
