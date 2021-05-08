using System;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace DapperEntityGenerator.BootstrapperInfrastructure
{
    static class AssemblyResourceHelper
    {
        /// <summary>
        ///     Reads the resource.
        /// </summary>
        public static byte[] ReadResource(Assembly locatedAssembly, Func<string,bool> matchByManifestResourceName)
        {
            var resourceName = locatedAssembly.GetManifestResourceNames().FirstOrDefault(matchByManifestResourceName);
            if (resourceName == null)
            {
                throw new MissingManifestResourceException("Resource not matched");
            }

            using (var manifestResourceStream = locatedAssembly.GetManifestResourceStream(resourceName))
            {
                if (manifestResourceStream == null)
                {
                    throw new MissingManifestResourceException(resourceName);
                }

                var data = new byte[manifestResourceStream.Length];

                manifestResourceStream.Read(data, 0, data.Length);

                return data;
            }
        }
    }
}