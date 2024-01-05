using System;
using System.Diagnostics;

namespace Shining_BeautifulGirls
{
    public partial class AdbHelper
    {
        public string 输出 { get; set; } = string.Empty;
        public string EmulatorName { get; set; } = string.Empty;

        string ProgramPath { get; init; }

        // 进程启动信息
        ProcessStartInfo PSI { get; init; }

        public AdbHelper(string program, string workspace)
        {
            KillProcess("adb");
            ProgramPath = program;
            PSI = new()
            {
                FileName = ProgramPath,
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
                输出 = process.StandardOutput.ReadToEnd();

                // 等待进程完成
                process.WaitForExit();

                Debug.WriteLine(输出);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("发生异常：" + ex.Message);
                return false;
            }
        }

        public static void KillProcess(string processName)
        {
            try
            {
                // 获取所有同名进程
                Process[] processes = Process.GetProcessesByName(processName);

                // 结束每一个同名进程
                foreach (Process process in processes)
                {
                    process.Kill();
                    Console.WriteLine($"进程 {process.ProcessName} 已结束");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"结束进程时出错：{ex.Message}");
            }
        }
    }
}
