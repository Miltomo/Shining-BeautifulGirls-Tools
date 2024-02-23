namespace Shining_BeautifulGirls
{
    partial class World
    {
#pragma warning disable IDE0001
        public class MoveControl
        {
            public static MoveControl Builder => new();
            private static bool True => true;
            private static bool False => false;
            private enum ModeEnum
            {
                MoveTo,
                WaitTo,
                PageDown,
            }

            private class Configuration
            {
                public int Step { get; set; } = 200;
                public int MaxWait { get; set; } = 0;
                public ModeEnum Mode { get; set; } = ModeEnum.MoveTo;
            }

#pragma warning disable CS8618 // 禁止构造null检查
            private MoveControl() { }
#pragma warning restore CS8618 // 恢复构造null检查
            private List<Action> DoSomethings { get; set; } = [];
            private List<Func<bool>> Targets { get; } = [];
            private List<Func<bool>> Others { get; } = [];
            private object[] Buttons { get; set; } = [];
            private World World { get; set; }
            private Configuration Config { get; set; }


            public MoveControl AddProcess(Action action)
            {
                DoSomethings.Add(action);
                return this;
            }
            public MoveControl AddProcess(string symbol, double sim = 0.9, params object[] bts) =>
                AddProcess(() =>
                {
                    if (World.FastCheck(symbol, sim: sim))
                        World.Click(bts);
                });
            public MoveControl AddProcess(Enum zone, Enum ptext, params object[] bts) =>
                AddProcess(() =>
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

            public bool StartAsMoveTo(World world, int step = 300, int sec = 0)
            {
                World = world;
                Config = new Configuration()
                {
                    Step = step < 20 ? 20 : step,
                    MaxWait = sec < 0 ? 0 : sec,
                    Mode = ModeEnum.MoveTo,
                };
                return Execute();
            }

            public bool StartAsWaitTo(World world, int step = 500, int sec = 5)
            {
                World = world;
                Config = new Configuration()
                {
                    Step = step < 200 ? 200 : step,
                    MaxWait = sec < 2 ? 2 : sec,
                    Mode = ModeEnum.WaitTo,
                };
                return Execute();
            }

            public bool StartAsPageDown(World world, int step = 300, int sec = 10)
            {
                World = world;
                Config = new Configuration()
                {
                    Step = step < 20 ? 20 : step,
                    MaxWait = sec < 5 ? 5 : sec,
                    Mode = ModeEnum.PageDown,
                };
                return Execute();
            }

            public bool StartAsClickEx(World world, int step = 1000)
            {
                if (Targets.Count > 0)
                    throw new ArgumentException("不应设置Target");

                World = world;
                Config = new Configuration()
                {
                    Step = step,
                };
                return Execute();
            }

            private bool Execute()
            {
                int sum = 0;
                int step = Config.Step;
                int max = Config.MaxWait * 1000;
                var mode = Config.Mode;
                bool toClick = mode switch
                {
                    ModeEnum.PageDown => false,
                    _ => true,
                };

                while (true)
                {
                    if (toClick && Buttons.Length > 0)
                        World.Click(Buttons, step);
                    else
                        World.Pause(step);


                    DoSomethings.ForEach(a => a());


                    //==========检测条件==========
                    if (Targets.Count == 0)
                        return True;

                    foreach (var t in Targets)
                        if (t())
                        {
                            if (mode == ModeEnum.PageDown)
                                World.Click(Buttons);
                            return True;
                        }

                    foreach (var ex in Others)
                        if (ex())
                            return False;
                    //============================

                    sum += step;

                    switch (mode)
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

                        case ModeEnum.WaitTo:
                            if (sum > max)
                                return False;
                            break;
                    }
                }
            }
        }
#pragma warning restore IDE0001
    }
}
