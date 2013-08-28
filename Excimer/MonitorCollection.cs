using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Excimer
{
    public interface IMonitorCollection
    {
        void Pulse(string token);
        void Wait(string token, int timeout);
    }

    public class MonitorCollection : IMonitorCollection
    {
        private Dictionary<string, object> _lockObjects = new Dictionary<string, object>();

        public void Wait(string token, int timeout)
        {
            if (!_lockObjects.ContainsKey(token))
                _lockObjects[token] = new object();

            lock (_lockObjects[token]) Monitor.Wait(_lockObjects[token], timeout);
        }

        public void Pulse(string token)
        {
            if (!_lockObjects.ContainsKey(token)) return;
            lock (_lockObjects[token]) Monitor.PulseAll(_lockObjects[token]);
        }
    }
}
