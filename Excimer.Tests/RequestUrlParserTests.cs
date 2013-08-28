using NUnit.Framework;

namespace Excimer.Tests
{
    [TestFixture]
	public class RequestUrlParserTests
	{
        private RequestUrlParser _requestUrlParser = new RequestUrlParser();
        
        [Test]
        public void ParsePingTest()
        {
            var result = _requestUrlParser.ParseUrl("http://localhost:8100/ping");

            Assert.That(result, Is.InstanceOf(typeof(PingRequest)));
        }

        [Test]
        public void ParseApiTest()
        {
            var result = _requestUrlParser.ParseUrl("http://localhost:8100/api/someCommand?p1=v1&p2=v2");

            Assert.That(result, Is.InstanceOf(typeof(ApiRequest)));
            var ar = (ApiRequest)result;

            Assert.That(ar.CommandName, Is.EqualTo("someCommand"));
            Assert.That(ar.Parameters.Count, Is.EqualTo(2));

            Assert.That(ar.Parameters["p1"], Is.EqualTo("v1"));
            Assert.That(ar.Parameters["p2"], Is.EqualTo("v2"));
        }

        [Test]
        public void UrlEncodingTest()
        {
            var result = _requestUrlParser.ParseUrl("http://localhost:8100/api/startTimeBox?period=25%3A00");

            var ar = (ApiRequest)result;
            Assert.That(ar.Parameters["period"], Is.EqualTo("25:00"));
        }

        [Test]
        public void ParseImageTest()
        {
            var result = _requestUrlParser.ParseUrl("http://localhost:8100/api/renderOrb?width=32&height=32&frameColor=#000000&primaryColor=#ffffff&satelliteColor=#ff0000");
        }

        [Test]
        public void ParseFileWithQueryStringTest()
        {
            _requestUrlParser.AddMapping("someassembly", "someassembly.dll", "Ui");
            var result = _requestUrlParser.ParseUrl("http://localhost:8100/someassembly/test.jpg?v=1");

            var fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("someassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.test.jpg"));
        }

        [Test]
        public void MappingTest()
        {
            _requestUrlParser.AddMapping("someassembly", "someassembly.dll", "Ui");

            // Try a file in the mapped assembly
            var result = _requestUrlParser.ParseUrl("http://localhost:8100/someassembly/somefile.html");
            Assert.That(result, Is.InstanceOf(typeof(FileRequest)));
            var fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("someassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.somefile.html"));

            // Get a file in a folder
            result = _requestUrlParser.ParseUrl("http://localhost:8100/someassembly/folderA/folderB/somefile.html");
            Assert.That(result, Is.InstanceOf(typeof(FileRequest)));
            fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("someassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.folderA.folderB.somefile.html"));

            // No filename. Get Index.htm
            result = _requestUrlParser.ParseUrl("http://localhost:8100/someassembly/");
            Assert.That(result, Is.InstanceOf(typeof(FileRequest)));
            fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("someassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.Index.htm"));

            // Only foldername
            result = _requestUrlParser.ParseUrl("http://localhost:8100/someassembly/folderA/");
            Assert.That(result, Is.InstanceOf(typeof(FileRequest)));
            fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("someassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.folderA.Index.htm"));
        }

        [Test]
        public void DefaultMappingTest()
        {
            _requestUrlParser.AddMapping("", "defaultassembly.dll", "Ui");

            var result = _requestUrlParser.ParseUrl("http://localhost:8100/test.htm");
            Assert.That(result, Is.InstanceOf(typeof(FileRequest)));
            var fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("defaultassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.test.htm"));

            // No filename. Get Index.htm
            result = _requestUrlParser.ParseUrl("http://localhost:8100/");
            Assert.That(result, Is.InstanceOf(typeof(FileRequest)));
            fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("defaultassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.Index.htm"));

            // No filename and no path separator. Get Index.htm
            result = _requestUrlParser.ParseUrl("http://localhost:8100");
            Assert.That(result, Is.InstanceOf(typeof(FileRequest)));
            fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("defaultassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.Index.htm"));

            // Get a file in a folder
            result = _requestUrlParser.ParseUrl("http://localhost:8100/folderA/folderB/somefile.html");
            Assert.That(result, Is.InstanceOf(typeof(FileRequest)));
            fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("defaultassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.folderA.folderB.somefile.html"));

            // Only foldername
            result = _requestUrlParser.ParseUrl("http://localhost:8100/folderA/folderB/");
            Assert.That(result, Is.InstanceOf(typeof(FileRequest)));
            fr = (FileRequest)result;
            Assert.That(fr.AssemblyFilename, Is.EqualTo("defaultassembly.dll"));
            Assert.That(fr.ResourceName, Is.EqualTo("Ui.folderA.folderB.Index.htm"));
        
        }
    }
}
