using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Net;
using System.IO;
using Excimer.Drawing;

namespace Excimer
{
    public class Response
    {
        public ITypedStream ResponseStream { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public Response(ITypedStream responseStream, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            ResponseStream = responseStream;
            StatusCode = statusCode;
        }
    }

    public class RequestHandler
    {
        private Dictionary<string, Assembly> _registeredAssemblies = new Dictionary<string, Assembly>();
        private IOperationRegistry _operationRegistry;
        private RequestUrlParser _requestUrlParser = new RequestUrlParser();

        public RequestHandler(IOperationRegistry operationRegistry)
        {
            _operationRegistry = operationRegistry;
        }

        public void RegisterCommand(string name, Delegate commandFunction)
        {
            _operationRegistry.RegisterCommand(name, commandFunction);
        }

        public void RegisterAssembly(Assembly assembly, string urlFolder, string resourcePrefix)
        {
            var assemblyFilename = Path.GetFileName(assembly.Location);
            _registeredAssemblies[assemblyFilename] = assembly;

            _requestUrlParser.AddMapping(urlFolder, assemblyFilename, resourcePrefix);
        }

        public Response HandleRequest(string url, string requestData, string httpMethod)
        {
            var parsedRequest = _requestUrlParser.ParseUrl(url);

            if (parsedRequest is PingRequest)
                return new Response(new TextString("pong"));

            else if (parsedRequest is ApiRequest)
                return HandleApiRequest((ApiRequest)parsedRequest, requestData);

            else if (parsedRequest is FileRequest)
                return HandleFileRequest((FileRequest)parsedRequest, url);

            else 
                throw new Exception("Unknown request type");
        }

        private Response HandleApiRequest(ApiRequest apiRequest, string requestData)
        {
            var parameters = apiRequest.Parameters;

            // If this was a POST request, the request data itself contains parameters.
            if (!string.IsNullOrWhiteSpace(requestData))
            {
                var paramStrings = requestData.Split('&');
                foreach (var parts in paramStrings.Select(p => p.Split('=')))
                {
                    parameters[parts[0]] = Uri.UnescapeDataString(parts[1].Replace('+', ' '));
                }
            }

            var rawResult = _operationRegistry.InvokeCommand(apiRequest.CommandName, parameters);

            if (rawResult == null)
            {
                return new Response(new JsonString(""));
            }
            else
            {
                if (rawResult is Image)   // Image generates a png
                {
                    var memoryStream = new MemoryStream();
                    ((Image)rawResult).SaveBmp(memoryStream);
                    memoryStream.Position = 0;

                    return new Response(new TypedStream(memoryStream, "image/png"));
                }
                else if (rawResult is ITypedStream)
                {
                    return new Response((ITypedStream)rawResult);
                }
                else        // Otherwise, return a json-serialized version
                {
                    string result = JsonSerializer.Serialize(rawResult);
                    return new Response(new JsonString(result));
                }
            }
        }

        private Response HandleFileRequest(FileRequest fileRequest, string rawUrl)
        {
            try
            {
                var contentType = "text/plain";
                var ext = Path.GetExtension(fileRequest.ResourceName).ToLower();
                if (ext == ".htm" || ext == ".html") contentType = "text/html";
                else if (ext == ".css") contentType = "text/css";
                else if (ext == ".js") contentType = "text/javascript";
                else if (ext == ".png") contentType = "image/png";
                else if (ext == ".jpg" || ext == ".jpeg") contentType = "image/jpg";

                var assemblyWithResources = _registeredAssemblies[fileRequest.AssemblyFilename];
                var stream = assemblyWithResources.GetManifestResourceStream(fileRequest.ResourceName);

                if (stream == null)
                    throw new Exception();

                return new Response(new TypedStream(stream, contentType));
            }
            catch (Exception)
            {
                Env.Log("Webserver 404: file '" + rawUrl + "' not found.");
                return new Response(new TextString(rawUrl + " not found."), HttpStatusCode.NotFound);
            }
        }
    }
}
