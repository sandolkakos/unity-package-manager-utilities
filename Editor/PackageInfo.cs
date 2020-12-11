using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SandolkakosDigital.PackageManagerUtilities.Editor
{
    [Serializable]
    public struct PackageInfo
    {
        private const string PackageInfoFileName = "package.json";

        public string name;
        public string version;
        public string displayName;
        public string description;
        public string[] keywords;
        public Dictionary<string, string> dependencies;
        public Dictionary<string, string> gitDependencies;

        public static PackageInfo GetPackageFromDirectory(string directory)
        {
            if (TryGetPackageInfo(Path.Combine(directory, PackageInfoFileName), out var packageInfo))
            {
                return packageInfo;
            }
            else
            {
                return default;
            }
        }

        public static bool TryGetPackageInfo(string filePath, out PackageInfo packageInfo)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    packageInfo = default;
                    return false;
                }

                string json = File.ReadAllText(filePath);
                var deserializedObjet = Json.Deserialize(json) as Dictionary<string, object>;

                if (deserializedObjet != null)
                {
                    packageInfo = new PackageInfo();

                    if (deserializedObjet.TryGetValue(nameof(packageInfo.name), out object result))
                    {
                        packageInfo.name = (string)result;
                    }
                    else
                    {
                        return false;
                    }

                    if (deserializedObjet.TryGetValue(nameof(packageInfo.version), out result))
                    {
                        packageInfo.version = (string)result;
                    }
                    else
                    {
                        return false;
                    }

                    if (deserializedObjet.TryGetValue(nameof(packageInfo.displayName), out result))
                    {
                        packageInfo.displayName = (string)result;
                    }
                    else
                    {
                        return false;
                    }

                    if (deserializedObjet.TryGetValue(nameof(packageInfo.description), out result))
                    {
                        packageInfo.description = (string)result;
                    }

                    if (deserializedObjet.TryGetValue(nameof(packageInfo.keywords), out result))
                    {
                        var list = (IList)result;
                        int count = list.Count;
                        packageInfo.keywords = new string[count];

                        for (int i = 0; i < count; i++)
                        {
                            packageInfo.keywords[i] = list[i] as string;
                        }
                    }

                    if (deserializedObjet.TryGetValue(nameof(packageInfo.dependencies), out result))
                    {
                        var dict = (IDictionary)result;
                        packageInfo.dependencies = new Dictionary<string, string>();

                        foreach (var packageName in dict.Keys)
                        {
                            packageInfo.dependencies.Add(packageName as string, dict[packageName] as string);
                        }
                    }

                    if (deserializedObjet.TryGetValue(nameof(packageInfo.gitDependencies), out result))
                    {
                        var dict = (IDictionary)result;
                        packageInfo.gitDependencies = new Dictionary<string, string>();

                        foreach (var packageName in dict.Keys)
                        {
                            packageInfo.gitDependencies.Add(packageName as string, dict[packageName] as string);
                        }
                    }

                    return true;
                }

                packageInfo = default;
                return false;
            }
            catch
            {
                packageInfo = default;
                return false;
            }
        }
    }
}
