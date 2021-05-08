using System;
using System.IO;

namespace DapperEntityGenerator.BootstrapperInfrastructure
{
    /// <summary>
    ///     The embedded compressed assembly references resolver
    /// </summary>
    class EmbeddedCompressedAssemblyReferencesResolver
    {
        #region Public Methods
        /// <summary>
        ///     Resolves the specified embedded zip file name.
        /// </summary>
        public static void Resolve(string embeddedZipFileName)
        {
            var locatedAssembly = typeof(EmbeddedCompressedAssemblyReferencesResolver).Assembly;

            var appDomain = AppDomain.CurrentDomain;

            var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var zipFolderName = Path.GetFileNameWithoutExtension(locatedAssembly.Location) + "-" + Path.GetFileNameWithoutExtension(embeddedZipFileName);

            var zipExtractDirectoryPath = Path.Combine(myDocuments, zipFolderName) + Path.DirectorySeparatorChar;

            var zipFileData = AssemblyResourceHelper.ReadResource(locatedAssembly, resourceFileName => resourceFileName.EndsWith("." + embeddedZipFileName));

            ZipHelper.ExtractZipFileContentsToDirectory(zipFileData, zipExtractDirectoryPath, Console.WriteLine);

            appDomain.AssemblyResolve += (s, e) => AssemblyResolver.ResolveAssembly(s, e, zipExtractDirectoryPath);
        }
        #endregion
    }
}