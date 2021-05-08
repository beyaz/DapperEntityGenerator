using System;
using System.IO;
using System.Reflection;

namespace DapperEntityGenerator.BootstrapperInfrastructure
{
    static class AssemblyResolver
    {
        public static Assembly ResolveAssembly(object sender, ResolveEventArgs args, string searchDirectoryPath)
        {
            // Sample:
            // args.Name => YamlDotNet, Version=8.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
            // new AssemblyName(args.Name).Name => YamlDotNet
            // searchFileName => YamlDotNet.dll

            var searchFileName = new AssemblyName(args.Name).Name + ".dll";

            if (!File.Exists(searchDirectoryPath + searchFileName))
            {
                return null;
            }

            var bytes = File.ReadAllBytes(searchDirectoryPath + searchFileName);

            return Assembly.Load(bytes);
        }
    }
}