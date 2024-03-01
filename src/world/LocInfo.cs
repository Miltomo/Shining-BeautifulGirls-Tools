namespace Shining_BeautifulGirls
{
    partial class World
    {
        public static class NP
        {
            public enum ZButton
            {
                养成,
                医务室,
                养成日常赛事位,
                外出1,
                外出2,
                返回,
                高返回,
                通用参赛,
                查看结果,

                JJC队伍1,
                JJC队伍2,
                JJC队伍3,

                传奇赛事,
                传奇赛事比赛位,

                群英联赛,
                群英联赛报名,
                群英联赛主按钮,
            }
            public enum Button
            {
                强化编成,
                剧情,
                主页,
                赛事,
                JJC1,
                JJC2,
                竞技场连点,
                日常赛事,
                日1,
                日2,
                日3,
                取消,
                扭蛋,
                继续,
                大弹窗确认,
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
                外出,
                速度,
                耐力,
                力量,
                毅力,
                智力,
                能力详情,
                赛事位置1,
                赛事位置2,
                比赛结束,
                低继续,
                最低继续,
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
                赛事活动,
                比赛连点,
                传奇赛事连点,
                群英联赛限定任务,
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
                决赛判断,

                赛事Name1, 赛事Intro1, 赛事Fans1, 赛事Type1,
                赛事Name2, 赛事Intro2, 赛事Fans2, 赛事Type2,
                Rank草地, Rank泥地,
                Rank短距离, Rank英里, Rank中距离, Rank长距离,
                Rank领跑, Rank跟前, Rank居中, Rank后追,

                上部, 中部, 下部,
                群英联赛赛程位,
                群英联赛识别位,
            }
        }

        public static void CreateLocInfo()
        {
            ButtonLocation.Clear();
            ZoneLocation.Clear();

            MakeButton(Button.强化编成, 75, 1225);
            MakeButton(Button.剧情, 190, 1225);
            MakeButton(Button.主页, 360, 1195, 50, 25);
            MakeButton(Button.赛事, 520, 1225);
            MakeButton(Button.JJC1, 208, 864);
            MakeButton(Button.JJC2, 360, 810);
            MakeButton(Button.竞技场连点, 275, 1165, 40); // last 320 1180
            MakeButton(Button.日常赛事, 210, 1040);
            MakeButton(Button.日1, 360, 690);
            MakeButton(Button.日2, 360, 840);
            MakeButton(Button.日3, 360, 980);
            MakeButton(Button.取消, 200, 1180);
            MakeButton(Button.扭蛋, 640, 1225);
            MakeButton(Button.继续, 360, 1080);
            MakeButton(Button.大弹窗确认, 515, 915);
            MakeButton(Button.使用宝石, 610, 180);
            MakeButton(Button.使用能量饮料, 610, 320);
            MakeButton(Button.加号, 523, 670);
            MakeButton(Button.刷新协助卡, 650, 1005);
            MakeButton(Button.开始养成, 515, 1174);
            MakeButton(Button.快进, 651, 1209);
            MakeButton(Button.弹窗勾选, 245, 705);
            MakeButton(Button.弹窗确认, 515, 833);
            MakeButton(Button.跳过, 260, 1248, 60);
            MakeButton(Button.缩短所有事件, 360, 625, 250); // last is 70
            MakeButton(Button.缩短事件确定, 360, 915);
            MakeButton(Button.休息, 115, 985);
            MakeButton(Button.训练, 355, 985);
            MakeButton(Button.技能, 210, 1075);
            MakeButton(Button.外出, 400, 1125);

            int trainY = 1055, train_d = 25;
            MakeButton(Button.速度, 105, trainY, train_d, train_d);
            MakeButton(Button.耐力, 233, trainY, train_d, train_d);
            MakeButton(Button.力量, 360, trainY, train_d, train_d);
            MakeButton(Button.毅力, 488, trainY, train_d, train_d);
            MakeButton(Button.智力, 615, trainY, train_d, train_d);

            MakeButton(Button.能力详情, 645, 772, 20, 20);
            MakeButton(Button.赛事位置1, 360, 765, 200, 30);
            MakeButton(Button.赛事位置2, 360, 917, 200, 30);

            MakeButton(Button.比赛连点, 560, 1260, dy: 5);
            MakeButton(Button.比赛结束, 435, 1200, 55, 10);// last is 460 maybe => [1230 , 30] ???
            MakeButton(Button.低继续, 360, 1110);
            MakeButton(Button.最低继续, 360, 1172);
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
            MakeButton(Button.结束连点, 345, 1145, 20, 5);// 中心点在1140 - 1145 之间
            MakeButton(Button.不弹赛事推荐, 260, 1035);

            MakeButton(Button.赛事活动, 505, 850, 130, 90);
            MakeButton(Button.传奇赛事连点, 412, 1140, 15, 5);
            MakeButton(Button.群英联赛限定任务, 650, 850, 20, 30);

            //============================================
            double L = 0, R = STANDARD_WIDTH;
            double U = 0, D = STANDARD_HEIGHT;
            MakeZone(Zone.上部, L, R, U, 300);
            MakeZone(Zone.中部, L, R, 300, 1000);
            MakeZone(Zone.下部, L, R, 1000, D);

            MakeZone(Zone.群英联赛赛程位, L, R, 420, 640);
            MakeZone(Zone.群英联赛识别位, 170, 540, 920, 1160);

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
            MakeZone(Zone.卡1, 35, 160, 80, 310);
            MakeZone(Zone.卡2, 35, 160, 250, 480);
            MakeZone(Zone.卡3, 35, 160, 430, 655);
            MakeZone(Zone.卡4, 35, 160, 600, 830);
            MakeZone(Zone.卡5, 35, 160, 780, 1000);
            MakeZone(Zone.技1, L, R, 465, 660);
            MakeZone(Zone.技2, L, R, 620, 840);
            MakeZone(Zone.技3, L, R, 800, 1000);
            MakeZone(Zone.技白, 10, 300, 950, 1025);
            MakeZone(Zone.技能点, 600, 695, 920, 965);
            MakeZone(Zone.技能点2, 520, 650, 400, 440);
            MakeZone(Zone.决赛判断, 403, 626, 975, 1028);

            int ssNameL = 30, ssNameR = 250;
            int ssFansL = 295, ssFansR = 530;
            int ssTypeL = 560, ssTypeR = 690;

            MakeZone(Zone.赛事Name1, ssNameL, ssNameR, 680, 870);
            MakeZone(Zone.赛事Intro1, ssNameR, ssTypeR, 680, 760);
            MakeZone(Zone.赛事Fans1, ssFansL, ssFansR, 750, 870);
            MakeZone(Zone.赛事Type1, ssTypeL, ssTypeR, 750, 870);

            MakeZone(Zone.赛事Name2, ssNameL, ssNameR, 830, 1030);
            MakeZone(Zone.赛事Intro2, ssNameR, ssTypeR, 830, 910);
            MakeZone(Zone.赛事Fans2, ssFansL, ssFansR, 905, 1030);
            MakeZone(Zone.赛事Type2, ssTypeL, ssTypeR, 905, 1030);

            int rC1L = 230, rC1R = 280, rC2L = 365, rC2R = 410,
                rC3L = 500, rC3R = 540, rC4L = 630, rC4R = 675;
            int rR1U = 390, rR1D = 445, rR2U = 435, rR2D = 480,
                rR3U = 480, rR3D = 520;
            // 场地适应性
            MakeZone(Zone.Rank草地, rC1L, rC1R, rR1U, rR1D);
            MakeZone(Zone.Rank泥地, rC2L, rC2R, rR1U, rR1D);
            // 距离适应性
            MakeZone(Zone.Rank短距离, rC1L, rC1R, rR2U, rR2D);
            MakeZone(Zone.Rank英里, rC2L, rC2R, rR2U, rR2D);
            MakeZone(Zone.Rank中距离, rC3L, rC3R, rR2U, rR2D);
            MakeZone(Zone.Rank长距离, rC4L, rC4R, rR2U, rR2D);
            // 跑法适应性
            MakeZone(Zone.Rank领跑, rC1L, rC1R, rR3U, rR3D);
            MakeZone(Zone.Rank跟前, rC2L, rC2R, rR3U, rR3D);
            MakeZone(Zone.Rank居中, rC3L, rC3R, rR3U, rR3D);
            MakeZone(Zone.Rank后追, rC4L, rC4R, rR3U, rR3D);

            //============================================
            BuildZB(ZButton.养成, 410, 1040, 680, 1130);
            BuildZB(ZButton.返回, 45, 1200, 130, 1260);
            BuildZB(ZButton.高返回, 36, 1190, 130, 1210);
            BuildZB(ZButton.医务室, 107, 1085, 228, 1170);
            BuildZB(ZButton.养成日常赛事位, 491, 1084, 613, 1171);
            BuildZB(ZButton.通用参赛, 495, 1085, 605, 1115);
            BuildZB(ZButton.查看结果, 165, 1145, 340, 1200);

            BuildZB(ZButton.外出1, 50, 400, 670, 500);
            BuildZB(ZButton.外出2, 50, 540, 670, 640);

            BuildZB(ZButton.JJC队伍1, 36, 238, 685, 467);
            BuildZB(ZButton.JJC队伍2, 36, 514, 685, 740);
            BuildZB(ZButton.JJC队伍3, 36, 782, 685, 1010);

            BuildZB(ZButton.传奇赛事, 377, 833, 637, 1011);
            BuildZB(ZButton.传奇赛事比赛位, 40, 632, 677, 741);

            BuildZB(ZButton.群英联赛, 85, 835, 340, 1010);
            BuildZB(ZButton.群英联赛报名, 170, 875, 550, 970);
            BuildZB(ZButton.群英联赛主按钮, 255, 1045, 465, 1130);
        }

        private static void MakeButton(Button bt, double x, double y, double dx = 15, double dy = 15)
        {
            MakeButton(bt.ToString(), x, y, dx, dy);
        }
        private static void MakeButton(string bt, double x, double y, double dx = 15, double dy = 15)
        {
            x /= STANDARD_WIDTH;
            y /= STANDARD_HEIGHT;
            dx /= STANDARD_WIDTH;
            dy /= STANDARD_HEIGHT;

            ButtonLocation.Add(bt, [x, y, dx, dy]);
        }

        private static void MakeZone(Zone zone, double x1, double x2, double y1, double y2)
        {
            MakeZone(zone.ToString(), x1, x2, y1, y2);
        }
        private static void MakeZone(string zone, double x1, double x2, double y1, double y2)
        {
            x1 /= STANDARD_WIDTH;
            x2 /= STANDARD_WIDTH;
            y1 /= STANDARD_HEIGHT;
            y2 /= STANDARD_HEIGHT;
            ZoneLocation.Add(zone, [x1, x2, y1, y2]);
        }

        private static void BuildZB(ZButton zb, double x1_left, double y1_up, double x2_right, double y2_down)
        {
            var cx = (x1_left + x2_right) / 2;
            var cy = (y1_up + y2_down) / 2;

            MakeButton(zb.ToString(), cx, cy, Math.Floor(x2_right - cx), Math.Floor(y2_down - cy));
            MakeZone(zb.ToString(), x1_left, x2_right, y1_up, y2_down);
        }


        private static Dictionary<string, double[]> ButtonLocation { get; } = [];
        private static Dictionary<string, double[]> ZoneLocation { get; } = [];
    }
}
