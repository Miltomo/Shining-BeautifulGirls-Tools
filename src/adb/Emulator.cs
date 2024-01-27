using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Shining_BeautifulGirls
{
    public static class Emulator
    {
        public enum Name
        {
            MUMU = 16384,
        }

        public static string[] GetPotentialDevices()
        {
            List<string> others = [];
            var emS = Enum.GetValues<Name>();
            foreach (var em in emS)
            {
                var start = (int)em;
                while (true)
                {
                    if (CheckConnection(port: start))
                    {
                        others.Add($"127.0.0.1:{start++}");
                        continue;
                    }
                    break;
                }
            }
            return [.. others];
        }

        public static bool CheckConnection(string ip = "127.0.0.1", int port = 0)
        {
            try
            {
                Process process = new();

                // 配置 ProcessStartInfo 对象
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C netstat -an | findstr \"LISTENING\" | findstr \"{ip}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process.StartInfo = processStartInfo;

                // 启动进程
                process.Start();

                // 读取并输出命令执行的输出流
                string output = process.StandardOutput.ReadToEnd();

                // 等待命令执行完成
                process.WaitForExit();

                // 关闭进程
                process.Close();

                return Regex.Match(output, $@"{Regex.Escape(ip)}:{port}").Success;
            }
            catch (Exception ex)
            {
                // 处理异常，例如输出到日志或返回 false
                Trace.WriteLine($"Error checking connection: {ex.Message}");
                return false;
            }
        }
    }
}
