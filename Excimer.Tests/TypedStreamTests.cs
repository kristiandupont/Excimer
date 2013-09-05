using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Excimer.Tests
{
    [TestFixture]
    public class TypedStreamTests
    {
        [Test]
        public void ShouldWriteToString()
        {
            var s = "Hejsa";
            //var ms = new MemoryStream(s.ToCharArray());

            //var t = new TypedStream(ms, "text");

        }
    }
}
