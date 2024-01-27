using System.Linq;
using System.Text.RegularExpressions;

namespace Shining_BeautifulGirls
{
    partial class AdbHelper
    {
        public AdbHelper Connect(string emulator)
        {
            EmulatorName = emulator;
            PSI.Arguments = $"connect {EmulatorName}";
            Execute();

            return this;
        }

        public string[][] SearchDevices()
        {
            PSI.Arguments = $"devices";
            Execute();

            var nameS = DeviceNameRegex().Matches(Result).ToArray();
            var stateS = DeviceStateRegex().Matches(Result).ToArray();

            string[][] result = new string[nameS.Length][];
            for (int i = 0; i < nameS.Length; i += 1)
                result[i] = [nameS[i].Value, stateS[i].Value];

            return result;
        }


        public int[] GetSize()
        {
            PSI.Arguments = $"-s {EmulatorName} shell wm size";
            Execute();

            return
                DeviceSizeRegex().Matches(Result).ToArray()
                .Select(x => int.Parse(x.Value))
                .ToArray();
        }


        /// <summary>
        /// 获取目标设备的屏幕截图
        /// </summary>
        /// <param name="pictureName"></param>
        public void GetScreen(string pictureName = "background")
        {
            PSI.Arguments = $"-s {EmulatorName} shell screencap -p /sdcard/{pictureName}.png";
            Execute();
            PSI.Arguments = $"-s {EmulatorName} pull /sdcard/{pictureName}.png";
            Execute();
        }

        /// <summary>
        /// 模拟点击屏幕
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bool Click(double x, double y)
        {
            PSI.Arguments = $"-s {EmulatorName} shell input tap {x} {y}";
            return Execute();
        }

        /// <summary>
        /// 模拟滑动屏幕
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="time"></param>
        public void Swipe(double[] start, double[] end, int time)
        {
            PSI.Arguments = $"-s {EmulatorName} shell input swipe " +
                $"{start[0]} {start[1]} " +
                $"{end[0]} {end[1]} " +
                $"{time}";
            Execute();
        }

        [GeneratedRegex("(?<=\\n)\\S+(?=\\t)")]
        private static partial Regex DeviceNameRegex();
        [GeneratedRegex("(?<=\\t)\\S+")]
        private static partial Regex DeviceStateRegex();
        [GeneratedRegex("(\\d+(?=x))|((?<=x)\\d+)")]
        private static partial Regex DeviceSizeRegex();
    }
}
