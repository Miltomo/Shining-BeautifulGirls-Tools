using MHTools;
using System;
using static Shining_BeautifulGirls.World.NP;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        private int _dqRemakeTimes = 0;

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
                        比赛处理("不满足参赛要求");
                        养成流程("每日总结");
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
                    Log($"###############第 {Turn} 回合###############");

                    ReadInfo();
                    Log(基本信息);

                    // 判断要去训练还是干别的
                    if (Vitality < 26)
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
                    Log($"###############第 {Turn} 回合###############");

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

                    if (比赛处理("正常比赛"))
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

        /// <summary>
        /// 比赛的过程处理
        /// </summary>
        /// <param name="state"></param>
        /// <returns><b>true</b>,比赛成功; <b>false</b>,比赛失败</returns>
        private bool 比赛处理(string state)
        {
            Mnt.Refresh();
            switch (state)
            {
                case "正常比赛":
                    Click(Button.参赛, 1000);
                    return 比赛处理("参赛");
                case "不满足参赛要求":
                    Click(Button.大弹窗确认, 1000);
                    return 比赛处理("参赛");
                case "参赛":
                    _lastAction = "比赛";
                    _dqRemakeTimes = 0;
                    if (Check(Symbol.连续参赛))
                        Click(Button.弹窗确认, 1000);
                    if (Check(Symbol.赛事推荐弹窗, sim: 0.8))
                    {
                        Click(Button.不弹赛事推荐);
                        Click(Button.比赛结束);
                    }
                    Log("参加比赛");
                    Click(Button.参赛);
                    PageDown([Symbol.参赛确认, Button.大弹窗确认]);

                StartNewRace:
                    PageDown([Symbol.前往赛事]);
                    if (Match(Symbol.查看结果) < 0.99)
                    {
                        Click(Button.前往赛事);
                        PageDown([Symbol.参赛, Button.比赛结束]);
                        Mnt.MoveToEx([[Symbol.下一页], [Button.比赛快进]], [
                            [Symbol.重新挑战]
                            ], sec: 0);
                    }
                    else
                    {
                        Click(Button.查看结果);
                        Click(Button.查看结果);

                        if (Mnt.MoveToEx([[Symbol.下一页], [Button.继续]],
                            [[Symbol.重新挑战]
                            ], sec: 0))
                        {
                            Click(Button.比赛结束);
                            PageDown([Symbol.赛果, Button.比赛结束]);

                            Log("比赛结束，已达成目标");
                            return true;
                        }
                        // 比赛失败
                        else
                        {
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
                                goto StartNewRace;
                            }
                            Log("放弃重新挑战");
                        }
                    }
                    return 比赛处理("比赛失败");
                case "比赛失败":
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
                    var orin = ExtractValue(Zone.技能点2);
                    for (int i = 1; i < 4; i++)
                    {
                        var mask = MaskScreen($"技{i}");

                        // 加速
                        if (Check(Symbol.已获得, mask, 0.8))
                            continue;

                        if (IsNecessarySkill(mask))
                        {
                            Match(out OpenCvSharp.Point pt, Symbol.技能加, mask);
                            for (int k = 0; k < 2; k++)
                                Mnt.Click(pt.X + 20, pt.Y + 20, 200);
                            Mnt.Refresh();
                            var ch = ExtractValue(Zone.技能点2);
                            if (ch != orin)
                            {
                                orin = ch;
                                Log($"学习技能「{LearnName}」");
                                SkList.Remove(LearnName!);
                            }
                        }
                    }
                    技能学习过程("翻页");
                    break;
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
