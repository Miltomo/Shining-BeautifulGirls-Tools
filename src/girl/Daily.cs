using System.Collections.Generic;
using static Shining_BeautifulGirls.World.NP;

namespace Shining_BeautifulGirls
{
    //TODO 比赛后选择第一个选项
    //TODO 保留每次养成的日志和成果记录
    //TODO 继续完善突发分支的处理
    //TODO 修改比赛逻辑
    partial class ShiningGirl
    {
        private bool InSummer { get; set; } = false;

        public void Start()
        {
            养成流程("普通日");
        }

        public void Continue()
        {
            养成流程("转场处理");
        }

        public void ReadInfo()
        {
            Mnt.Refresh();
            // 刷新体力
            Vitality = GetHP();
            Vitality = Vitality > 95 && _lastHP < 45 && _lastAction != "休息" ? 0 : Vitality;
            _lastHP = Vitality;

            // 获取心情
            Mood = GetMood();

            // 检查日期
            InSummer = Check("夏日");

            // 读取当前属性值
            List<int> property = [];
            for (int i = 0; i < SubjectS.Count; i += 1)
                property.Add(GetPropertyValue(SubjectS[i]));

            _hproperty ??= [.. property];

            for (int i = 0; i < property.Count; i += 1)
            {
                var dq = property[i];
                var hdq = _hproperty[i];

                // 识别值正常时，更新历史记录
                if (hdq - dq < 100) _hproperty[i] = dq;
            }
        }

        private void System__relex__()
        {
            _lastAction = "休息";
            while (true)
            {
                if (Check("养成主页"))
                    break;
                Click(Button.返回, 300);
            }

            if (!InSummer)
                Mnt.ClickEx(Button.医务室, "医务室确认", [Button.弹窗勾选, Button.弹窗确认]);

            Mnt.ClickEx(Button.休息, "休息确认", [Button.弹窗勾选, Button.弹窗确认]);
            Mnt.Pause(1000);
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
                if (Check("养成主页"))
                    break;
                Click(Button.返回, 300);
            }

            if (!InSummer)
                Mnt.ClickEx(Button.医务室, "医务室确认", [Button.弹窗勾选, Button.弹窗确认]);


            Mnt.ClickEx(Button.外出, "外出确认", [Button.弹窗勾选, Button.弹窗确认]);
            Mnt.Pause(1000);
        }
    }
}
