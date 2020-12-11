using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SandolkakosDigital.PackageManagerUtilities.Editor
{
    [InitializeOnLoad]
    public static class DependenciesResolver
    {
        #region Confirmation Dialog Texts
        private const string DialogTitle = "Dependencies Resolver";

        private const string DialogMessage =
            "New Git Dependencies were detected in one or more Custom Packages." +
            "\n\n" +
            "The dependencies were added to './Packages/manifest.json'." +
            "\n\n" +
            "The Unity Package Manager will resolve them after Unity getting unfocused and focused again!";

        private const string DialogOkButton = "Ok";
        #endregion

        private const string CachedPackagesDirectory = "./Library/PackageCache";
        private const string LocalPackagesDirectory = "./Packages";

        static DependenciesResolver()
        {
            Resolve();

            // That is uses because InitializeOnLoad will not get invoked when there are code errors,
            // and that is exactly the main reason we want to resolve dependencies.
            Application.logMessageReceived -= OnLogMessageReceived;
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                Application.logMessageReceived -= OnLogMessageReceived;
                Resolve();
            }
        }

        [MenuItem("Test/Resolve")]
        public static void Resolve()
        {
            if (TryGetUnresolvedGitDependencies(out var unresolvedGitDependencies))
            {
                try
                {
                    AssetDatabase.StartAssetEditing();

                    Debug.Log($"Starting to resolve not installed Git Dependencies.");

                    if (PackageManagerManifestInfo.TryLoad(out PackageManagerManifestInfo manifestInfo))
                    {
                        var dependencies = manifestInfo.dependencies.ToList();

                        foreach (var item in unresolvedGitDependencies)
                        {
                            Debug.Log($"Dependency: {item.Key} : {item.Value}");
                            dependencies.Insert(0, new KeyValuePair<string, string>(item.Key, item.Value));
                        }

                        manifestInfo.dependencies = dependencies.Distinct().ToDictionary(pair => pair.Key, pair => pair.Value);
                        manifestInfo.Save();

                        Debug.Log($"Git Dependencies resolved.");
                    }
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();

                    // Showing that dialog because Unity will not resolve the new entries before the Editor gets unfocused and focused again.
                    if (EditorUtility.DisplayDialog(DialogTitle, DialogMessage, DialogOkButton))
                    {
                        // Recompile the packages
                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        private static bool TryGetUnresolvedGitDependencies(out KeyValuePair<string, string>[] result)
        {
            if (TryGetInstalledPackages(out var installedPackages)
                && TryGetGitDependencies(installedPackages, out var gitDependencies))
            {
                result = gitDependencies
                    .Where(x => !installedPackages.Exists(i => i.name.Equals(x.Key)))
                    .ToArray();

                return result?.Length > 0;
            }

            result = default;
            return false;
        }

        private static bool TryGetGitDependencies(List<PackageInfo> installedPackages, out KeyValuePair<string, string>[] result)
        {
            result = installedPackages
                .Where(x => !x.Equals(default) && x.gitDependencies?.Count > 0)
                .SelectMany(x => x.gitDependencies)
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .ToArray();

            return result?.Length > 0;
        }

        private static bool TryGetInstalledPackages(out List<PackageInfo> result)
        {
            result = Directory.GetDirectories(CachedPackagesDirectory)
                .Concat(Directory.GetDirectories(LocalPackagesDirectory))
                .Select(PackageInfo.GetPackageFromDirectory)
                .Where(x => !x.Equals(default))
                .ToList();

            return result?.Count > 0;
        }
    }
}