using System.Collections.Generic;
using static Shining_BeautifulGirls.World.NP;

namespace Shining_BeautifulGirls
{
    partial class World
    {
        public static class NP
        {
            public enum Button
            {
                强化编成,
                剧情,
                主页,
                赛事,
                JJC1,
                JJC2,
                第1队,
                第2队,
                第3队,
                JJC3,
                JJC4,
                竞技场连点,
                日常赛事,
                日1,
                日2,
                日3,
                取消,
                扭蛋,
                养成,
                继续,
                大弹窗确认,
                优先活动卡,
                自动编成确认,
                使用宝石,
                使用能量饮料,
                加号,
                刷新协助卡,
                开始养成,
                快进,
                弹窗勾选,
                弹窗确认,
                跳过,
                缩短所有事件,
                缩短事件确定,
                休息,
                训练,
                技能,
                医务室,
                外出,
                返回,
                速度,
                耐力,
                力量,
                毅力,
                智力,
                参赛,
                查看结果,
                前往赛事,
                比赛快进,
                比赛结束,
                低继续,
                选择倒二,
                选择末尾,
                选卡,
                卡1,
                卡2,
                卡3,
                卡4,
                卡5,
                技能获取,
                结束养成,
                养成结束,
                结束连点,
                不弹赛事推荐,
            }
            public enum Zone
            {
                事件类型,
                事件名,
                日历,
                心情,
                体力,
                协助1,
                协助2,
                协助3,
                协助4,
                协助5,
                速度增加,
                耐力增加,
                力量增加,
                毅力增加,
                智力增加,
                速度,
                耐力,
                力量,
                毅力,
                智力,
                第1队,
                第2队,
                第3队,
                卡1,
                卡2,
                卡3,
                卡4,
                卡5,
                技1,
                技2,
                技3,
                技白,
                技能点,
                技能点2,
            }
        }

