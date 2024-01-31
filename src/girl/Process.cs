using MHTools;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {


        private int _dqRemakeTimes = 0;

        //TODO 重构是否需要比赛的判断，可进行提前判断了。
        //TODO 重构对技能的判断: 大改 + 重构

        //TODO 比赛算法可以考虑实施了
        private void 养成流程(string stage)
        {
            Stage = stage;
            switch (stage)
            {
                // 控制中心
                case "转场处理":
                    Mnt.Refresh();

                    if (FastCheck(Symbol.养成主页))
                    {
                        养成流程("普通日");
                        break;
                    }
                    else if (FastCheck(Symbol.比赛日主页))
                    {
                        养成流程("比赛日");
                        break;
                    }
                    else if (FastCheck(Symbol.粉丝不足) || FastCheck(Symbol.未达要求) || FastCheck(Symbol.无法参赛))
                    {
                        if (比赛处理(比赛过程Enum.额外比赛))
                            养成流程("每日总结");
                        // 没有合适比赛的情况
                        else
                        {
                            MoveTo(AtMainPage, Button.返回);
                            养成流程("普通日");
                        }
                        break;
                    }
                    else if (FastCheck(Symbol.继续) || FastCheck(Symbol.抓娃娃))
                    {
                        Click(Button.低继续);
                    }
                    else if (FastCheck(Symbol.OK))
                    {
                        Click(Button.比赛结束);
                    }
                    else if (FastCheck(Symbol.因子继承))
                    {
                        Click(Button.继续);
                    }
                    else if (FastCheck(Symbol.养成结束))
                    {
                        养成流程("结束");
                        break;
                    }

                    Click(Button.选择末尾, 300);
                    养成流程("转场处理");
                    break;

                case "普通日":
                    Log(回合开始);

                    ReadInfo();
                    Log(基本信息);

                    // 判断要去训练还是干别的
                    if (InAilment)
                    {
                        Log("已受伤，进行治疗");
                        System__treat__();
                    }
                    else if (Vitality < 26)
                    {
                        Log("体力过低，放松休息");
                        System__relex__();
                    }
                    else if (Mood < 3)
                    {
                        Log("心情低落，选择外出");
                        System__out__();
                    }
                    else
                    {
                        Log("前往训练场地");
                        if (!TryTrain())
                        {
                            养成流程("转场处理");
                            break;
                        }
                    }

                    养成流程("每日总结");
                    break;

                case "比赛日":
                    Log(回合开始);

                    UpdateHP();
                    SkPoints = ExtractValue(Zone.技能点);

                    Log($"★今天是比赛日★");
                    Log($"技能点：{SkPoints}");

                    if ((Turn > 35 || SkPoints > 500) && SkPoints > 150)
                    {
                        if (Check(Symbol.决赛))
                            技能学习过程("最终学习");
                        else
                            技能学习过程("普通学习");
                    }

                    if (比赛处理(比赛过程Enum.目标比赛))
                        养成流程("每日总结");
                    else
                        养成流程("结束");
                    break;

                case "每日总结":
                    Turn += 1;
                    养成流程("转场处理");
                    break;

                case "结束":
                    技能学习过程("结束学习");
                    Log("结束养成");
                    Click(Button.养成结束);
                    PageDown([Symbol.养成结束弹窗, Button.大弹窗确认]);
                    EndTraining = true;

                    var dir = GetTodayRecordDir();
                    var name = FileManagerHelper.SetDir(dir).NextName();

                    PageDown([Symbol.下一页, Button.结束连点]);
                    PageDown([Symbol.下一页]);
                    Mnt.SaveScreen(dir, $"{name}_因子");
                    Log("已保存因子信息截图");
                    Click(Button.结束连点, 1000);
                    PageDown([Symbol.优俊少女详情]);
                    Mnt.SaveScreen(dir, name);
                    Log("已保存养成信息截图");

                    MoveTo([Symbol.主界面, Button.结束连点], sec: 0, sim: 0.7);
                    break;

                default:
                    throw new NotImplementedException($"养成流程不存在此过程:{stage}");
            }
        }


        private enum 比赛过程Enum
        {
            目标比赛,
            额外比赛,
            进入,
            新比赛,
            重赛处理,
            比赛成功,
            比赛失败,
        }

        /// <summary>
        /// 比赛的过程处理
        /// </summary>
        /// <param name="state"></param>
        /// <returns><b>true</b>,比赛成功; <b>false</b>,比赛失败或未进行比赛</returns>
        private bool 比赛处理(比赛过程Enum e)
        {
            Mnt.Refresh();
            switch (e)
            {
                case 比赛过程Enum.目标比赛:
                    GotoRacePage();
                    return 比赛处理(比赛过程Enum.进入);

                case 比赛过程Enum.额外比赛:
                    MoveTo(AtRacePage, Button.大弹窗确认);

                    if (SelectFirstSuitableRace())
                    {
                        UpdateHP();
                        Log(回合开始);
                        Log("选择比赛:");
                        Log($"{"",5}{TargetRace}");
                        return 比赛处理(比赛过程Enum.进入);
                    }
                    return false;

                case 比赛过程Enum.进入:
                    _lastAction = "比赛";
                    _dqRemakeTimes = 0;
                    Log("参加比赛");
                    Click(ZButton.通用参赛);
                    PageDown(Zone.中部, PText.Race.赛事详情, Button.大弹窗确认);
                    return 比赛处理(比赛过程Enum.新比赛);

                //TODO 测试
                case 比赛过程Enum.新比赛:
                    PageDown(Zone.下部, PText.Race.前往赛事);
                    Mnt.Pause(500);
                    if (IsDimmed(ZButton.查看结果, 160))
                    {
                        MoveTo([Symbol.快进, Button.比赛结束]);

                        while (true)
                        {
                            Click(Button.比赛连点, 0);
                            Mnt.Refresh();

                            var zb = Extract中部();
                            if (zb.Equals(PText.Race.重新挑战))
                            {
                                //相关处理...
                                return 比赛处理(比赛过程Enum.重赛处理);
                            }
                            else if (IsZoneContains(Zone.下部, PText.Race.下一页))
                                return 比赛处理(比赛过程Enum.比赛成功);
                        }
                    }
                    else
                    {
                        Click(ZButton.查看结果, 1000);
                        while (true)
                        {
                            Click(Button.继续, 0);
                            Mnt.Refresh();

                            var zb = Extract中部();
                            if (zb.Equals(PText.Race.重新挑战))
                            {
                                //相关处理...
                                return 比赛处理(比赛过程Enum.重赛处理);
                            }
                            else if (IsZoneContains(Zone.下部, PText.Race.下一页))
                                return 比赛处理(比赛过程Enum.比赛成功);
                        }
                    }

                case 比赛过程Enum.重赛处理:
                    bool A = false;

                    Log("比赛未取得良好结果，等待重新挑战......");
                    if (Check(Symbol.免费闹钟))
                        A = true;
                    if (!A && UserConfig is not null)
                    {
                        switch (UserConfig.ReChallenge)
                        {
                            // 每场比赛一次
                            case 1:
                                if (_dqRemakeTimes < 1)
                                    A = true;
                                break;
                            // 一直重赛
                            case 2:
                                A = true;
                                break;
                            default: break;
                        }
                    }
                    if (A)
                    {
                        _dqRemakeTimes++;
                        Log($"选择重新挑战，这是第 {_dqRemakeTimes} 次重新挑战");
                        Click(Button.大弹窗确认);
                        return 比赛处理(比赛过程Enum.新比赛);
                    }
                    Log("放弃重新挑战");
                    return 比赛处理(比赛过程Enum.比赛失败);

                case 比赛过程Enum.比赛成功:
                    Click(Button.比赛结束);
                    PageDown([Symbol.赛果, Button.比赛结束]);

                    Log("比赛结束，已达成目标");
                    return true;

                case 比赛过程Enum.比赛失败:
                    Log("比赛失败");
                    Click(Button.结束养成);
                    MoveTo([Symbol.养成结束, Button.比赛结束], 0);
                    return false;

                default:
                    return false;
            }
        }

        private void 技能学习过程(string state)
        {
            switch (state)
            {
                case "普通学习":
                    SkTurns++;
                    UpdateLearningList();
                    Log($"=====>准备进行第 {SkTurns} 次技能学习<=====");
                    技能学习过程("进入");
                    break;

                case "最终学习":
                    while (SkIndex < 3)
                        SkList.AddRange(PrioritySkillList[SkIndex++]);
                    Log($"=====>准备进行最后一次技能学习<=====");
                    技能学习过程("进入");
                    break;

                case "结束学习":
                    while (SkIndex < 4)
                        SkList.AddRange(PrioritySkillList[SkIndex++]);
                    Log($"=====>开始进行养成结束的技能学习<=====");
                    技能学习过程("进入");
                    break;

                case "进入":
                    Click(Button.技能);
                    PageDown([Symbol.技能获取]);
                    技能学习过程("学习");
                    break;

                case "学习":
                    //
                    //Mnt.SaveScreen();
                    //
                    for (int i = 1; i < 4; i++)
                    {
                        Zone zone = i switch
                        {
                            1 => Zone.技1,
                            2 => Zone.技2,
                            3 => Zone.技3,
                            _ => throw new NotImplementedException()
                        };

                        bool Aw = false;

                        var mask = MaskScreen(zone);

                        // 加速：若已获得则跳过
                        if (IsHadSkill(mask))
                            continue;

                        // 处理
                        if (IsNecessarySkill(zone))
                        {
                            // 加速：若不够学习则跳过
                            if (theLearnCost > SkPoints)
                                continue;

                            Aw = true;
                            //TODO 测试

                            Match(out OpenCvSharp.Point pt, Symbol.技能加, mask);
                            Mnt.Click(pt.X + 20, pt.Y + 20, pauseTime: 300);
                            Mnt.Refresh();

                            // 增加额外判断
                            // 学完后变为「已获得」？还是能继续点？
                            // 需要立即继续点吗？

                            // 二次学习检测
                            if (!IsHadSkill(CropScreen(zone)) && (SkPoints >= 2 * theLearnCost))
                            {
                                Mnt.Click(pt.X + 20, pt.Y + 20, pauseTime: 200);
                                Mnt.Refresh();
                            }

                            if (Aw)
                            {
                                Log($"学习技能「{theLearnName}」");
                                if (IsHadSkill(CropScreen(zone)))
                                    SkList.Remove(theLearnName!);
                            }
                        }

                        // 更新技能点
                        // 加速：只在变化时更新
                        if (Aw) SkPoints = ExtractValue(Zone.技能点2);

                        // 加速：若技能点过小则结束
                        if (SkPoints < 71)
                        {
                            技能学习过程("结束");
                            return;
                        }
                    }//for

                    技能学习过程("翻页");
                    break;
                //TODO 考虑重构
                case "翻页":
                    if (Match(Symbol.技白, CropScreen(Zone.技白)) > 0.9)
                        技能学习过程("结束");
                    else
                    {
                        SkillScroll(445);
                        技能学习过程("学习");
                    }
                    break;

                case "结束":
                    Mnt.ClickEx(Button.继续, Symbol.技能获取确认, [Button.技能获取]);

                    while (true)
                    {
                        Click(Button.返回);
                        if (Match(Symbol.比赛日主页) > 0.7 || Match(Symbol.养成结束) > 0.8)
                            break;
                    }

                    Log("结束技能学习");
                    break;
            }
        }
    }
}
