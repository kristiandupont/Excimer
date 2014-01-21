using NUnit.Framework;

namespace Excimer.Tests
{
    [TestFixture]
    public class HtmlRendererTest
    {
        private const string ExpectedPrefix = "<form id='TestClass_form'>";

        private const string ExpectedSuffix =
            "</form>" + "<script>" +
            "$('#TestClass_form .mappedCheckBox').click(function() { updateConfigVar('TestClass', $(this).attr('name'), ($(this).attr('checked') == 'checked') ? 'True' : 'False');  });" +
            "$('#TestClass_form textarea').keydown(function() { updateConfigVar('TestClass', $(this).attr('name'), $(this).val());  });" +
            "$('#TestClass_form input').keydown(function() { updateConfigVar('TestClass', $(this).attr('name'), $(this).val());  });" +
            "</script>";

        [Test]
        public void SimpleTest()
        {
            var h = new HtmlRenderer();

            h.BeginSettingsForm("TestClass");
            h.AddSegment("Test");
            h.AddMappedCheckbox("Test checkbox", "TestCheckBox", true);
            h.EndSettingsForm();

            var s = h.ToString();

            var expected = ExpectedPrefix +
                "<h1>Test</h1>" +
                "<div class='controls'>" +
                "<label class='checkbox'>" +
                "<input type='checkbox' class='mappedCheckBox' name='TestCheckBox' value='True' checked='checked' />" +
                "Test checkbox" +
                "</label>" +
                "</div>" +
                ExpectedSuffix;

            Assert.That(s, Is.EqualTo(expected));
        }
    }
}
