using System;
using System.IO;
using DapperEntityGenerator.CodeGeneration;
using Newtonsoft.Json;
using static System.IO.File;
using static DapperEntityGenerator.IO.FileHelper;

namespace DapperEntityGenerator.UI
{
    /// <summary>
    ///     The cache helper
    /// </summary>
    static class CacheHelper
    {
        #region Properties
        /// <summary>
        ///     Gets the configuration file path.
        /// </summary>
        static string ConfigFilePath => Path.Combine(Path.GetTempPath(), "DapperEntityGenerator.json");
        #endregion

        #region Public Methods
        /// <summary>
        ///     Gets the main window model from cache.
        /// </summary>
        public static EntityGeneratorInput GetMainWindowModelFromCache()
        {
            if (!Exists(ConfigFilePath))
            {
                WriteToFile(ConfigFilePath, ReadAllText(Path.GetFileName(ConfigFilePath)));
            }

            return JsonConvert.DeserializeObject<EntityGeneratorInput>(ReadAllText(ConfigFilePath ?? throw new InvalidOperationException()));
        }

        /// <summary>
        ///     Saves the main window model to cache.
        /// </summary>
        public static void SaveMainWindowModelToCache(EntityGeneratorInput model)
        {
            string serializeToJson()
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };

                return JsonConvert.SerializeObject(model, jsonSerializerSettings);
            }

            WriteToFile(ConfigFilePath, serializeToJson());
        }
        #endregion
    }
}