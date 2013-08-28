using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excimer;
using System.IO;
using System.Diagnostics;

namespace CherryCommander.Core
{
    public class FileIdentifier
    {
        public string Filename { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
    }

    public class DirWatcher
    {
        public string Directory { get; set; }

        public List<FileIdentifier> GetElements()
        {
            var di = new DirectoryInfo(Directory);
            var dirs = from f in di.GetDirectories()
                       where !f.Attributes.HasFlag(FileAttributes.Hidden)
                       select new FileIdentifier()
                       {
                           Filename = f.Name,
                           FullPath = f.FullName,
                           IsDirectory = true
                       };

            var files = from f in di.GetFiles()
                        where !f.Attributes.HasFlag(FileAttributes.Hidden)
                        select new FileIdentifier()
                            {
                                Filename = f.Name,
                                FullPath = f.FullName,
                                IsDirectory = false
                            };

            var result = new List<FileIdentifier>();

            if (di.Parent != null) result.Add(new FileIdentifier { Filename = "..", FullPath = di.Parent.FullName, IsDirectory = true });
            result.AddRange(dirs);
            result.AddRange(files);

            return result;
        }
    }

    public class CherryCommanderService
    {
        private IOperationRegistry operationRegistry;
        private WebServer webServer;

        public CherryCommanderService()
        {
            operationRegistry = new OperationRegistry();

            operationRegistry.RegisterCommand("GetFiles", new Func<string, List<FileIdentifier>>(GetFiles));

            webServer = new WebServer(operationRegistry) { Port = 8181 };
            webServer.RegisterAssembly(GetType().Assembly, "", "CherryCommander.Core.Ui");
        }

        public List<FileIdentifier> GetFiles(string directory)
        {
            var result = new DirWatcher { Directory = directory }.GetElements();
            return result;
        }

        public void Execute(string filename)
        {
            
        }
    }
}
