namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        abstract class Algorithm(ShiningGirl girl)
        {
            List<Plan> T { get; } = [];
            public bool IsNeedInfo => T.Count == 0;
            public virtual bool IsReadUpValue => true;
            public virtual bool IsG1Consider => false;
            public bool TodayNoSuitableRace { get; private set; } = false;

            /// <summary>
            /// 休息
            /// </summary>
            protected Plan Relex => new()
            {
                Name = PlanName.休息,
                Fail = First.Fail,
            };

            /// <summary>
            /// 外出
            /// </summary>
            protected Plan GoOut => new()
            {
                Name = PlanName.外出,
                Fail = First.Fail,
            };

            /// <summary>
            /// (日常)比赛
            /// </summary>
            protected Plan Race => new()
            {
                Name = PlanName.比赛,
            };

            /// <summary>
            /// 得分最高的训练项目
            /// </summary>
            protected Plan First
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
            protected Plan MaxSum
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

            abstract protected double Score(Plan info);
            abstract protected Plan Select();
            virtual public bool Skip() => false;
            public Plan PlanToDo() => Select();

            //TODO 再次包装，带结果检查！
            public virtual Plan WhenNoRace()
            {
                if (Vitality > 69)
                    return TheOne(t => t.Is智力) ?? First;

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
                    var t = dq.Name + " => ";
                    t += $"协助数: {dq.HeadInfo.Count} ";
                    if (IsReadUpValue)
                    {
                        t += "增益值: ";
                        for (int k = 0; k < dq.UpS.Length; k++)
                            t += dq.UpS[k] + " ";
                    }
                    t += $"得分: {dq.Score:f3}";
                    list.Add(t);
                }
                return [.. list];
            }

            public void Add(Plan info)
            {
                info.Score = Score(info);
                T.Add(info);
            }

            /// <summary>
            /// 返回满足条件的(首个)训练项目
            /// </summary>
            /// <param name="predicate"></param>
            /// <returns>目标项目；如果找不到，则返回空值</returns>
            protected Plan? TheOne(Func<Plan, bool> predicate) =>
                T.Where(t => predicate(t)).FirstOrDefault();

            protected void SortByIndex()
            {
                int L = Plan.TrainningItemsCount - 1;
                for (int i = 0; i < L; i++)
                {
                    var orin = T.FindIndex(x => x.Name == Plan.GetTrainningItemByIndex(i));
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

        private class Plan
        {
            public PlanName Name { get; set; }
            public int[] UpS { get; set; } = [];
            public HeadInfo HeadInfo { get; set; }

            private int _fail = 0;
            public int Fail
            {
                get => _fail;
                set => _fail = value < 0 ? 0 : value;
            }
            public double Score { get; set; } = 0d;


            private static readonly PlanName[] TrainingItems =
                [PlanName.速度, PlanName.耐力, PlanName.力量, PlanName.毅力, PlanName.智力];

            public static bool IsTrainningPlan(Plan info) => TrainingItems.Contains(info.Name);
            public bool Is训练任务 => IsTrainningPlan(this);
            public bool Is非训练任务 => !IsTrainningPlan(this);
            public bool Is速度 => Name == PlanName.速度;
            public bool Is耐力 => Name == PlanName.耐力;
            public bool Is力量 => Name == PlanName.力量;
            public bool Is毅力 => Name == PlanName.毅力;
            public bool Is智力 => Name == PlanName.智力;
            public bool IsRelex => Name == PlanName.休息;
            public bool IsGoOut => Name == PlanName.外出;
            public bool IsRace => Name == PlanName.比赛;

            public int TrainningIndex => GetIndexOfTrainning(this);
            public static int TrainningItemsCount => TrainingItems.Length;
            public static int GetIndexOfTrainning(Plan info) => Array.IndexOf(TrainingItems, info.Name);
            public static PlanName GetTrainningItemByIndex(int index) => TrainingItems[index];

            public override bool Equals(object? obj)
            {
                if (obj is Plan another)
                    return another.Name == Name;
                if (obj is PlanName pName)
                    return pName == Name;
                return false;
            }
        }

        private enum PlanName
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
    }
}
