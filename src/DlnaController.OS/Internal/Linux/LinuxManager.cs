using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DlnaController.OS.Internal.Linux
{
    internal class LinuxManager : IOsManager
    {
        protected const string Bash = "/bin/bash";

        protected const string LocalScriptsPath = "scripts";

        protected static readonly string LocalScriptsExtendPath = string.Empty;

        public string WorkingDirectory => ProcessHelper.WorkingDirectory;
        
        protected virtual string GetLocalScripts(string scriptFileName)
        {
            if(!string.IsNullOrEmpty(LocalScriptsExtendPath))
            {
                var extend = Path.Combine(LocalScriptsExtendPath, scriptFileName);
                if (File.Exists(extend))
                {
                    return extend;
                }
            }
            return Path.Combine(LocalScriptsPath, scriptFileName);
        }

        public ProcessResult RunBash(string file)
        {
            return ProcessHelper.Run(Bash, file);
        }

        public ProcessResult RunLocalScript(string scriptFileName)
        {
            return RunBash(GetLocalScripts(scriptFileName));
        }

        public string GetLocalIp()
        {
            return RunLocalScript("ip.sh").Output;
        }
    }
}
