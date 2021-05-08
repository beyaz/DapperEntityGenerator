using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace DapperEntityGenerator.BootstrapperInfrastructure
{
    /// <summary>
    ///     The embedded compressed assembly references resolver
    /// </summary>
    class EmbeddedCompressedAssemblyReferencesResolver
    {
        #region Fields
        /// <summary>
        ///     The application domain
        /// </summary>
        readonly AppDomain appDomain;

        /// <summary>
        ///     The embedded zip file name
        /// </summary>
        readonly string embeddedZipFileName;

        /// <summary>
        ///     The located assembly
        /// </summary>
        readonly Assembly locatedAssembly;

        /// <summary>
        ///     The temporary directory
        /// </summary>
        readonly string temporaryDirectory;

        /// <summary>
        ///     The temporary zip file path
        /// </summary>
        readonly string temporaryZipFilePath;

        /// <summary>
        ///     The tracer
        /// </summary>
        readonly Tracer tracer;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbeddedCompressedAssemblyReferencesResolver" /> class.
        /// </summary>
        public EmbeddedCompressedAssemblyReferencesResolver(AppDomain appDomain,
                                                            Assembly  locatedAssembly,
                                                            string    embeddedZipFileName,
                                                            Tracer    tracer)
        {
            this.appDomain           = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            this.locatedAssembly     = locatedAssembly ?? throw new ArgumentNullException(nameof(locatedAssembly));
            this.embeddedZipFileName = embeddedZipFileName ?? throw new ArgumentNullException(nameof(embeddedZipFileName));
            this.tracer              = tracer ?? throw new ArgumentNullException(nameof(tracer));

            var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var zipFolderName = Path.GetFileNameWithoutExtension(locatedAssembly.Location) + "-" + Path.GetFileNameWithoutExtension(embeddedZipFileName);

            temporaryDirectory = Path.Combine(myDocuments, zipFolderName) + Path.DirectorySeparatorChar;

            temporaryZipFilePath = temporaryDirectory + embeddedZipFileName;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbeddedCompressedAssemblyReferencesResolver" /> class.
        /// </summary>
        public EmbeddedCompressedAssemblyReferencesResolver(Assembly locatedAssembly, string assemblyFullName, Tracer tracer) : this(AppDomain.CurrentDomain, locatedAssembly, assemblyFullName, tracer)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Resolves the specified assembly full name.
        /// </summary>
        public static void Resolve(string embeddedZipFileName)
        {
            var assembly = typeof(EmbeddedCompressedAssemblyReferencesResolver).Assembly;
            new EmbeddedCompressedAssemblyReferencesResolver(assembly, embeddedZipFileName, new Tracer()).Resolve();
        }

        /// <summary>
        ///     Resolves this instance.
        /// </summary>
        public void Resolve()
        {
            ExtractZipFile();

            appDomain.AssemblyResolve += (s,e)=>AssemblyResolver.ResolveAssembly(s,e,temporaryDirectory);
        }
        #endregion

        #region Methods
        

        /// <summary>
        ///     Extracts the zip file.
        /// </summary>
        void ExtractZipFile()
        {
            if (File.Exists(temporaryZipFilePath))
            {
                Trace($"Deleting file {temporaryZipFilePath}");
                File.Delete(temporaryZipFilePath);
            }

            var targetDirectory = Path.GetDirectoryName(temporaryZipFilePath);
            if (targetDirectory == null)
            {
                throw new ArgumentNullException(nameof(targetDirectory));
            }

            if (File.Exists(targetDirectory))
            {
                Trace($"Deleting file {targetDirectory}");
                File.Delete(targetDirectory);
            }

            if (Directory.Exists(targetDirectory))
            {
                Trace($"Deleting directory {targetDirectory}");
                Directory.Delete(targetDirectory, true);
            }

            Trace($"Creating directory {targetDirectory}");
            Directory.CreateDirectory(targetDirectory);

            
            File.WriteAllBytes(temporaryZipFilePath, AssemblyResourceHelper.ReadResource(locatedAssembly,resourceFileName => resourceFileName.EndsWith("." + embeddedZipFileName)));

            ZipFile.ExtractToDirectory(temporaryZipFilePath, temporaryDirectory);
        }

        

        /// <summary>
        ///     Traces the specified message.
        /// </summary>
        void Trace(string message)
        {
            tracer.Trace(message);
        }
        #endregion
    }
}