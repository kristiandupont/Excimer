using System;
using System.Collections.Generic;
using System.Linq;

namespace Excimer
{
    public interface IParsedRequest {}

    public class PingRequest : IParsedRequest {}

    public class ApiRequest : IParsedRequest
    {
        public string CommandName { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }

    public class FileRequest : IParsedRequest
    {
        public string AssemblyFilename { get; set; }
        public string ResourceName { get; set; }
    }

    public class RequestUrlParser
    {
        public IParsedRequest ParseUrl(string url) { return ParseUrl(new Uri(url)); }

        public IParsedRequest ParseUrl(Uri url)
        {
            if (url.AbsolutePath == "/ping")
            {
                return new PingRequest();
            }
            else if (url.AbsolutePath.StartsWith("/api/"))
            {
                return ParseApiUrl(url);
            }
            else
            {
                return ParseFileUrl(url);
            }
        }

        private ApiRequest ParseApiUrl(Uri url)
        {
            var result = new ApiRequest
            {
                Parameters = new Dictionary<string, string>(),
                CommandName = url.AbsolutePath.Substring("/api/".Length)
            };

            if (!string.IsNullOrEmpty(url.Query))
            {
                var paramStrings = url.Query.Substring(1).Split('&');
                foreach (var parts in paramStrings.Select(p => p.Split('=')))
                {
                    result.Parameters[parts[0]] = Uri.UnescapeDataString(parts[1].Replace('+', ' '));
                }
            }
            return result;
        }

        private FileRequest ParseFileUrl(Uri url)
        {
            var path = url.AbsolutePath.Substring(1);
            var result = new FileRequest();

            if (path.Contains("/"))
            {
                var firstPart = path.Substring(0, path.IndexOf('/'));
                if (urlMappings.ContainsKey(firstPart))
                {
                    var mapping = urlMappings[firstPart];
                    result.AssemblyFilename = mapping.AssemblyName;

                    var secondPart = path.Substring(firstPart.Length + 1).Replace('/', '.');

                    if (string.IsNullOrEmpty(secondPart) || secondPart.EndsWith("."))
                        secondPart += "Index.htm";

                    result.ResourceName = StringExtensions.ConcatenateWithSeparator(mapping.ResourcePrefix, ".", secondPart);
                    return result;
                }
            }

            // First part is not a registered assembly. Treat it as a directory from the default assembly then.
            var defaultMapping = urlMappings[""];
            result.AssemblyFilename = defaultMapping.AssemblyName;

            path = path.Replace('/', '.');

            if (string.IsNullOrEmpty(path) || path.EndsWith("."))
                path += "Index.htm";

            result.ResourceName = StringExtensions.ConcatenateWithSeparator(defaultMapping.ResourcePrefix, ".", path);
            return result;
        }

        private class Mapping
        {
            public string AssemblyName { get; set; }
            public string ResourcePrefix { get; set; }
        }

        private Dictionary<string, Mapping> urlMappings = new Dictionary<string, Mapping>();

        public void AddMapping(string urlFolder, string assemblyName, string resourcePrefix)
        {
            urlMappings[urlFolder] = new Mapping { AssemblyName = assemblyName, ResourcePrefix = resourcePrefix };
        }
    }
}
