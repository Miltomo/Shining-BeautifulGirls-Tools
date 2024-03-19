namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        class PrimaryBattle(ShiningGirl girl) : PrimaryLogic(girl)
        {
            public override bool IsG1Consider => true;

            private bool SumFail(Plan p)
            {
                int s = p.Is智力 ? 25 : 30;
                var f = p.Fail;
                var index = Plan.GetIndexOfTrainning(p);
                return
                    (p.UpS.Sum() > s + (f - 10 > 0 ? f - 10 : 0)) &&
                    (CurrentProperty[index] < TargetProperty[index] - 100);
            }

            protected override Plan Select()
            {
                if (Turn < 30)
                    return base.Select();

                var orin = base.Select();

                // 若为非训练项目
                if (orin.Is非训练任务)
                {
                    if (orin.IsRelex)
                    {
                        if (Mood < 5)
                            return GoOut;
                        else
                            return Relex;
                    }
                    else
                        return orin;
                }

                if (Mood < 4)
                    return GoOut;

                if (SumFail(orin))
                    return orin;

                if (TheOne(x =>
                    x != orin && (x.Score > 16) &&
                    SumFail(x)
                    ) is Plan ano)
                    return ano;

                return Race;
            }

            public override Plan WhenNoRace()
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
