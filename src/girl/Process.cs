using MHTools;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        private int _dqRemakeTimes = 0;

        private void 养成流程(string stage)
        {
            switch (stage)
            {
                case "转场处理":
                    //TODO 改为控制中心语法
                    var rs = Mnt.MoveToEx([["养成主页", "比赛日主页"], ["选择末尾"]], [
                        ["粉丝不足"],
                        ["未达要求"],
                        ["无法参赛"],
                        ["养成结束"],
                        ["继续", "比赛结束3"],
                        ["因子继承", "继续"],
                        ["抓娃娃", "比赛结束3"],
                        ["OK", "比赛结束1"]
                        ],
                        sec: 0);
                    if (rs == true)
                    {
                        //新的一天...
                    }
                    else
                    {
                        //要去比赛或者特殊处理...
                    }
                    break;
                case "普通日":
                    // 读取基本值
                    // 判断要去训练还是干别的
                    break;
                case "比赛日":
                    break;
                case "每日总结":
                    // 在此处进行回合数增加操作
                    // 跳到 => 转场处理
                    break;
                case "结束":

                    break;
            }
        }

        /// <summary>
        /// 比赛的过程处理
        /// </summary>
        /// <param name="state"></param>
        /// <returns><b>true</b>,继续游戏; <b>false</b>,养成结束</returns>
        private bool 比赛和结束处理(string state)
        {
            Mnt.Refresh();
            switch (state)
            {
                case "正常比赛":
                    Click("参赛", 1000);//比赛日主页的“赛事”按键
                    return 比赛和结束处理("参赛");
                case "不满足参赛要求":
                    Click("大弹窗确认", 1000);
                    return 比赛和结束处理("参赛");
                case "参赛":
                    _lastAction = "比赛";
                    _dqRemakeTimes = 0;
                    if (Check("连续参赛"))
                        Click("弹窗确认", 1000);
                    //TODO 测试
                    if (Check("赛事推荐弹窗"))
                    {
                        Click("不弹赛事推荐");
                        Click("比赛结束1");
                    }
                    Log("参加比赛");
                    Click("参赛");
                    PageDown(["参赛确认", "大弹窗确认"]);

                StartNewRace:
                    PageDown(["前往赛事"]);
                    if (Match("查看结果") < 0.99)
                    {
                        Click("前往赛事");
                        PageDown(["参赛!", "比赛结束1"]);
                        Mnt.MoveToEx([["下一页"], ["比赛快进"]], [
                            ["重新挑战"]
                            ], sec: 0);
                    }
                    else
                    {
                        Click("查看结果");
                        Click("查看结果");

                        if (Mnt.MoveToEx([["下一页"], ["继续"]],
                            [["重新挑战"]
                            ], sec: 0))
                        {
                            Click("比赛结束2");
                            PageDown(["赛果", "比赛结束2"]);

                            Log("比赛结束，已达成目标");
                            return true;
                        }
                        // 比赛失败
                        else
                        {
                            bool A = false;

                            Log("比赛未取得良好结果，等待重新挑战......");
                            if (Check("免费闹钟"))
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
                                Click("大弹窗确认");
                                goto StartNewRace;
                            }
                            Log("放弃重新挑战");
                        }
                    }
                    return 比赛和结束处理("比赛失败");
                case "比赛失败":
                    Log("比赛失败");
                    Click("结束养成");
                    MoveTo(["养成结束", "比赛结束2"], 0);
                    return 比赛和结束处理("结束养成");
                case "结束养成":
                    技能学习过程("结束学习");
                    Log("结束养成");
                    Click("养成结束");
                    PageDown(["养成结束弹窗", "大弹窗确认"]);
                    EndTraining = true;

                    var dir = GetTodayRecordDir();
                    var name = FileManagerHelper.SetDir(dir).NextName();

                    PageDown(["下一页", "结束连点"]);
                    PageDown(["下一页"]);
                    Mnt.SaveScreen(dir, $"{name}_因子");
                    Log("已保存因子信息截图");
                    Click("结束连点", 1000);
                    PageDown(["优俊少女详情"]);
                    Mnt.SaveScreen(dir, name);
                    Log("已保存养成信息截图");

                    MoveTo(["主界面", "结束连点"], sec: 0, sim: 0.7);
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
                    Click("技能");
                    PageDown(["技能获取"]);
                    技能学习过程("学习");
                    break;
                case "学习":
                    //
                    //Mnt.SaveScreen();
                    //
                    var orin = ExtractValue("技能点2");
                    for (int i = 1; i < 4; i++)
                    {
                        var mask = MaskScreen($"技{i}");

                        //加速
                        if (Check("已获得", mask, 0.8))
                            continue;

                        if (IsNecessarySkill(mask))
                        {
                            Match(out OpenCvSharp.Point pt, "技能+", mask);
                            Mnt.Click(pt.X + 20, pt.Y + 20, 1);
                            Mnt.Refresh();
                            var ch = ExtractValue("技能点2");
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
                    if (Match("技白", CropScreen("技白")) > 0.9)
                        技能学习过程("结束");
                    else
                    {
                        SkillScroll(445);
                        技能学习过程("学习");
                    }
                    break;
                case "结束":
                    ClickEx("继续", "技能获取确认", ["技能获取"]);

                    while (true)
                    {
                        Click("返回");
                        if (Match("比赛日主页") > 0.7 || Match("养成结束") > 0.8)
                            break;
                    }

                    Log("结束技能学习");
                    break;
            }
        }
    }
}
