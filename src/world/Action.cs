using System;
using static ComputerVision.ImageRecognition;

namespace Shining_BeautifulGirls
{
    partial class World
    {
        private bool 竞技场流程()
        {
            var state = "初入";
            bool Aw = true;
            while (Aw)
            {
                switch (state)
                {
                    case "初入":
                        Log("开始自动竞技场");
                        MoveTo(["赛事", "赛事"], 1);
                        Click("JJC1");
                        MoveTo(["竞技场", "继续"], 1);
                        Click("JJC2");
                        if (!PageDownEx(["选队"], ["竞技值不足"]))
                        {
                            Click("主页");
                            return false;
                        }
                        state = "选队";
                        break;
                    case "选队":
                        Pause(1000);
                        string target = $"第{(UserConfig is null ? 1 : UserConfig.TeamNumber)}队";
                        string gift = MakeUniqueCacheFile("gift");
                        for (int i = 1; i < 4; i += 1)
                        {
                            var dq = $"第{i}队";
                            Gray(CropScreen(dq)).SaveImage(gift);
                            if (CheckSymbol("胜利时必得", gift, 0.7))
                            {
                                target = dq;
                                break;
                            }
                        }
                        MoveTo(["VS", target]);
                        MoveTo(["选择道具", "继续"]);
                        MoveTo(["VS", "JJC3"]);
                        state = "跳跃处理";
                        break;
                    case "跳跃处理":
                        Pause(300);
                        if (CheckSymbol("未全跳"))
                        {
                            Click("JJC4");
                            state = "查看结果";
                        }
                        else if (CheckSymbol("已全跳"))
                            state = "查看结果";
                        else
                            state = "跳跃处理";
                        break;
                    case "查看结果":
                        Click("比赛结束1", 1000);
                        Click("快进");
                        PageDown(["赛事结束", "比赛结束3"]);
                        state = "循环处理";
                        break;
                    case "循环处理":
                        if (CheckSymbol("竞技值不足"))
                        {
                            MoveTo(["主界面", "主页"], 0, 0.8);
                            state = "结束";
                            break;
                        }
                        else if (CheckSymbol("选队"))
                        {
                            Log("再次参赛");
                            state = "选队";
                        }
                        else
                            Click("竞技场连点");
                        break;
                    case "结束":
                        Aw = false;
                        break;
                    default:
                        break;
                }
            }
            Log("结束竞技场");
            return true;
        }

        private void 日常赛事流程(string state = "进入")
        {
            switch (state)
            {
                case "进入":
                    MoveTo(["赛事", "赛事"], 1);
                    MoveTo(["金币", "日常赛事"], 6, 0.8);
                    日常赛事流程("赛事选择");
                    break;
                case "赛事选择":
                    while (CheckSymbol("金币"))
                        Click($"日{(UserConfig is not null ? UserConfig.DailyRaceNumber : 2)}", 1000);
                    Click($"日{(UserConfig is not null ? UserConfig.DRDNumber : 1)}");
                    if (PageDownEx(["参赛!"], ["日常赛事券不足"]))
                    {
                        if (CheckSymbol("多次参赛ON", delta: 0.99))
                        {
                            Click("比赛结束2");
                            PageDown(["返回"], sim: 0.7);
                            日常赛事流程("参赛");
                        }
                        else
                        {
                            Log("⚠️未开启多次参赛，自行开启后再尝试⚠️");
                            Click("取消");
                            日常赛事流程("退出");
                        }
                    }
                    else
                    {
                        Log("日常赛事券不足，已退出");
                        日常赛事流程("退出");
                    }
                    break;
                case "参赛":
                    Log("进行日常赛事比赛");
                    ClickEx("继续", "多次参赛", ["弹窗确认"]);
                    MoveTo(["返回", "比赛结束1"], sec: 1, sim: 0.7);
                    日常赛事流程("退出");
                    break;
                case "退出":
                    MoveTo(["主界面", "主页"], 0, 0.8);
                    break;
            }
        }

