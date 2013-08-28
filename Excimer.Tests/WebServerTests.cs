using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading;

namespace Excimer.Tests
{
    [TestFixture]
    public class WebServerTests
    {
        [Test]
        public void ShouldStart()
        {
            using (var webServer = new WebServer(null))
            {
                webServer.Start();
                Thread.Sleep(1000);
            }
        }
    }
}
