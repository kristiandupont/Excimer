using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excimer
{
    public interface IAutoReleasePoolFactory
    {
        IDisposable CreateAutoReleasePool();
    }

    public class AutoReleasePoolFactory : IAutoReleasePoolFactory
    {
        public IDisposable CreateAutoReleasePool() { return null; }
    }
}