        private bool 养成流程()
        {
            bool 已存在养成 = false;

            Click("主页");
            if (PageDownEx(["养成", "养成"], ["养成2"]))
            {
                PageDown(["继续"]);
                Pause(1000);
                ClickEx("继续", "选择养成难度", ["比赛结束2"]);
                if (!MoveToEx([["选卡"], ["继续"]], [
                    ["未选继承"]
                    ], sec: 0))
                {
                    Log("⚠️请先自行选好继承优俊少女再尝试养成⚠️");
                    return false;
                }

                var card = UserConfig is null ? "北部玄驹" : UserConfig.SupportCard;
                if (寻找协助卡(card))
                    PageDown(["选卡", "继续"]);
                else
                {
                    Log($"⚠️未找到目标协助卡\"{card}\"⚠️");
                    throw new Exception();
                }

                if (!PageDownEx(["最终确认"], ["回复训练值确认"], 0.99))
                {
                    if ((UserConfig?.CultivateUseProp ?? false) && UserConfig.CultivateCount > 0)
                    {
                        Click("弹窗确认");
                        PageDown(["回复训练值", "使用能量饮料"]);
                        PageDown(["回复选条", "大弹窗确认"]);
                        MoveTo(["最终确认", "继续"], sec: 0);
                    }
                    else
                    {
                        MoveTo(["养成", "主页"], 0);
                        return false;
                    }
                };

                Log("确定养成");
                Click("开始养成");
                PageDown(["快进"]);
                ClickEx("快进", "快进确认", ["弹窗勾选", "弹窗确认"]);
                PageDown(["缩短事件"]);
                Click(["跳过", "跳过", "缩短所有事件", "缩短事件确定"]);
                PageDown(["养成主页"], 0.8);
                Log("到达界面，正式开始");
                Girl = default;
            }
            else
            {
                已存在养成 = true;
                Click("养成");
                PageDown(["继续养成", "大弹窗确认"]);
                Log("继续上次养成");
            }

            if (Girl is null || Girl.EndTraining)
            {
                Girl = new ShiningGirl(this, UserConfig?.SBGConfig);
                if (已存在养成)
                    Log("⚠️未检测到上次养成数据，会从零开始进行养成，这可能会导致行为不符合预期");
                Girl.BeginNewDay();
            }
            else
            {
                Log("✔️检测到上次养成数据");
                Log($"回合数: {Girl.Turn}");
                Log($"属性值: {Girl.Property[0]} , {Girl.Property[1]} , {Girl.Property[2]} , {Girl.Property[3]} , {Girl.Property[4]}");
                Log($"技能学习次数: {Girl.SkTurns}");
                Girl.UserConfig = UserConfig?.SBGConfig;
                Girl.Continue();
            }

            return true;
        }

        public void 标准竞技场()
        {
            if (竞技场流程())
            {
                Log("完成竞技场任务");
            }
            else
            {
                Log("竞技值不足，已退出");
            };
        }

        public void 标准日常赛事()
        {
            日常赛事流程();
            Log("结束日常赛事任务");
        }

        public bool 一次养成()
        {
            return 养成流程();
        }

        public void 自定义养成()
        {
            if (UserConfig is not null)
            {
                var count = UserConfig.CultivateCount;
                int t = 0;
                while (true)
                {
                    Log($"#####准备第 {++t} 次养成######");
                    if (count > 0)
                    {
                        while (true)
                        {
                            if (一次养成())
                                break;
                            // 等待十分钟
                            Pause(10 * 60 * 1000);
                        }
                        if (t == count)
                            break;
                    }
                    else
                    {
                        if (一次养成())
                            continue;
                        Log($"训练值不足，结束任务");
                        t--;
                        break;
                    }
                }
                Log($"#####养成结束，共完成 {t} 次养成######");
            }
        }

        private bool 寻找协助卡(string card)
        {
            Log("正在选择好友协助卡");
            Click("选卡");
            Scroll([100, 1100, 80]);
            int i = 1;
            bool hasFound = false;
            for (int t = 0; t < 4; t++)
            {
                Refresh();
                for (i = 1; i < 6; i++)
                {
                    if (CheckCard(card, MaskScreen($"卡{i}", "card")))
                    {
                        Log($"已找到目标卡「{card}」");
                        Click($"卡{i}");
                        hasFound = true;
                        break;
                    }
                }

                if (hasFound)
                    break;
                Scroll([100, 1100, 750]);
                Pause(1000);
            }
            return hasFound;
        }

        public void Start()
        {
            _stop = false;
        }

        public void Stop()
        {
            _stop = true;
        }
    }
}
