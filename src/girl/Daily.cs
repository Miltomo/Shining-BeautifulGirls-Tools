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
        public bool AtMainPage() => FastCheck(Symbol.养成主页) || FastCheck(Symbol.比赛日主页);

        /// <summary>
        /// (不刷新) 检测是否在赛事选择界面
        /// </summary>
        public bool AtRacePage() => FastCheck(Symbol.预测);

        /// <summary>
        /// (不刷新) 检测是否在养成结束页面
        /// </summary>
        public bool AtEndPage() => Extract上部().Equals(PText.Cultivation.养成结束确认);

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
            _lastAction = "休息";
            while (true)
            {
                if (FastCheck(Symbol.养成主页))
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

            _lastAction = "外出";
            while (true)
            {
                if (FastCheck(Symbol.养成主页))
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
            _lastAction = "治疗";
            System__bt_clicker__(ZButton.医务室);
        }
    }
}
