using System;   
using System.Collections.Generic;
using NUnit.Framework;
using System.Diagnostics;
using Excimer.Drawing;
using Excimer;

namespace ExcimerTests
{
    [TestFixture]
	public class OperationRegistryTests
	{
        private OperationRegistry _operationRegistry;

        [SetUp]
        public void Setup()
        {
            _operationRegistry = new OperationRegistry();
        }

        [Test]
        public void SimpleTest()
        {
            var success = false;
            _operationRegistry.RegisterCommand("VoidFunc", new Action(() => success = true));
            
            var result = _operationRegistry.InvokeCommand("VoidFunc", new Dictionary<string, string>());

            Assert.That(success);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void InvokeWithParameterTest()
        {
            var success = false;

            _operationRegistry.RegisterCommand("Func", new Action<int>(i => success = (i == 14)));

            var args = new Dictionary<string, string>();
            args["i"] = "14";
            var result = _operationRegistry.InvokeCommand("Func", args);

            Assert.That(success);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void InvokeWithMultiParameterTest()
        {
            var success = false;

            _operationRegistry.RegisterCommand("Func", new Action<int, string>((i, s) => success = (i == 14 && s == "hej")));

            var args = new Dictionary<string, string>();
            args["i"] = "14";
            args["s"] = "hej";
            var result = _operationRegistry.InvokeCommand("Func", args);

            Assert.That(success);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void InvokeWithInvalidParametersTest()
        {
            _operationRegistry.RegisterCommand("Func", new Action<int, string>((i, s) => { }));

            var args = new Dictionary<string, string>();
            args["i"] = "14";

            var argumentException = Assert.Throws<ArgumentException>(() => _operationRegistry.InvokeCommand("Func", args));
            Assert.That(argumentException.ParamName, Is.EqualTo("s"));
        }

        [Test]
        public void VariousTypesTest()
        {
            var success = false;

            _operationRegistry.RegisterCommand("Func", new Action<int, bool, string, double, DateTime, TimeSpan>((i, b, s, d, dt, ts) => 
                { 
                    Assert.That(i, Is.EqualTo(14));
                    Assert.That(b, Is.True);
                    Assert.That(s, Is.EqualTo("hej"));
                    Assert.That(d, Is.EqualTo(12.34));
                    Assert.That(dt, Is.EqualTo(new DateTime(2011, 10, 12)));
                    Assert.That(ts, Is.EqualTo(new TimeSpan(1, 2, 3)));
                    success = true;
                }));

            var args = new Dictionary<string, string>();
            args["i"] = "14";
            args["b"] = "true";
            args["s"] = "hej";
            args["d"] = "12.34";
            args["dt"] = "2011-10-12 00:00:00.0000000";
            args["ts"] = "1:2:3";
            var result = _operationRegistry.InvokeCommand("Func", args);

            var tst = DateTime.ParseExact("1978-05-13 12:01:05.123456", "yyyy-MM-dd HH:mm:ss.FFFFFFF", System.Globalization.CultureInfo.InvariantCulture);
            Debug.WriteLine(tst.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF"));

            Assert.That(success);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void InvokeWithReturnValue()
        {
            _operationRegistry.RegisterCommand("Func", new Func<int>(() => 14));

            var args = new Dictionary<string, string>();
            var result = _operationRegistry.InvokeCommand("Func", args);

            Assert.That(result is int);
            Assert.That(result, Is.EqualTo(14));
        }

        private class M { public int a { get; set; } public string b { get; set; } };

        [Test]
        public void InvokeWithMultiReturnVal()
        {
            _operationRegistry.RegisterCommand("Func", new Func<M>(() => new M { a = 14, b = "hej" }));

            var args = new Dictionary<string, string>();
            var result = _operationRegistry.InvokeCommand("Func", args);

            Assert.That(result is M);
            Assert.That(((M)result).a, Is.EqualTo(14));
            Assert.That(((M)result).b, Is.EqualTo("hej"));
        }

        [Test]
        public void ColorParameter()
        {
            _operationRegistry.RegisterCommand("Func", new Action<Color>(c => Assert.That(c, Is.EqualTo(Color.FromRgb(31, 144, 255)))));
            var args = new Dictionary<string, string>();
            args["c"] = "1f90ff";

            var result = _operationRegistry.InvokeCommand("Func", args);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void InvokeWithDictionaryTest()
        {
            _operationRegistry.RegisterCommand("Func", new Action<Dictionary<string, string>>(d => 
                {
                    Assert.That(d["i"], Is.EqualTo("14"));
                    Assert.That(d["s"], Is.EqualTo("hej"));
                }));

            var args = new Dictionary<string, string>();
            args["i"] = "14";
            args["s"] = "hej";
            var result = _operationRegistry.InvokeCommand("Func", args);

            Assert.That(result, Is.Null);
        }
	}
}
