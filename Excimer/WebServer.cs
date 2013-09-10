using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Reflection;
using System.IO;
using Excimer.Drawing;

namespace Excimer
{
    public class WebServer : IWebServer
    {
        public int Port 
        {
            get { return _port; }
            set 
            { 
                if (_isRunning) throw new Exception("Web server cannot change port while running"); 
                _port = value; 
            }
        }
        private int _port = 8100;

        public string Root { get { return "http://localhost:" + Port; } }
        public string PublicRoot { get { return "http://" + Environment.MachineName + ":" + Port; } }

        public string Index { get { return Root; } }

        private Thread _serviceThread;
        private readonly HttpListener _listener = new HttpListener();

        private RequestHandler _requestHandler;

        private bool _isRunning = false;

        public WebServer(RequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public void Start()
        {
            if (_isRunning) throw new Exception("Web server is already running");

            _listener.Prefixes.Add("http://+:" + Port + "/");

            ServicePointManager.UseNagleAlgorithm = false;

            ThreadPool.SetMaxThreads(10, 100);
            ThreadPool.SetMinThreads(1, 1);

            _serviceThread = new Thread(new ThreadStart(ServiceThreadStart));
            _serviceThread.Start();
            _isRunning = true;
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _listener.Stop();
            _serviceThread.Abort();
        }

        public void Dispose()
        {
            Stop();
        }

        private void ServiceThreadStart()
        {
            Thread.CurrentThread.Name = "WebServer Master";
            Env.Log("WebServer.ServiceThreadStart() - host: " + Environment.MachineName);
            _listener.Start();

            while (_listener.IsListening)
            {
				try
				{
					ThreadPool.QueueUserWorkItem(ProcessRequest, _listener.GetContext());
				}
				catch(Exception e) 
				{
					Env.Log ("Exception caught in webserver service thread: " + e.Message);
				}
			}
        }

        void ProcessRequest(object listenerContext)
        {
            using (var arp = Env.Get<IAutoReleasePoolFactory>().CreateAutoReleasePool())
            {
                try
                {
                    var context = listenerContext as HttpListenerContext;
                    var urlDescription = context.Request.RawUrl + (context.Request.IsLocal ? "" : " from outside");
                    Env.Log("Webserver handling " + urlDescription);

                    if(!Env.RunningOnMono)  // Renaming threads in Mono is not allowed.
                        Thread.CurrentThread.Name = urlDescription;

                    var requestData = new StreamReader(context.Request.InputStream).ReadToEnd();
                    var response = _requestHandler.HandleRequest(context.Request.Url.ToString(), requestData, context.Request.HttpMethod);

                    context.Response.StatusCode = (int)response.StatusCode;
                    context.Response.ContentType = response.ResponseStream.ContentType;

                    context.Response.ContentLength64 = response.ResponseStream.Length;
                    response.ResponseStream.WriteTo(context.Response.OutputStream);
                }
                catch (HttpListenerException e) { Env.Log("HttpListenerException: " + e.Message); }
                catch (InvalidOperationException e) { Env.Log("InvalidOperationException: " + e.Message); }
            }
        }

        public void RegisterAssembly(Assembly assembly, string urlFolder, string resourcePrefix)
        {
            _requestHandler.RegisterAssembly(assembly, urlFolder, resourcePrefix);
        }

        public void RegisterCommand(string name, Delegate commandFunction)
        {
            _requestHandler.RegisterCommand(name, commandFunction);
        }
    }
}
