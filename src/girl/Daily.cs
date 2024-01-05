namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        public static string RecordDir { get; set; } = @"./record/";
        private bool InSummer { get; set; } = false;

        //TODO 比赛后选择第一个选项
        //TODO 保留每次养成的日志和成果记录
        //TODO 继续完善突发分支的处理
        //TODO 修改比赛逻辑
        public void BeginNewDay()
        {
            Update();
            Log($"###############第 {Turn} 回合###############");
            //进行比赛
            if (Check("比赛日主页"))
            {
                SkPoints = ExtractValue("技能点");

                Log($"★今天是比赛日★");
                Log($"技能点：{SkPoints}");

                if ((Turn > 35 || SkPoints > 500) && SkPoints > 150)
                {
                    if (Check("决赛"))
                        技能学习过程("最终学习");
                    else
                        技能学习过程("普通学习");
                }

                if (!比赛和结束处理("正常比赛"))
                    return;
            }
            //日常活动
            else
            {
                Log(基本信息);
                if (Check("医务室", sim: 0.8))
                {
                    Log("已受伤，进行治疗");
                    __treat__();
                }
                else if (Vitality < 26)
                {
                    Log("体力过低，放松休息");
                    __relax__();
                }
                else if (Mood < 3)
                {
                    Log("心情低落，选择外出");
                    __out__();
                }
                else
                {
                    Log("前往训练场地");
                    TryTrain();
                }
            }
            EndTheDay();
        }

        private void __relax__()
        {
            _lastAction = "休息";
            MoveTo("主页", ["养成主页", "返回"]);
            Mnt.ClickEx("休息", "休息确认", ["弹窗勾选", "弹窗确认"]);
            Mnt.Pause(1000);
        }

        private void __out__()
        {
            _lastAction = "外出";
            MoveTo("主页", ["养成主页", "返回"]);
            //夏日判断
            if (InSummer)
            {
                __relax__();
                return;
            }
            Mnt.ClickEx("外出", "外出确认", ["弹窗勾选", "弹窗确认"]);
            Mnt.Pause(1000);
        }

        private void __treat__()
        {
            _lastAction = "治疗";
            Click("医务室", 2000);
        }

        public void EndTheDay()
        {
            //Log("等待结算......");
            Location = "主页";
            //TODO 有概率长时间无响应，ADB连接问题？

            //选项处理
            if (Mnt.MoveToEx([["养成主页", "比赛日主页"], ["选择末尾"]],
                [
                    ["粉丝不足"],
                    ["未达要求"],
                    ["无法参赛"],
                    ["养成结束"],
                    ["继续", "比赛结束3"],
                    ["因子继承", "继续"],
                    ["抓娃娃", "比赛结束3"],
                    ["OK", "比赛结束1"]
                ], sec: 0))
                BeginNewDay();
            else
            {
                if (Check("养成结束"))
                {
                    Log("养成通关，完成所有比赛！");
                    比赛和结束处理("结束养成");
                }
                // 未达成参赛条件，需要额外比赛
                else
                {
                    比赛和结束处理("不满足参赛要求");
                    BeginNewDay();
                }
            }
        }

        public void Continue()
        {
            EndTheDay();
        }
    }
}
