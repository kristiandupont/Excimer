using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Excimer.Tests
{
    [TestFixture]
    public class JsonSerializerTests
    {
        [Test]
        public void ShouldSerializePrimitiveTypes()
        {
            Assert.That(JsonSerializer.Serialize(42), Is.EqualTo("42"));
            Assert.That(JsonSerializer.Serialize(42.1), Is.EqualTo("42.1"));
            Assert.That(JsonSerializer.Serialize(42.1f), Is.EqualTo("42.1"));
            Assert.That(JsonSerializer.Serialize(false), Is.EqualTo("false"));
        }

        [Test]
        public void ShouldSerializeNull()
        {
            Assert.That(JsonSerializer.Serialize(null), Is.EqualTo("null"));
        }

        [Test]
        public void ShouldSerializeClass()
        {
            var result = JsonSerializer.Serialize(new { a = "hej" });
            Assert.That(result, Is.EqualTo("{\"a\": \"hej\"}"));
        }

        [Test]
        public void ShouldSerializeDateTimeAsTicks()
        {
            var result = JsonSerializer.Serialize(new DateTime(1978, 5, 13, 0, 0, 0, DateTimeKind.Utc));
            Assert.That(result, Is.EqualTo("263865600000"));
        }

        [Test]
        public void ShouldSerializeTimeSpanAsMinutes()
        {
            var result = JsonSerializer.Serialize(new TimeSpan(0, 1, 1));
            Assert.That(result, Is.EqualTo("1.01666666666667"));
        }

        [Test]
        public void ShouldSerializeEnumerablesAsArrays()
        {
            var result = JsonSerializer.Serialize(new List<int> { 1, 2, 3 });
            Assert.That(result, Is.EqualTo("[1, 2, 3]"));
        }

        [Test]
        public void ShouldSerializeDictionariesAsMaps()
        {
            var d = new Dictionary<string, object>();

            d["A"] = new {Prop1 = "Prop1Value", Prop2 = 42};
            d["B"] = new { Val="Value" };

            var result = JsonSerializer.Serialize(d);

            Assert.That(result, Is.EqualTo("{\"A\": {\"prop1\": \"Prop1Value\", \"prop2\": 42}, \"B\": {\"val\": \"Value\"}}"));
        }

        [Test]
        public void ShouldSerializeRecursively()
        {
            var t = new List<object>
            {
                new { a = "hej" },
                new { s = "string", i = 14 }
            };

            var result = JsonSerializer.Serialize(t);
            Assert.That(result, Is.EqualTo("[{\"a\": \"hej\"}, {\"s\": \"string\", \"i\": 14}]"));
        }

        [Test]
        public void ShouldMakeFirstLetterLowerCase()
        {
            var o = new
            {
                lowerCaseProp = 1,
                UpperCaseProp = 2,
            };

            var result = JsonSerializer.Serialize(o);
            Assert.That(result, Is.EqualTo("{\"lowerCaseProp\": 1, \"upperCaseProp\": 2}"));
        }

        [Test]
        public void ShouldEscapeStrings()
        {
            var result = JsonSerializer.Escape("Test with \"quotation marks\",\n\rNewlines and CR's,\t tabs");
            Assert.That(result, Is.EqualTo("Test with \\\"quotation marks\\\",\\n\\rNewlines and CR's,\\t tabs"));
        }
    }
}
