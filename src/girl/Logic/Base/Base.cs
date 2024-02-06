namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        abstract class Algorithm(ShiningGirl girl)
        {
            List<PlanInfo> T { get; } = [];

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

            protected int Turn => girl.Turn;
            protected int Vitality => girl.Vitality;
            protected int Mood => girl.Mood;
            protected int[] TargetProperty => girl.UserConfig!.TargetProperty!;
            protected int[] CurrentProperty => girl.Property;

            public void Add(PlanInfo info)
            {
                info.Score = Score(info);
                T.Add(info);
            }

            public PlanInfo PlanToDo()
            {
                return Select();
            }

            public string[] Print()
            {
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


            /// <summary>
            /// 返回满足条件的(首个)训练项目；如果没有，则返回得分最高的
            /// </summary>
            /// <param name="predicate"></param>
            /// <returns></returns>
            protected PlanInfo TheOne(Func<PlanInfo, bool> predicate)
            {
                return T.Where(t => predicate(t)).FirstOrDefault() ?? First;
            }

            abstract protected double Score(PlanInfo info);
            abstract protected PlanInfo Select();
            virtual public bool Skip() => false;

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
