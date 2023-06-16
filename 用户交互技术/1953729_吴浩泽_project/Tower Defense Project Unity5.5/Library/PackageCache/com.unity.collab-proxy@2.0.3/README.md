# Collaborate Client Package
![ReleaseBadge](https://badges.cds.internal.unity3d.com/packages/com.unity.collab-proxy/release-badge.svg) ![ReleaseBadge](https://badges.cds.internal.unity3d.com/packages/com.unity.collab-proxy/candidates-badge.svg)

This is the package to add Collaborate support to the Unity Editor. Unlike its predecessor CollabProxy,
this package has completely switched the UI to using UIElements. There is no more CEF, JS, or HTML.

The project is exclusively targeting .NetStandard 2.0 and will not work with the legacy Mono runtime.

The minimum supported version of the Unity Editor is 2020.1a13.

## Development
**For developers:**

Option 1: clone this repository out into the `packages/` directory in a project.

Option 2: clone elsewhere and link with the `packages/manifest.json` file in the project:
```
"com.unity.collab-proxy": "file:/some/path/to/package"
```
To add testing support also add the testibles section to the manifest. Your manifest should look like this:
```json
{
  "dependencies": {
    "com.unity.collab-proxy": "file:/some/path/to/package",
    ...
  },
  "testables": [
    "com.unity.collab-proxy",
    ...
  ]
}
```

**For internal testers:** simply add the git url into the `packages/manifest.json` file:
```
"com.unity.collab-proxy": "git://git@github.cds.internal.unity3d.com:unity/com.unity.cloud.collaborate.git"
```
If you need a specific revisision:
```
"com.unity.collab-proxy": "git://git@github.cds.internal.unity3d.com:unity/com.unity.cloud.collaborate.git#<rev>"
```
If you need more information, read the [Documentation](https://docs.unity3d.com/Manual/upm-dependencies.html#Git) for package dependencies from git.

Code style is as dictated in [Unity Meta](https://github.cds.internal.unity3d.com/unity/unity-meta).

There are IDE Specific code style configs under the `Config/` directory in the above repo.

## Overview
Source code for the packages is contained within the `Editor/`
and the tests are in `Tests/`. The structure of the package follows
the **MVP** pattern with a separate directory for each group of classes
and interfaces.

Here are some files and folders of note:
```none
<root>
  ├── package.json
  ├── README.md
  ├── CHANGELOG.md
  ├── LICENSE.md
  ├── Third Party Notices.md
  ├── QAReport.md
  ├── Editor/
  │   └── Collaborate
  │       ├── Unity.CollabProxy.Editor.asmdef
  │       ├── Assets/
  │       │   ├── Icons/
  │       │   ├── Layouts/
  │       │   ├── Styles/
  │       │   └── UiConstants.cs
  │       ├── Models/
  │       │   ├── Api/
  │       │   │   └── ISourceControlProvider.cs
  │       │   └── Providers/
  │       │       └── Collab.cs
  │       ├── Views/
  │       ├── Presenters/
  │       ├── Common/
  │       ├── Settings/
  │       ├── Components/
  │       ├── Utilities/
  │       └── UserInterface/
  │           ├── Bootstrap.cs
  │           ├── WindowCache.cs
  │           ├── ToolbarButton.cs
  │           └── CollaborateWindow.cs
  ├── Tests/
  │   ├── Collaborate
  │   │   └── Editor/
  │   │       └── Unity.CollabProxy.EditorTests.asmdef
  │   └── .tests.json
  └── Documentation~/
       ├── unity-cloud-collaborate.md
       └── Images/
```

- `Editor/Assets/` directory of the collaborate assets.
- `Editor/Assets/Icons/` directory for the collection of icons (png) used in the UI.
- `Editor/Assets/Layouts/` directory for the collection of layouts (uxml) used in the UI.
- `Editor/Assets/Styles/` directory for the collection of styles (uss) used in the UI.
- `Editor/Models/` directory of the models in the MVP architecture.
- `Editor/Models/Api/ISourceControlProvider.cs` interface for source control providers. Just Collab for now.
- `Editor/Models/Providers/Collab.cs` backend for providing the interface between this client and collab in the Unity Editor.
- `Editor/Views/` directory of the views in the MVP architecture.
- `Editor/Views/Adaptors/` directory for the list adaptors used in views.
- `Editor/Presenters/` directory of the presenters in the MVP architecture.
- `Editor/Components/` directory for the collection of UIElements components used in the UI.
- `Editor/UserInterface/` directory for the window and toolbar button source code.
- `Editor/UserInterface/Bootstrap.cs` code to bootstrap the toolbar button when the editor starts.
- `Editor/UserInterface/WindowCache.cs` code to cache the state of the window during domain reload.
- `Editor/UserInterface/ToolbarButton.cs` code to create and manage the collab button in the toolbar.
- `Editor/UserInterface/CollaborateWindow.cs` code for the window itself.
- `Tests/Editor/` directory of the client tests.

Each directory contains a README file with additional details about what is contained within them, including code
examples.

## Package Information
For more info on packages and best practices, visit the [package-starter-kit](https://github.cds.internal.unity3d.com/unity/com.unity.package-starter-kit) repository and read the documentation.

## Known Issues
* [COL-1079] The history window doesn't correctly distinguish local vs remote changes
* [COL-573] Publishing new versions of some packages in Collab results in Cannot Copy File error
* [COL-1083] Error message for opening diff tool on conflicted file when none are installed is not very helpful. Workaround is to install and select a supported diff tool in the Preferences->External Tools window.
* [COL-1084] Triggering a domain reload while Collab History tab is open disables the UX until the Editor is focused. Workaround is to click onto the Editor a second time.
* [COL-1085] Go Back To commit in 2020.1 with pre-v1.2.17 in package manifest breaks Collaborate window. Workaround is to open project in a version of Unity older than 2020.1.0a13 where pre-v1.2.17 packages are supported.
