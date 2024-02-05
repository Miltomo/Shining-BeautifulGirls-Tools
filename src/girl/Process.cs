using ComputerVision;
using MHTools;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        private bool _havefree = false;
        private int _dqRemakeTimes = 0;

        //TODO 比赛算法可以考虑实施了

        private enum 养成过程Enum
        {
            转场处理,
            普通日,
            比赛日,
            每日总结,
            结束,
        }

        private void 养成流程(养成过程Enum stage)
        {
            //Stage = stage;
            switch (stage)
            {
                //TODO 使用Task异步
                // 控制中心
                case 养成过程Enum.转场处理:
                    Click(Button.选择末尾, 20);
                    IOCRResult zb, xb;

                    /*FastCheck(Symbol.粉丝不足) || FastCheck(Symbol.未达要求) || FastCheck(Symbol.无法参赛)*/
                    if ((zb = Extract中部(), zb.Contains(PText.Race.前往赛事)).Item2)
                    {
                        if (比赛处理(比赛过程Enum.提醒比赛))
                            养成流程(养成过程Enum.每日总结);
                        // 没有合适比赛的情况
                        else
                        {
                            MoveTo(AtMainPage, ZButton.返回);
                            养成流程(养成过程Enum.普通日);
                        }
                        break;
                    }
                    else if (FastCheck(Symbol.继续) || FastCheck(Symbol.抓娃娃))
                    {
                        Click(Button.低继续);
                    }
                    /*FastCheck(Symbol.OK)*/
                    else if ((xb = Extract下部(), xb.Contains(PText.Cultivation.OK)).Item2)
                    {
                        Click(Button.比赛结束);
                    }
                    else if (xb.Contains(PText.Cultivation.因子继承))
                    {
                        Click(Button.继续);
                    }
                    else if (AtEndPage())
                    {
                        养成流程(养成过程Enum.结束);
                        break;
                    }

                    Mnt.Refresh();
                    if (FastCheck(Symbol.养成主页))
                    {
                        养成流程(养成过程Enum.普通日);
                        break;
                    }
                    else if (FastCheck(Symbol.比赛日主页))
                    {
                        养成流程(养成过程Enum.比赛日);
                        break;
                    }


                    养成流程(养成过程Enum.转场处理);
                    break;

                case 养成过程Enum.普通日:
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
                            养成流程(养成过程Enum.转场处理);
                            break;
                        }
                    }

                    养成流程(养成过程Enum.每日总结);
                    break;

                case 养成过程Enum.比赛日:
                    Log(回合开始);

                    //TODO 使用文字识别替换，同时检查技能点和是否是决赛
                    SkPoints = ExtractValue(Zone.技能点);

                    Log($"★今天是比赛日★");
                    Log($"技能点：{SkPoints}");

                    if ((Turn > 35 || SkPoints > 500) && SkPoints > 150)
                    {
                        if (FastCheck(Symbol.决赛))
                            技能学习过程("最终学习");
                        else
                            技能学习过程("普通学习");
                    }

                    if (比赛处理(比赛过程Enum.目标比赛))
                        养成流程(养成过程Enum.每日总结);
                    else
                        养成流程(养成过程Enum.结束);
                    break;

                case 养成过程Enum.每日总结:
                    Turn += 1;
                    养成流程(养成过程Enum.转场处理);
                    break;

                case 养成过程Enum.结束:
                    Log("养成已结束");

                    MoveTo(() =>
                    {
                        var r = Extract上部();
                        if (r.Contains(PText.Cultivation.养成结束确认))
                        {
                            Log($"获得粉丝 {GetFans(r)} 人");
                            return true;
                        }
                        return false;
                    }, Button.比赛结束, 0);

                    技能学习过程("结束学习");

                    Click(Button.养成结束);
                    PageDown(Zone.中部, PText.Cultivation.养成结束确认, Button.大弹窗确认);


                    var dir = GetTodayRecordDir();
                    var name = FileManagerHelper.SetDir(dir).NextName();

                    PageDown([Symbol.下一页, Button.结束连点]);
                    PageDown([Symbol.下一页]);
                    Mnt.SaveScreen(dir, $"{name}_因子");
                    Log("已保存因子信息截图");
                    Click(Button.结束连点);
                    PageDown(Zone.上部, PText.Cultivation.优俊少女详情);
                    Mnt.SaveScreen(dir, name);
                    Log("已保存养成信息截图");

                    MoveTo([Symbol.主界面, Button.结束连点], sec: 0, sim: 0.7);

                    EndTraining = true;
                    break;

                default:
                    throw new NotImplementedException($"养成流程不存在此过程:{stage}");
            }
        }


        private enum 比赛过程Enum
        {
            目标比赛,
            提醒比赛,
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
        private bool 比赛处理(比赛过程Enum p)
        {
            switch (p)
            {
                case 比赛过程Enum.目标比赛:
                    // 从主界面进入比赛界面
                    GotoRacePage();
                    return 比赛处理(比赛过程Enum.进入);

                case 比赛过程Enum.提醒比赛:
                    // 通过提醒弹窗进入比赛界面
                    GotoRacePage();
                    // 选择适合自己的比赛
                    if (SelectFirstSuitableRace())
                    {
                        Log(回合开始);
                        Log("选择比赛:");
                        Log($"{"",5}{TargetRace}");
                        return 比赛处理(比赛过程Enum.进入);
                    }
                    return false;

                case 比赛过程Enum.进入:
                    _lastAction = "比赛";
                    _dqRemakeTimes = 0;
                    UpdateHP();

                    Log("参加比赛");
                    Click(ZButton.通用参赛, 0);
                    PageDown(Zone.中部, PText.Race.赛事详情, Button.大弹窗确认);
                    return 比赛处理(比赛过程Enum.新比赛);

                case 比赛过程Enum.新比赛:
                    PageDown(Zone.下部, PText.Race.前往赛事);
                    Pause();
                    var bd = MC.Builder
                        .AddTarget(Symbol.下一页)
                        .AddOpposite(() =>
                        {
                            var zb = Extract中部();
                            if (zb.Equals(PText.Race.重新挑战))
                            {
                                _havefree = zb.Contains(PText.Race.免费机会);
                                return true;
                            }
                            return false;
                        });

                    if (IsDimmed(ZButton.查看结果, 160))
                    {
                        MoveTo([Symbol.快进, Button.比赛结束]);
                        bd.SetButtons(Button.比赛连点);
                    }
                    else
                    {
                        Click(ZButton.查看结果, 1000);
                        bd.SetButtons(Button.继续);
                    }

                    if (bd.StartAsMoveTo(Mnt))
                        return 比赛处理(比赛过程Enum.比赛成功);
                    return 比赛处理(比赛过程Enum.重赛处理);

                case 比赛过程Enum.重赛处理:
                    Log("比赛未取得良好结果，等待重新挑战......");

                    bool A = _havefree;

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
                    PageDown(Zone.上部, PText.Cultivation.技能获取);
                    技能学习过程("学习");
                    break;

                case "学习":
                    //
                    //Mnt.SaveScreen();
                    //
                    //TODO 使用Task异步
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

                            Match(out OpenCvSharp.Point pt, Symbol.技能加, mask);
                            Mnt.Click(pt.X + 20, pt.Y + 20, pauseTime: 300);

                            // 二次学习检测
                            if (!IsHadSkill(CropScreen(zone)) && (SkPoints >= 2 * theLearnCost))
                                Mnt.Click(pt.X + 20, pt.Y + 20, pauseTime: 200);

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
                //TODO 使用文字检测重构 => 检测最后一个技能的名字
                case "翻页":
                    if (Mnt.Match(Symbol.技白, CropScreen(Zone.技白)) > 0.9)
                        技能学习过程("结束");
                    else
                    {
                        SkillScroll(430);
                        技能学习过程("学习");
                    }
                    break;

                case "结束":
                    MC.Builder
                        .AddProcess(Zone.上部, PText.Cultivation.技能获取确认, Button.技能获取)
                        .SetButtons(Button.继续)
                        .StartAsClickEx(Mnt);

                    while (true)
                    {
                        Click(ZButton.返回);
                        if (FastCheck(Symbol.比赛日主页, 0.7) || AtEndPage())
                            break;
                    }

                    Log("结束技能学习");
                    break;
            }
        }
    }
}
