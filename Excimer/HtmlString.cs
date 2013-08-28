using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Excimer
{
    public interface ITypedStream
    {
        void WriteTo(Stream outputStream);
        string ContentType { get; }
        Int64 Length { get; }
    }

    public class TypedStream : ITypedStream
    {
        public string ContentType { get; private set; }
        public Int64 Length { get { return _stream.Length; } }

        private Stream _stream;

        public TypedStream(Stream stream, string contentType)
        {
            _stream = stream;
            ContentType = contentType;
        }
    
        public void  WriteTo(Stream outputStream)
        {
            var buffer = new byte[_stream.Length];
            _stream.Read(buffer, 0, (int)_stream.Length);

            outputStream.Write(buffer, 0, (int)_stream.Length);
            outputStream.Close();
        }

        public override string ToString()
        {
            var buffer = new byte[_stream.Length];
            _stream.Read(buffer, 0, (int)_stream.Length);

            return null;
            //return new string(buffer);
        }
    }

    public class TypedString : ITypedStream
    {
        public string ContentType { get; private set; }
        public Int64 Length { get { return _string.Length; } }

        private string _string;

        public TypedString(String @string, string contentType)
        {
            _string = @string;
            ContentType = contentType;
        }
    
        public void WriteTo(Stream outputStream)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(_string);
            outputStream.Write(buffer, 0, (int)_string.Length);
            outputStream.Close();
        }

        public override string ToString()
        {
            return _string;
        }
    }

    public class TextString : TypedString
    {
        public TextString(string content) : base(content, "text/plain") { }
    }

    public class HtmlString : TypedString
    {
        public HtmlString(string content) : base(content, "text/html") { }
    }

    public class JavascriptString : TypedString
    {
        public JavascriptString(string content) : base(content, "text/javascript") { }
    }

    public class JsonString : TypedString
    {
        public JsonString(string content) : base(content, "application/json") { }
    }

    public class PngStream : TypedStream
    {
        public PngStream(Stream stream) : base(stream, "image/png") { }
    }
}
