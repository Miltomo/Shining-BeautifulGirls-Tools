using System;
using System.Diagnostics;

namespace Shining_BeautifulGirls
{
    //TODO 改为单例类。修改所有函数，全部变为bool！
    public partial class AdbHelper
    {
        public string Result { get; private set; } = string.Empty;
        public string EmulatorName { get; set; } = string.Empty;

        // 进程启动信息
        ProcessStartInfo PSI { get; init; }

        private AdbHelper(string k) { }

        public AdbHelper(string program, string workspace)
        {
            PSI = new()
            {
                FileName = program,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workspace
            };
        }

        private bool Execute()
        {
            try
            {
                // 创建一个进程对象
                Process process = new()
                {
                    StartInfo = PSI
                };

                // 启动进程
                process.Start();

                // 从进程读取输出
                Result = process.StandardOutput.ReadToEnd();

                // 等待进程完成
                process.WaitForExit();

                Debug.WriteLine(Result);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("发生异常：" + ex.Message);
                return false;
            }
        }

        public static void KillAll()
        {
            try
            {
                // 获取所有同名进程
                Process[] processes = Process.GetProcessesByName("adb");

                // 结束每一个同名进程
                foreach (Process process in processes)
                {
                    process.Kill();
                    Debug.WriteLine($"进程 {process.ProcessName} 已结束");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"结束进程时出错：{ex.Message}");
            }
        }


        public static AdbMaker SetConfig()
        {
            return new AdbMaker();
        }

        public class AdbMaker
        {

            internal AdbMaker() { }

            /// <summary>
            /// 生成ADB实例，只在第一次调用时创建
            /// </summary>
            /// <returns></returns>
            public AdbHelper Make()
            {
                return new AdbHelper("1");
            }
        }
    }
}
