namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 过程信息文本
    /// </summary>
    static class PText
    {
        /// <summary>
        /// 群英联赛
        /// </summary>
        public enum Extravaganza
        {
            参赛登记确认,
            决赛,
        }

        /// <summary>
        /// 团队竞技场
        /// </summary>
        public enum JJC
        {
            选择对战对手,
            竞技值不足,
        }

        /// <summary>
        /// 通用比赛
        /// </summary>
        public enum Race
        {
            日常赛事入场券不足,
            赛事详情,
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
            训练值,
            最终确认,
            回复训练值,
            跳过确认,

            // 结束
            养成结束确认,
            优俊少女详情,
        }
    }
}
