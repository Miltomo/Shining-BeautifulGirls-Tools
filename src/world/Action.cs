using MHTools;
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
                        MoveTo([Symbol.赛事, Button.赛事], 1);
                        Click(Button.JJC1);
                        MoveTo([Symbol.竞技场, Button.继续], 1);
                        Click(Button.JJC2);
                        if (!PageDownEx([Symbol.选队], [Symbol.竞技值不足]))
                        {
                            Click(Button.主页);
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
                            if (Check(Symbol.胜利时必得, gift, 0.7))
                            {
                                target = dq;
                                break;
                            }
                        }
                        MoveTo([Symbol.继续, target], sim: 0.8);
                        Click(Button.继续);
                        PageDown([Symbol.选择道具, Button.JJC3]);
                        state = "比赛处理";
                        break;

                    case "比赛处理":
                        Pause(2000);
                        MoveTo([Symbol.赛事结束, Button.比赛结束, Button.快进], sec: 0);
                        Click(Button.低继续);
                        state = "循环处理";
                        break;

                    case "循环处理":
                        Refresh();
                        if (FastCheck(Symbol.竞技值不足, sim: 0.8))
                        {
                            MoveTo([Symbol.主界面, Button.主页], 0, 0.8);
                            state = "结束";
                            break;
                        }
                        else if (FastCheck(Symbol.选队, sim: 0.8))
                        {
                            Log("再次参赛");
                            state = "选队";
                        }
                        else
                            Click(Button.竞技场连点);
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
                    MoveTo([Symbol.赛事, Button.赛事], 1);
                    MoveTo([Symbol.金币, Button.日常赛事], 6, 0.8);
                    日常赛事流程("赛事选择");
                    break;
                case "赛事选择":
                    while (Check(Symbol.金币))
                        Click($"日{(UserConfig is not null ? UserConfig.DailyRaceNumber : 2)}", 1000);
                    Click($"日{(UserConfig is not null ? UserConfig.DRDNumber : 1)}");
                    if (PageDownEx([Symbol.参赛], [Symbol.日常赛事券不足]))
                    {
                        if (Check(Symbol.多次参赛ON, delta: 0.99))
                        {
                            Click(Button.比赛结束);
                            PageDown([Symbol.返回]);
                            日常赛事流程("参赛");
                        }
                        else
                        {
                            Log("⚠️未开启多次参赛，自行开启后再尝试⚠️");
                            Click(Button.取消);
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
                    ClickEx(Button.继续, Symbol.多次参赛, [Button.弹窗确认]);
                    MoveTo([Symbol.返回, Button.比赛结束], sec: 1);
                    日常赛事流程("退出");
                    break;
                case "退出":
                    MoveTo([Symbol.主界面, Button.主页], 0, 0.8);
                    break;
            }
        }

        private void 群英联赛流程()
        {
            MoveTo([Symbol.赛事, Button.赛事], 1);
            Click(Button.赛事活动);
            PageDown([Symbol.赛事活动, Button.群英联赛]);

        入场判定:
            PageDown([Symbol.返回]);
            if (ExtractZoneAndContains(Zone.中部, PText.Extravaganza.决赛))
            {
                Log("⚠️已进入决赛，请自行操作⚠️");
                goto 退出;
            }

            // 无法报名
            if (IsNoLight(ZButton.群英联赛报名))
            {
                goto 退出;
            }
            Click(ZButton.群英联赛报名);
            goto 开始分歧;

        开始分歧:
            Pause(1000);
            Refresh();
            var zb = ExtractZone(Zone.中部);

            if (zb.Equals("报名确认"))
            {
                Click(Button.弹窗确认);
                goto 开始分歧;
            }
            else if (zb.Contains("适应性"))
            {
                MoveTo(() => ExtractZoneAndContains(Zone.中部, "参赛登记确认"), [Button.继续]);
                Click(Button.弹窗确认);
                goto 开始分歧;
            }
            else if (zb.Contains("奖励"))
            {
                if (IsNoLight(ZButton.群英联赛_赛事与奖励))
                    goto 退出;

                var xb = ExtractZone(Zone.下部);
                // 参赛
                if (xb.Contains("赛事"))
                    goto 比赛处理;
                // 结束
                else if (xb.Contains("获得奖励"))
                    goto 结果处理;
            }
            goto 开始分歧;

        比赛处理:
            Click(ZButton.群英联赛_赛事与奖励);
            if (WaitTo([Symbol.继续], maxWait: 299))
            {
                MoveTo([Symbol.快进, Button.比赛结束], 0);
                MoveTo([Symbol.继续, Button.比赛连点], 0);
                Click(Button.比赛结束);
                goto 开始分歧;
            }
            goto 退出;

        结果处理:
            Click(ZButton.群英联赛_赛事与奖励);
            PageDown([Symbol.继续, Button.比赛结束]);
            goto 入场判定;

        退出:;
        }

        /// <summary>
        /// 进行一次养成的标准流程
        /// </summary>
        /// <returns>true,当成功进行了完整养成; false,当且仅当训练值不足时</returns>
        /// <exception cref="StopException"></exception>
        private bool 养成流程()
        {
            bool 已存在养成 = false;

            MoveTo([Symbol.主界面, Button.主页], 0, 0.8);
            if (PageDownEx([Symbol.养成, Button.养成], [Symbol.养成2], sim: 0.8))
            {
                PageDown([Symbol.继续]);
                Pause(1000);
                ClickEx(Button.继续, Symbol.选择养成难度, [Button.比赛结束]);
                if (!MoveToEx([[Symbol.选卡], [Button.继续]], [
                    [Symbol.未选继承]
                    ], sec: 0))
                {
                    Log("⚠️请先自行选好继承优俊少女再尝试养成⚠️");
                    throw new StopException();
                }

                var card = UserConfig is null ? "北部玄驹" : UserConfig.SupportCard;
                if (寻找协助卡(card))
                    PageDown([Symbol.选卡, Button.继续]);
                else
                {
                    Log($"⚠️未找到目标协助卡\"{card}\"⚠️");
                    throw new StopException();
                }

                // 训练值不足时的处理
                if (!PageDownEx([Symbol.最终确认], [Symbol.回复训练值确认], 0.8))
                {
                    if (UserConfig is not null && (UserConfig.CultivateCount > 0))
                    {
                        Click(Button.弹窗确认);
                        PageDown([Symbol.回复训练值]);

                        if (UserConfig.CultivateUseProp)
                        {
                            if (FastCheck(Symbol.能量饮料))
                                Click(Button.使用能量饮料);
                            else goto UseMoney;
                            PageDown([Symbol.回复选条, Button.大弹窗确认]);
                            goto End;
                        }

                    UseMoney:
                        if (UserConfig.CultivateUseMoney)
                        {
                            Click(Button.使用宝石);
                            PageDown([Symbol.回复选条]);
                            Click(Button.加号);
                            Click(Button.大弹窗确认);
                        }
                        else
                        {
                            // 关闭
                            Click(Button.比赛结束);
                            return false;
                        }

                    End:
                        MoveTo([Symbol.最终确认, Button.继续], sec: 0);
                    }
                    else
                        return false;
                };

                Log("确定养成");
                Click(Button.开始养成);
                PageDown([Symbol.快进]);
                ClickEx(Button.快进, Symbol.快进确认, [Button.弹窗勾选, Button.弹窗确认]);
                PageDown([Symbol.缩短事件]);
                Click([Button.跳过, Button.跳过, Button.缩短所有事件, Button.缩短事件确定]);
                PageDown([Symbol.养成主页], 0.8);
                Log("到达界面，正式开始");
                Girl = default;
            }
            else
            {
                已存在养成 = true;
                Click(Button.养成);
                PageDown([Symbol.继续养成, Button.大弹窗确认]);
                Log("继续上次养成");
            }

            if (Girl is null || Girl.EndTraining)
            {
                Girl = new ShiningGirl(this, UserConfig?.SBGConfig);
                if (已存在养成)
                    Log("⚠️未检测到上次养成数据，会从零开始进行养成，这可能会导致行为不符合预期");
                Girl.Start();
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

        public void 标准群英联赛()
        {
            群英联赛流程();
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
                            if (养成流程())
                                break;

                            MoveTo([Symbol.养成, Button.主页], 0);
                            DeleteLog(2);
                            // 等待十分钟
                            Pause(10 * 60 * 1000);
                        }
                        if (t == count)
                            break;
                    }
                    else
                    {
                        if (养成流程())
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
            Click(Button.选卡);

            Queue<string> queue = new();
            queue.Enqueue("移到合适位置");
            queue.Enqueue("比对");
            queue.Enqueue("下移");
            queue.Enqueue("比对");

            // 定义刷新次数
            for (int s = 0; s < 10; s++)
            {
                queue.Enqueue("刷新");
                queue.Enqueue("比对");
            }

            while (queue.Count > 0)
            {
                switch (queue.Dequeue())
                {
                    case "移到合适位置":
                        Scroll([100, 1100, 80]);
                        Pause(200);
                        break;

                    case "下移":
                        Scroll([100, 1100, 750]);
                        Pause(1000);
                        break;

                    case "刷新":
                        Click(Button.刷新协助卡, 1000);
                        break;

                    case "比对":
                        Refresh();
                        for (int i = 1; i < 6; i++)
                        {
                            if (FastCheck(
                                FileManagerHelper.SetDir(CardDir).Find(card)!,
                                MaskScreen($"卡{i}", "card"))
                                )
                            {
                                Log($"已找到目标卡「{card}」");
                                Click($"卡{i}");
                                return true;
                            }
                        }
                        break;
                }
            }

            return false;
        }

        public bool 位置检测()
        {
            Start();
            Log("正在检测位置......");
            return WaitTo([Symbol.主界面, Button.主页], 0.8);
        }

        public void Start()
        {
            _stop = false;
        }

        public void Stop()
        {
            _stop = true;
            StopOverTimer();
        }
    }
}
