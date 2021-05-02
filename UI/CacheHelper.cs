using System.IO;
using DapperEntityGenerator.CodeGeneration;
using Newtonsoft.Json;
using static System.IO.File;
using static DapperEntityGenerator.IO.FileHelper;

namespace DapperEntityGenerator.UI
{
    static class CacheHelper
    {
        #region Properties
        static string ConfigFilePath => Path.Combine(Path.GetTempPath(), "DapperEntityGenerator.json");
        #endregion

        #region Public Methods
        public static EntityGeneratorInput GetMainWindowModelFromCache()
        {
            if (!Exists(ConfigFilePath))
            {
                WriteToFile(ConfigFilePath, ReadAllText(Path.GetFileName(ConfigFilePath)));
            }

            return JsonConvert.DeserializeObject<EntityGeneratorInput>(ReadAllText(ConfigFilePath));
        }

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