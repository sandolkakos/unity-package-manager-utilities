using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SandolkakosDigital.PackageManagerUtilities.Editor
{
    [Serializable]
    public struct PackageManagerManifestInfo
    {
        private const string PackageManagerManifestFilePath = "./Packages/manifest.json";

        public Dictionary<string, string> dependencies;

        public void Save()
        {
            File.WriteAllText(PackageManagerManifestFilePath, Json.Serialize(this, true));
        }

        public static bool TryLoad(out PackageManagerManifestInfo manifestInfo)
        {
            if (!File.Exists(PackageManagerManifestFilePath))
            {
                manifestInfo = default;
                return false;
            }

            string json = File.ReadAllText(PackageManagerManifestFilePath);
            var deserializedObjet = Json.Deserialize(json) as Dictionary<string, object>;

            if (deserializedObjet != null && deserializedObjet.TryGetValue(nameof(dependencies), out object result))
            {
                var dict = (IDictionary)result;
                manifestInfo = new PackageManagerManifestInfo();
                manifestInfo.dependencies = new Dictionary<string, string>();

                foreach (var packageName in dict.Keys)
                {
                    manifestInfo.dependencies.Add(packageName as string, dict[packageName] as string);
                }

                return true;
            }

            manifestInfo = default;
            return false;
        }
    }
}
