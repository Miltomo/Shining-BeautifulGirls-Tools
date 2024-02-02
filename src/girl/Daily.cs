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

        public void Start()
        {
            养成流程(养成过程Enum.转场处理);
        }

        public void Continue()
        {
            养成流程(养成过程Enum.转场处理);
        }

        public void ReadInfo()
        {
            Mnt.Refresh();

            // 刷新体力
            UpdateHP();

            // 获取心情
            UpdateMood();

            // 检查日期
            InSummer = FastCheck(Symbol.夏日);

            // 判断是否需要治疗
            InAilment = !(InSummer || IsDimmed(Zone.医务室, 160));

            // 读取当前属性值
            List<int> property = [];
            for (int i = 0; i < SubjectS.Count; i += 1)
                property.Add(ExtractValue(i switch
                {
                    0 => Zone.速度,
                    1 => Zone.耐力,
                    2 => Zone.力量,
                    3 => Zone.毅力,
                    4 => Zone.智力,
                    _ => throw new KeyNotFoundException()
                })); //GetPropertyValue(SubjectS[i])

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
        /// <returns></returns>
        public bool AtMainPage()
        {
            return FastCheck(Symbol.养成主页) || FastCheck(Symbol.比赛日主页);
        }

        /// <summary>
        /// (不刷新) 检测是否在赛事选择界面
        /// </summary>
        /// <returns></returns>
        public bool AtRacePage()
        {
            return FastCheck(Symbol.预测);
        }

        private void System__relex__()
        {
            _lastAction = "休息";
            //TODO 有BUG
            while (true)
            {
                if (FastCheck(Symbol.养成主页))
                    break;
                Click(Button.返回, 300);
            }

            Mnt.ClickEx(Button.休息, Symbol.休息确认, [Button.弹窗勾选, Button.弹窗确认]);
            Pause(1000);
        }

        private void System__out__()
        {
            //夏日判断
            if (InSummer)
            {
                System__relex__();
                return;
            }

            _lastAction = "外出";
            while (true)
            {
                if (FastCheck(Symbol.养成主页))
                    break;
                Click(Button.返回, 300);
            }

            Mnt.ClickEx(Button.外出, Symbol.外出确认, [Button.弹窗勾选, Button.弹窗确认]);
            Pause(1000);
        }

        private void System__treat__()
        {
            _lastAction = "治疗";
            Mnt.ClickEx(Button.医务室, Symbol.医务室确认, [Button.弹窗勾选, Button.弹窗确认]);
            Pause(1000);
        }
    }
}
