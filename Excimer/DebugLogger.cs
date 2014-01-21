using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Excimer
{
    public class DebugLogger : ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
