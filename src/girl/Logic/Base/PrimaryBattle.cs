namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        class PrimaryBattle(ShiningGirl girl) : PrimaryLogic(girl)
        {
            public override bool IsG1Consider => true;

            //TODO 智力只要求 > 25?
            private static bool SumFail(PlanInfo p)
            {
                var f = p.Fail;
                return p.UpS.Sum() > 30 + (f - 10 > 0 ? f - 10 : 0);
            }

            protected override PlanInfo Select()
            {
                if (Turn < 26)
                    return base.Select();

                var orin = base.Select();
                if (orin.Equals(Relex))
                {
                    if (Mood < 5)
                        return GoOut;
                    //TODO 要不要改为Relex？
                    else
                        return Race;
                }
                if (orin.Equals(Race))
                    return Race;
                if (orin.Equals(GoOut))
                    return GoOut;

                if (Mood < 4)
                    return GoOut;

                if (SumFail(orin))
                {
                    var index = Array.IndexOf(TrainingItems, orin.Plan);
                    if (CurrentProperty[index] < TargetProperty[index] - 100)
                        return orin;
                }

                //TODO 降低得分 > 16 的条件？ => 14? 会选择毅力的
                if (TheOne(x =>
                    x != orin && (x.Score > 16) &&
                    SumFail(x)
                    ) is PlanInfo ano)
                {
                    var index = Array.IndexOf(TrainingItems, ano.Plan);
                    if (CurrentProperty[index] < TargetProperty[index] - 100)
                        return ano;
                }

                return Race;
            }

            public override PlanInfo WhenNoRace()
            {
                if (Mood < 5)
                    return GoOut;

                return base.WhenNoRace();
            }

            protected override bool RaceFindingLogic() =>
                Girl.SelectMaxFansSuitableRace();
        }
    }
}
