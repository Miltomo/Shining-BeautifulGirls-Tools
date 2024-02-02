namespace Shining_BeautifulGirls
{
    partial class World
    {
#pragma warning disable IDE0001
        public class MoveControl
        {
            private static MoveControl? _instance;
            public static MoveControl Builder
            {
                get
                {
                    _instance ??= new MoveControl();
                    return _instance;
                }
            }
            private static void Consume()
            {
                _instance = null;
            }
            private static bool True
            {
                get
                {
                    Consume();
                    return true;
                }
            }
            private static bool False
            {
                get
                {
                    Consume();
                    return false;
                }
            }


            private enum ModeEnum
            {
                MoveTo,
                PageDown,
            }

            private class Configuration
            {
                public int Step { get; set; }
                public int MaxWait { get; set; }
            }

#pragma warning disable CS8618 // 禁止构造null检查
            private MoveControl() { }
#pragma warning restore CS8618 // 回复构造null检查
            private List<Action> DoSomes { get; set; } = [];
            private List<Func<bool>> Targets { get; } = [];
            private List<Func<bool>> Others { get; } = [];
            private object[] Buttons { get; set; } = [];
            private World World { get; set; }
            private ModeEnum Mode { get; set; }
            private Configuration Config { get; set; }


            public MoveControl AddAction(Action action)
            {
                DoSomes.Add(action);
                return this;
            }
            public MoveControl AddAction(string symbol, double sim = 0.9, params object[] bts) =>
                AddAction(() =>
                {
                    if (World.FastCheck(symbol, sim: sim))
                        World.Click(bts);
                });
            public MoveControl AddAction(Enum zone, Enum ptext, params object[] bts) =>
                AddAction(() =>
                {
                    if (World.ExtractZoneAndContains(zone, ptext))
                        World.Click(bts);
                });

            public MoveControl AddTarget(Func<bool> condition)
            {
                Targets.Add(condition);
                return this;
            }
            public MoveControl AddTarget(string symbol, double sim = 0.9) => AddTarget(() => World.FastCheck(symbol, sim: sim));
            public MoveControl AddTarget(Enum zone, Enum ptext) => AddTarget(() => World.ExtractZoneAndContains(zone, ptext));

            public MoveControl AddOpposite(Func<bool> condition)
            {
                Others.Add(condition);
                return this;
            }
            public MoveControl AddOpposite(string symbol, double sim = 0.9) => AddOpposite(() => World.FastCheck(symbol, sim: sim));
            public MoveControl AddOpposite(Enum zone, Enum ptext) => AddOpposite(() => World.ExtractZoneAndContains(zone, ptext));

            public MoveControl SetButtons(params object[] bts)
            {
                Buttons = bts;
                return this;
            }

            public bool StartM(World world, int step = 300, int sec = 0)
            {
                World = world;
                Mode = ModeEnum.MoveTo;
                Config = new Configuration()
                {
                    Step = step < 20 ? 20 : step,
                    MaxWait = sec < 0 ? 0 : sec,
                };
                return Execute();
            }

            public bool StartP(World world, int step = 300, int sec = 10)
            {
                World = world;
                Mode = ModeEnum.PageDown;
                Config = new Configuration()
                {
                    Step = step < 20 ? 20 : step,
                    MaxWait = sec < 5 ? 5 : sec,
                };
                return Execute();
            }

            private bool Execute()
            {
                int sum = 0;
                int step = Config.Step;
                int max = Config.MaxWait * 1000;
                bool toClick = Mode switch
                {
                    ModeEnum.MoveTo => true,
                    ModeEnum.PageDown => false,
                    _ => true,
                };

                while (true)
                {
                    if (toClick)
                        World.Click(Buttons, step);
                    else
                        World.Pause(step);


                    DoSomes.ForEach(a => a());


                    //==========检测条件==========
                    if (Targets.Count == 0)
                        return True;

                    foreach (var t in Targets)
                        if (t())
                        {
                            if (Mode == ModeEnum.PageDown)
                                World.Click(Buttons);
                            return True;
                        }

                    foreach (var ex in Others)
                        if (ex())
                            return False;
                    //============================

                    sum += step;

                    switch (Mode)
                    {
                        case ModeEnum.MoveTo:
                            toClick = false;
                            if (sum > max)
                            {
                                toClick = true;
                                sum = 0;
                            }
                            break;

                        case ModeEnum.PageDown:
                            if (sum > max)
                            {
                                World.ClickLast();
                                sum = 0;
                            }
                            break;
                    }
                }
            }
        }
#pragma warning restore IDE0001
    }
}
