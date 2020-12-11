UnityPackageManager Utilities
===

## Description
That package contains a utility that makes it possible to resolve Git Dependencies inside custom packages installed in your project via [UPM - Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html).

## How to use
Just copy the link below and add it to your project via Unity Package Manager: [Installing from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)
```
https://github.com/sandolkakos/unitypackagemanagerutilities.git
```

## Explanation

In *Unity 2018.3* the UPM added support for the use of Git URL to clone custom packages. More info here: [Installing from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

At the moment, UPM only supports dependencies having *name* and *version* as described [in the documentation](https://docs.unity3d.com/Manual/upm-manifestPkg.html):
```json
  "dependencies": {
    "com.companyname.packagename": "1.0.0"
  }
```

❌️ So we cannot add a dependency like that:
```json
  "dependencies": {
    "com.companyname.packagename": "https://your-git-repository-url.git"
  }
```

✅️ With the use of the **UnityPackageManager Utilities**, all that your custom package needs is to have a new entry in the *package.json* like that:
```json
  "gitDependencies": {
    "com.companyname.packagename": "https://your-git-repository-url.git"
  }
```
----

### The *package.json* of your custom package with dependencies should be something like:
```json
{
  "name": "com.companyname.custompackagename",
  "version": "1.0.0",
  "displayName": "Custom Package Name",
  "description": "Description of your custom package.",
  "author": {
    "name": "Author Name",
    "url": "https://your-url.com"
  },
  "gitDependencies": {
    "com.companyname.packagename_example1_root_of_git_respository": "https://your-git-repository-url.git",
  "com.companyname.packagename_example2_path_in_the_git_respository": "https://your-git-repository-url.git?path=/Packages/YourPackageName",
  "com.companyname.packagename_example3_path_in_the_git_respository_with_tag_version": "https://your-git-repository-url.git?path=/Packages/YourPackageName#1.2.0"
  }
}
```
----

## Path Query Parameter

With Unity 2019.2 or earlier, you can make use of [path query parameter (Subfolder)](https://forum.unity.com/threads/some-feedback-on-package-manager-git-support.743345/#post-5425311).

That would help when your *Custom Package* is not hosted at the root of your git repository.

That means you can create a repository with a Unity Project which will hold all your Custom Packages. That will avoid all the pain to maintain lots of custom packages in separate git repositories.

And when you need to use any of the packages inside your repository, you just need to make use of the *path parameter* in your git url, like:
```
https://your-git-repository-url.git?path=/Packages/YourPackageName
```

### According to that [Unity Forum post](https://forum.unity.com/threads/some-feedback-on-package-manager-git-support.743345/#post-5425311):

- You can specify a repository subfolder for a Git package through the path query parameter. The Package manager will only register the package located in the specified repository subfolder and disregard the rest of the repository.

### Special considerations:

- *path* must be a relative path to the root of the repository. An absolute path won't work. Ex:
  - `path=c:\my\repo\subfolder` is ❌️
  - `path=/subfolder` is ✅️
- Indirection notation is supported but will block at the repository root. Ex:
    - `/../../..` will resolve to `/`
- path query parameter must be placed before the revision anchor. The reverse order will fail.
- A package manifest (package.json) is expected in the specified path.

### Examples:

Path query parameter:

```
https://github.com/user/repo.git?path=/example/folder
```

Revision anchor and path query parameter:

```
https://github.com/user/repo.git?path=/example/folder#v1.2.3
```

Two packages from the same repository:

```
https://github.com/user/repo.git?path=/packageA https://github.com/user/repo.git?path=/packageB
```

----

## License

* MIT
* [MiniJson](https://gist.github.com/darktable/1411710) by Calvin Rien

## Author

[Marllon Vilano](https://github.com/sandolkakos)

## See Also

* GitHub page : https://github.com/sandolkakos/unitypackagemanagerutilities
* Releases : https://github.com/sandolkakos/unitypackagemanagerutilities/releases
* Issue tracker : https://github.com/sandolkakos/unitypackagemanagerutilities/issues
* Change log : https://github.com/sandolkakos/unitypackagemanagerutilities/CHANGELOG.md

## Final considerations

That work was inspired on that repository:
- https://github.com/mob-sakai/GitDependencyResolverForUnity
