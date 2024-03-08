namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 过程信息文本
    /// </summary>
    static class PText
    {
        /// <summary>
        /// 主页
        /// </summary>
        public enum Main
        {
            养成,
            赛事,
            团队竞技场,
        }

        public enum Date
        {
            初级,
            经典级,
            高级,
            上,
            下,
        }


        /// <summary>
        /// 群英联赛
        /// </summary>
        public enum Extravaganza
        {
            参赛登记确认,
            决赛轮,
            限定任务,
            报名确认,
            是否确认执行,
            适应性,
            奖励,
            开始匹配,
            获得奖励,
        }

        /// <summary>
        /// 团队竞技场
        /// </summary>
        public enum JJC
        {
            选择对战对手,
            胜利时必得,
            竞技值不足,
        }

        /// <summary>
        /// 通用比赛
        /// </summary>
        public enum Race
        {
            日常赛事入场券不足,
            赛事推荐功能,
            多次参赛,
            连续参赛,
            赛事详情,
            选择道具,
            前往赛事,
            免费机会,
            重新挑战,
            下一页,
        }

        /// <summary>
        /// 养成
        /// </summary>
        public enum Cultivation
        {
            // 开始
            养成,
            继续养成,
            选择养成难度,
            协助卡编成,
            训练值,
            最终确认,
            缩短事件设置,
            回复训练值,
            跳过确认,

            // 途中
            休息确认,
            外出确认,
            事件进度,
            事件完成,
            医务室确认,
            技能获取,
            技能获取确认,
            暂无可以参加的比赛,
            预测,
            OK,
            因子继承,
            决赛,


            // 结束
            目标未达成,
            养成结束确认,
            优俊少女详情,
        }
    }
}
