using System;
namespace Excimer
{
    public interface IWebServer : IDisposable
    {
        string Index { get; }
        string Root { get; }
        int Port { get; set; }
        string PublicRoot { get; }

        void RegisterAssembly(System.Reflection.Assembly assembly, string urlFolder, string resourcePrefix);
        void RegisterCommand(string name, Delegate commandFunction);

        void Start();
        void Stop();
    }
}
