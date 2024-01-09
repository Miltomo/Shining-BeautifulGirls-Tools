namespace Shining_BeautifulGirls
{
    //TODO 比赛后选择第一个选项
    //TODO 保留每次养成的日志和成果记录
    //TODO 继续完善突发分支的处理
    //TODO 修改比赛逻辑
    partial class ShiningGirl
    {
        public static string RecordDir { get; set; } = @"./record/";
        private bool InSummer { get; set; } = false;

        public void Start()
        {
            养成流程("普通日");
        }

        public void Continue()
        {
            养成流程("转场处理");
        }

        private void System__relex__()
        {
            _lastAction = "休息";
            MoveTo("主页", ["养成主页", "返回"]);
            Mnt.ClickEx("休息", "休息确认", ["弹窗勾选", "弹窗确认"]);
            Mnt.Pause(1000);
        }

        private void System__out__()
        {
            _lastAction = "外出";
            MoveTo("主页", ["养成主页", "返回"]);
            //夏日判断
            if (InSummer)
            {
                System__relex__();
                return;
            }
            Mnt.ClickEx("外出", "外出确认", ["弹窗勾选", "弹窗确认"]);
            Mnt.Pause(1000);
        }

        private void System__treat__()
        {
            _lastAction = "治疗";
            Mnt.ClickEx("医务室", "医务室确认", ["弹窗勾选", "弹窗确认"]);
            Mnt.Pause(1000);
        }
    }
}