        public static void CreateLocInfo()
        {
            ButtonLocation.Clear();
            ZoneLocation.Clear();

            MakeButton(Button.强化编成, 75, 1225);
            MakeButton(Button.剧情, 190, 1225);
            MakeButton(Button.主页, 360, 1225);
            MakeButton(Button.赛事, 520, 1225);
            MakeButton(Button.JJC1, 208, 864);
            MakeButton(Button.JJC2, 360, 810);
            MakeButton(Button.第1队, 360, 350);
            MakeButton(Button.第2队, 360, 620);
            MakeButton(Button.第3队, 360, 890);
            MakeButton(Button.JJC3, 510, 913);
            MakeButton(Button.JJC4, 457, 1063);
            MakeButton(Button.竞技场连点, 320, 1180);
            MakeButton(Button.日常赛事, 210, 1040);
            MakeButton(Button.日1, 360, 690);
            MakeButton(Button.日2, 360, 840);
            MakeButton(Button.日3, 360, 980);
            MakeButton(Button.取消, 200, 1180);
            MakeButton(Button.扭蛋, 640, 1225);
            MakeButton(Button.养成, 540, 1080);
            MakeButton(Button.继续, 360, 1080);
            MakeButton(Button.大弹窗确认, 515, 915);
            MakeButton(Button.优先活动卡, 233, 657);
            MakeButton(Button.自动编成确认, 515, 830);
            MakeButton(Button.使用宝石, 610, 180);
            MakeButton(Button.使用能量饮料, 610, 320);
            MakeButton(Button.加号, 523, 670);
            MakeButton(Button.刷新协助卡, 650, 1005);
            MakeButton(Button.开始养成, 515, 1174);
            MakeButton(Button.快进, 651, 1209);
            MakeButton(Button.弹窗勾选, 245, 705);
            MakeButton(Button.弹窗确认, 515, 833);
            MakeButton(Button.跳过, 258, 1244);
            MakeButton(Button.缩短所有事件, 68, 623);
            MakeButton(Button.缩短事件确定, 360, 915);
            MakeButton(Button.休息, 115, 985);
            MakeButton(Button.训练, 355, 985);
            MakeButton(Button.技能, 210, 1075);
            MakeButton(Button.医务室, 160, 1125);
            MakeButton(Button.外出, 400, 1125);
            MakeButton(Button.返回, 85, 1200);
            MakeButton(Button.速度, 100, 1080);
            MakeButton(Button.耐力, 230, 1080);
            MakeButton(Button.力量, 360, 1080);
            MakeButton(Button.毅力, 490, 1080);
            MakeButton(Button.智力, 620, 1080);
            MakeButton(Button.参赛, 505, 1080);
            MakeButton(Button.查看结果, 252, 1172);
            MakeButton(Button.前往赛事, 465, 1170);
            MakeButton(Button.比赛快进, 560, 1225);
            MakeButton(Button.比赛结束, 460, 1200);// 作了下调调整
            MakeButton(Button.低继续, 360, 1110);
            MakeButton(Button.选择倒二, 360, 715);
            MakeButton(Button.选择末尾, 360, 830);
            MakeButton(Button.选卡, 570, 680);
            MakeButton(Button.卡1, 360, 200);
            MakeButton(Button.卡2, 360, 375);
            MakeButton(Button.卡3, 360, 545);
            MakeButton(Button.卡4, 360, 720);
            MakeButton(Button.卡5, 360, 900);
            MakeButton(Button.技能获取, 520, 1180);
            MakeButton(Button.结束养成, 205, 915);
            MakeButton(Button.养成结束, 510, 1050);
            MakeButton(Button.结束连点, 360, 1145);
            MakeButton(Button.不弹赛事推荐, 260, 1035);

            //============================================

            MakeZone(Zone.事件类型, 110, 325, 210, 235);
            MakeZone(Zone.事件名, 110, 550, 245, 275);
            MakeZone(Zone.日历, 0, 150, 65, 190);
            MakeZone(Zone.心情, 530, 700, 138, 183);
            MakeZone(Zone.体力, 223, 600, 145, 177);
            MakeZone(Zone.协助1, 580, 700, 185, 290);
            MakeZone(Zone.协助2, 580, 700, 290, 410);
            MakeZone(Zone.协助3, 580, 700, 410, 530);
            MakeZone(Zone.协助4, 580, 700, 530, 650);
            MakeZone(Zone.协助5, 580, 700, 650, 770);
            MakeZone(Zone.速度增加, 25, 140, 775, 825);
            MakeZone(Zone.耐力增加, 140, 250, 775, 825);
            MakeZone(Zone.力量增加, 250, 365, 775, 825);
            MakeZone(Zone.毅力增加, 365, 475, 775, 825);
            MakeZone(Zone.智力增加, 475, 585, 775, 825);
            MakeZone(Zone.速度, 65, 140, 855, 883);
            MakeZone(Zone.耐力, 180, 250, 855, 883);
            MakeZone(Zone.力量, 290, 365, 855, 883);
            MakeZone(Zone.毅力, 405, 475, 855, 883);
            MakeZone(Zone.智力, 515, 585, 855, 883);
            MakeZone(Zone.第1队, 520, 685, 440, 475);
            MakeZone(Zone.第2队, 520, 685, 710, 745);
            MakeZone(Zone.第3队, 520, 685, 980, 1015);
            MakeZone(Zone.卡1, 35, 160, 80, 310);
            MakeZone(Zone.卡2, 35, 160, 250, 480);
            MakeZone(Zone.卡3, 35, 160, 430, 655);
            MakeZone(Zone.卡4, 35, 160, 600, 830);
            MakeZone(Zone.卡5, 35, 160, 780, 1000);
            MakeZone(Zone.技1, 10, 500, 465, 660);
            MakeZone(Zone.技2, 10, 500, 620, 840);
            MakeZone(Zone.技3, 10, 500, 800, 1000);
            MakeZone(Zone.技白, 10, 300, 950, 1025);
            MakeZone(Zone.技能点, 600, 695, 920, 965);
            MakeZone(Zone.技能点2, 520, 650, 400, 440);
        }

        private static void MakeButton(Button bt, double x, double y)
        {
            var rX = x / STANDARD_WIDTH;
            var rY = y / STANDARD_HEIGHT;
            ButtonLocation.Add(bt.ToString(), [rX, rY]);
        }

        private static void MakeZone(Zone zone, double x1, double x2, double y1, double y2)
        {
            var rX1 = x1 / STANDARD_WIDTH;
            var rX2 = x2 / STANDARD_WIDTH;
            var rY1 = y1 / STANDARD_HEIGHT;
            var rY2 = y2 / STANDARD_HEIGHT;
            ZoneLocation.Add(zone.ToString(), [rX1, rX2, rY1, rY2]);
        }


        private static Dictionary<string, double[]> ButtonLocation { get; } = [];
        private static Dictionary<string, double[]> ZoneLocation { get; } = [];
    }
}
