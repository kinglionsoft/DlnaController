using System;
using System.Collections.Generic;
using System.Text;

namespace DlnaController.OS
{
    public interface IOsManager
    {
        string WorkingDirectory { get; }

        string GetLocalIp();
    }
}
