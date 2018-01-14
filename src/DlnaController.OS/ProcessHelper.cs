using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace DlnaController.OS
{
    public static class ProcessHelper
    {
        public static string WorkingDirectory { get; internal set; } = string.Empty;

        public static T Run<T>(string cmd, string arguments = null, int timeout = 3000)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Run(cmd, arguments, timeout).Output);
        }

        public static ProcessResult Run(string cmd, string arguments = null, int timeout = 3000)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = cmd,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Arguments = arguments,
                        WorkingDirectory = WorkingDirectory
                    };

                    process.Start();

                    string result;
                    string error;

                    using (var sr = process.StandardOutput)
                    {
                        result = sr.ReadToEnd();
                    }

                    using (var sr = process.StandardError)
                    {
                        error = sr.ReadToEnd();
                    }

                    process.WaitForExit(timeout);
                    if (!process.HasExited)
                    {
                        process.Kill();
                        throw new TimeoutException($"执行 {cmd} {arguments} 超时");
                    }
                    return new ProcessResult(process.ExitCode, result, error);
                }
            }
            catch (Win32Exception ex) when (ex.Message.Contains("No such file or directory"))
            {

                throw new IOException($"脚本文件不存在：{cmd}");
            }
        }
    }
}
