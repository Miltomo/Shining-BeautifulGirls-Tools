namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        abstract class Algorithm(ShiningGirl girl)
        {
            List<PlanInfo> T { get; } = [];

            public bool IsNeedInfo => T.Count == 0;
            public virtual bool IsG1Consider => false;
            public bool TodayNoSuitableRace { get; private set; } = false;

            /// <summary>
            /// 休息
            /// </summary>
            protected PlanInfo Relex => new()
            {
                Plan = PlanEnum.休息,
                Fail = First.Fail,
            };

            /// <summary>
            /// 外出
            /// </summary>
            protected PlanInfo GoOut => new()
            {
                Plan = PlanEnum.外出,
                Fail = First.Fail,
            };

            /// <summary>
            /// (日常)比赛
            /// </summary>
            protected PlanInfo Race => new()
            {
                Plan = PlanEnum.比赛,
            };

            /// <summary>
            /// 得分最高的训练项目
            /// </summary>
            protected PlanInfo First
            {
                get
                {
                    SortByScore();
                    return T[0];
                }
            }

            /// <summary>
            /// 增益值总和最高的项目
            /// </summary>
            protected PlanInfo MaxSum
            {
                get
                {
                    var max = T.Select(x => x.UpS.Sum()).Max();
                    return TheOne(t => t.UpS.Sum() >= max)!;
                }
            }


            protected ShiningGirl Girl { get; init; } = girl;
            protected int Turn => Girl.Turn;
            protected int Vitality => Girl.Vitality;
            protected int Mood => Girl.Mood;
            protected bool InSummer => Girl.InSummer;
            protected bool InContinuousRace => Girl.InContinuousRace;
            protected int[] TargetProperty => Girl.UserConfig.TargetProperty ?? [0, 0, 0, 0, 0];
            protected int[] CurrentProperty => Girl.Property;

            abstract protected double Score(PlanInfo info);
            abstract protected PlanInfo Select();
            virtual public bool Skip() => false;
            public PlanInfo PlanToDo() => Select();

            public virtual PlanInfo WhenNoRace()
            {
                if (Vitality > 69)
                    return TheOne(t => t.Plan == PlanEnum.智力)!;

                if (Mood < 5 && Vitality > 39)
                    return GoOut;

                return Relex;
            }



            protected virtual bool RaceFindingLogic()
            {
                if (Turn < 31)
                    return Girl.SelectFirstSuitableRace();

                return Girl.SelectMaxFansSuitableRace();
            }

            public bool CheckRaceEnvironment()
            {
                return !(InSummer || Mood < 3 || InContinuousRace && Mood < 5 || TodayNoSuitableRace);
            }

            public bool TryRace()
            {
                if (CheckRaceEnvironment())
                {
                    if (RaceFindingLogic())
                        return true;
                    else
                    {
                        TodayNoSuitableRace = true;
                        return false;
                    }
                }
                return false;
            }

            public virtual void Reset()
            {
                T.Clear();
                TodayNoSuitableRace = false;
            }



            //==================================================

            public string[] Print()
            {
                SortByScore();

                List<string> list = [];
                for (int i = 0; i < T.Count; i++)
                {
                    var dq = T[i];
                    var t = dq.Plan + " => ";
                    t += $"协助数: {dq.HeadInfo.Count} ";
                    t += "增益值: ";
                    for (int k = 0; k < dq.UpS.Length; k++)
                        t += dq.UpS[k] + " ";
                    t += $"得分: {dq.Score:f3}";
                    list.Add(t);
                }
                return [.. list];
            }

            public void Add(PlanInfo info)
            {
                info.Score = Score(info);
                T.Add(info);
            }

            /// <summary>
            /// 返回满足条件的(首个)训练项目
            /// </summary>
            /// <param name="predicate"></param>
            /// <returns>目标项目；如果找不到，则返回空值</returns>
            protected PlanInfo? TheOne(Func<PlanInfo, bool> predicate) =>
                T.Where(t => predicate(t)).FirstOrDefault();

            protected void SortByIndex()
            {
                int L = TrainingItems.Length - 1;
                for (int i = 0; i < L; i++)
                {
                    var orin = T.FindIndex(x => x.Plan == TrainingItems[i]);
                    (T[i], T[orin]) = (T[orin], T[i]);
                }
            }
            protected void SortByScore()
            {
                T.Sort((a, b) =>
                {
                    var v1 = a.Score;
                    var v2 = b.Score;
                    return v1 > v2 ? -1 : v1 < v2 ? 1 : 0;
                });
            }
        }
    }
}
