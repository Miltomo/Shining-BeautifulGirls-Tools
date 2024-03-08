namespace Shining_BeautifulGirls
{
    //TODO 比赛后选择第一个选项
    //TODO 保留每次养成的日志和成果记录
    //TODO 继续完善突发分支的处理
    //TODO 修改比赛逻辑
    //TODO 增加友人卡外出分支
    partial class ShiningGirl
    {
        /// <summary>
        /// 指示当前是否为夏季特训时期
        /// </summary>
        private bool InSummer { get; set; } = false;

        /// <summary>
        /// 指示是否处于生病状态(需要治疗)
        /// </summary>
        private bool InAilment { get; set; } = false;

        /// <summary>
        /// 指示是否处于G1比赛日
        /// </summary>
        private bool InG1Day { get; set; } = false;

        /// <summary>
        /// 指示是否处于连续参赛状态
        /// </summary>
        private bool InContinuousRace { get; set; } = false;

        public void Start()
        {
            养成流程(养成过程Enum.转场处理);
        }

        public void Continue()
        {
            养成流程(养成过程Enum.转场处理);
        }

        public void UpdateBasicValue()
        {
            Mnt.Refresh();

            // 刷新体力
            UpdateHP();

            // 获取心情
            UpdateMood();

            // 检查日期
            Today = GetDate();
            InG1Day = G1TimeTable.Contains(Today);
            InSummer = FastCheck(Symbol.夏日);
            InContinuousRace &= LastAction == ActionEnum.比赛;

            // 判断是否需要治疗
            InAilment = !(InSummer || IsDimmed(ZButton.医务室, 160));

            // 读取当前属性值
            List<int> property = [];
            for (int i = 0; i < TrainingItems.Length; i += 1)
                property.Add(ExtractValue(i switch
                {
                    0 => Zone.速度,
                    1 => Zone.耐力,
                    2 => Zone.力量,
                    3 => Zone.毅力,
                    4 => Zone.智力,
                    _ => throw new KeyNotFoundException()
                }));

            _hproperty ??= [.. property];

            for (int i = 0; i < property.Count; i += 1)
            {
                var dq = property[i];
                var hdq = _hproperty[i];

                // 识别值正常时，更新历史记录
                if (hdq - dq < 100) _hproperty[i] = dq;
            }
        }

        /// <summary>
        /// (不刷新) 检测是否在主页（包括普通日和比赛日）
        /// </summary>
        public bool AtMainPage() => FastCheck(Symbol.普通日主页) || FastCheck(Symbol.比赛日主页);

        /// <summary>
        /// (不刷新) 检测是否在训练界面
        /// </summary>
        public bool AtTrainPage() => FastCheck(Symbol.返回, 0.8);

        /// <summary>
        /// (不刷新) 检测是否在赛事选择界面
        /// </summary>
        public bool AtRacePage() => Extract下部().Contains(PText.Cultivation.预测);

        /// <summary>
        /// (不刷新) 检测是否在养成结束页面
        /// </summary>
        public bool AtEndPage() => Extract上部().Equals(PText.Cultivation.养成结束确认);


        private bool GotoMainPage()
        {
            if (AtMainPage())
                return true;

            return MC.Builder
                .SetButtons(ZButton.返回)
                .AddTarget(AtMainPage)
                .StartAsWaitTo(Mnt);
        }


        /// <summary>
        /// 决定是否今天要参加赛事的总集程序
        /// </summary>
        /// <returns>true，成功进行了日常赛事；false，未进行此决断</returns>
        private bool 日常赛事决断()
        {
            if (Core.IsG1Consider && InG1Day)
            {
                Log("☆今天有G1比赛☆");
                //TODO 更改为只找G1比赛
                if (DailyRaceProcess(SelectFirstSuitableG1))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断今天是否直接跳过训练，进行放松类活动
        /// </summary>
        /// <returns>true，确定进行了放松类活动；false，未进行此决断</returns>
        private bool 直接决断()
        {
            bool Aw = true;
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
                Aw = false;

            return Aw;
        }

        /// <summary>
        /// 前往训练场地，并根据训练信息进行下一步决断
        /// </summary>
        /// <returns>true，进行了最终决断；false，<b>未成功前往训练场地</b></returns>
        private bool 训练决断()
        {
            if (获取训练信息())
            {
                var orin = Core.PlanToDo();
                if (orin.Plan == PlanEnum.比赛 && !Core.CheckRaceEnvironment())
                    StartPlan(Core.WhenNoRace());
                else
                    StartPlan(orin);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 进行日常比赛的处理程序。如果未进行比赛，会自动回到<b>主页面</b>
        /// </summary>
        /// <param name="findinglogic"></param>
        /// <returns>true，找到了比赛并进行；false，当前不适合参加比赛|未找到合适比赛</returns>
        private bool DailyRaceProcess(Func<bool> findinglogic)
        {
            if (Core.CheckRaceEnvironment())
            {
                if (findinglogic())
                {
                    Log($"已找到合适比赛：");
                    Log(TargetRace!);
                    比赛处理(比赛过程Enum.进入);
                    return true;
                }
                else
                {
                    Log($"未找到合适比赛");
                }
            }
            else
                Log($"赛事未开启或当前不宜参加赛事");
            GotoMainPage();
            return false;
        }


        private void System__bt_clicker__(Enum bt)
        {
            MC.Builder
                .AddProcess(() =>
                {
                    var r = Extract中部();
                    if (r.FirstIn([
                        PText.Cultivation.休息确认,
                        PText.Cultivation.外出确认,
                        PText.Cultivation.医务室确认
                    ]) is not null)
                        Mnt.Click([Button.弹窗勾选, Button.弹窗确认]);
                })
                .SetButtons(bt)
                .StartAsClickEx(Mnt);
        }

        private void System__relex__()
        {
            LastAction = ActionEnum.休息;
            while (true)
            {
                if (FastCheck(Symbol.普通日主页))
                    break;
                Click(ZButton.返回, 300);
            }


            System__bt_clicker__(Button.休息);
        }

        private void System__out__()
        {
            //夏日判断
            if (InSummer)
            {
                System__relex__();
                return;
            }

            LastAction = ActionEnum.外出;
            while (true)
            {
                if (FastCheck(Symbol.普通日主页))
                    break;
                Click(ZButton.返回, 300);
            }

            System__bt_clicker__(Button.外出);
            var r = ExtractInfo(ZButton.外出1);
            if (r.Contains(PText.Cultivation.事件进度))
            {
                if (r.Contains(PText.Cultivation.事件完成))
                    Click(ZButton.外出2);
                else
                    Click(ZButton.外出1);
            }
        }

        private void System__treat__()
        {
            LastAction = ActionEnum.治疗;
            System__bt_clicker__(ZButton.医务室);
        }


        enum ActionEnum
        {
            休息,
            外出,
            治疗,
            比赛,
            训练,
        }
    }
}
