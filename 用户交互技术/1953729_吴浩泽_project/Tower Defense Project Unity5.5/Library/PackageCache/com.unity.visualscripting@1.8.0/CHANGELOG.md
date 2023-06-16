# Changelog
All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [1.8.0] - 2022-11-03
### Fixed
- Fixed graphs being corrupted on deserialization if containing a node whose type cannot be found. [UVSB-2332](https://issuetracker.unity3d.com/product/unity/issues/guid/UVSB-2332)
- For nodes that support a default parameter for each of their inputs, detect and fix parameter renames [UVSB-1885](https://issuetracker.unity3d.com/product/unity/issues/guid/UVSB-1885)
- Fixed the problem that was preventing link.xml creation when building for Mono backend [UVSB-2348](https://issuetracker.unity3d.com/product/unity/issues/guid/UVSB-2348)
- Moved Events/MessageListeners files to a Listeners folder to avoid to exceed some OS path limit
- Fixed Grandient.mode serialization. Fix available for Unity 2021.3.9f1 or newer [UVSB-2356](https://issuetracker.unity3d.com/product/unity/issues/guid/UVSB-2356)
- Fixed Visual Scripting settings now only save to disk when modified 
- Fixed sub graphs being shown with broken connections on first load as of Unity 2021.2 [UVSB-2345](https://issuetracker.unity3d.com/product/unity/issues/guid/UVSB-2345)
- Fixed documentation links for Script Graph and State Graphs assets [UVSB-2422](https://issuetracker.unity3d.com/product/unity/issues/guid/UVSB-2422)

### Added
- Added confirmation popup when resetting project settings and editor preferences. [UVSB-2353](https://issuetracker.unity3d.com/product/unity/issues/guid/UVSB-2353)
- Added confirmation popup when resetting assemblies/types in project settings.
- Added Sticky Note for ScriptGraph and StateGraph.
- Nodes may now have a button which triggers a custom action in their inspector description.
- Nodes whose type cannot be found are now temporarily converted to dummy nodes until either their original type is defined again or the user replaces them.
- Support for parameter renaming in code used by API nodes

### Changed
- AOTStubs are now generated for all nodes regardless of whether they represent a runtime or editor member [UVS-2381](https://issuetracker.unity3d.com/product/unity/issues/guid/UVSB-2381)
- Increased zoom out distance in graphs.

## [1.7.8] - 2022-02-22
### Fixed
- Handle ReflectionTypeLoadException for TypeUtility to remove warning [BOLT-1900](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1900)
- Fixed drag inconsistency in Graph Variables [BOLT-2113](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-2113)
- Fixed exception after creating a graph from the Welcome Window on Linux [BOLT-1828](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1828)
- Fixed the Cooldown node not becoming "Ready" when the "Reset" port is triggered
- Fixed exception thrown after changing Hierarchy selection after removing Saved variable [BOLT-1919](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1919)
- Fixed old Bolt saved variables not loading when using a build created using a newer version of Visual Scripting [BOLT-2052](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-2052)
- Fixed a performance issue when using lots of Get/Set Scene variable nodes in an open graph
- Fixed zooming out in the Graph to be relative to the mouse cursor [BOLT-1667](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1667)
- Fixed a compilation error when migrating from Visual Scripting 1.7.6 to 1.7.7 with InputSystem-1.1.1 or below installed.
- Fixed a performance issue when using lots of Get/Set Scene variable nodes in an open graph
- Fixed default inspectors for nodes not appearing in the correct position after a connected node is deleted [BOLT-1457](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1457)
- Fixed Scene variables drag and drop in graph having wrong scope [BOLT-2247](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-2247)

### Changed
- Small optimization of load times involving generic types.
- Renamed ContinuousNumberDrawer.cs.cs to ContinuousNumberDrawer.cs [BOLT-2288](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-2288)

### Added
- TextMeshPro assembly is now added by default in Project Settings/Visual Scripting/Node Library
- Added highlight to new VS graph drop down items [BOLT-2205](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-2205)
- Added margins to the UI for project settings and editor preferences

## [1.7.7] - 2021-11-23
### Fixed
- Fix an NullException error that occurs when creating a Variable right after project initialization.
- Fix Visual scripting naming in Project Settings and listener.
- Scene is marked as dirty when a graph is created on a new or exiting GameObject [BOLT-1860](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1859)
- Fix Flow Variables missing icon
- Improved node regeneration speed
- Fix null texture error when switching platform after a build failure
- Fix null texture error when entering play mode
- Fix Linux build failing when run from command line
- Fix Editor Assemblies not detected correctly at Codebase initialization
- Fix Wait nodes naming inconsistency [BOLT-1886](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1886)
- Fix constant being stripped in IL2CPP builds [BOLT-1638](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1638)
- TryConvert<T> now returns true when the conversion was successful [BOLT-2105](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-2105)
- Fix Input system by using correct Input API [BOLT-2078](https://issuetracker.unity3d.com/issues/input-action-is-not-recognized-when-manipulating-canvas-text-using-visual-scripting)

## [1.7.6] - 2021-11-05
### Fixed
- Fixed a regression where AOT Stubs were not being generated correctly, causing AOT builds to fail when run.

## [1.7.5] - 2021-08-30
### Changed
- Removed unused Preferences
- Renamed preference "Update Units Automatically" to "Update Nodes Automatically"
- Reduced domain reload performance cost of visual scripting to 1ms or less when not actively used by a project

### Fixed
- Fixed an issue where uncaught exceptions were thrown in Debug builds of the Windows editor
- Fixed the missing arrow when the "Transition End Arrow" is on. [BOLT-1535](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1535)
- Fixed wrong graph is showed after creating script graph form selected object in "Welcome Screen"
- Fixed duplicate variable error. [BOLT-1569](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1569)
- Fixed 'ReadOnlySpan<>' does not exist in the namespace 'System'" error with AOT build. [BOLT-1648](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1648)
- Fixed jitter when the fuzzy window is on the bottom of the screen and the user scrolls [BOLT-1530](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1530)
- Fixed missing AOT prebuild step when building an IL2CPP project in batchmode [BOLT-1649](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1649)
- Restored a public icon set API in UnitPortDescription.cs that was by mistake
- Fixed il2cpp crash caused by a recursion of the machine states in itself when  AOTstubs is generating.[BOLT-1656](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1656)

## [1.7.3] - 2021-06-30
### Changed
- Removed unused Preferences
- Renamed preference "Update Units Automatically" to "Update Nodes Automatically"

### Fixed
- Fixed an issue where uncaught exceptions were thrown in Debug builds of the Windows editor
- Fixed custom units not appearing in the finder

## [1.7.2] - 2021-05-17
### Changed
- NotEquals node in non-scalar mode is now consistent with Equals

### Fixed
- Fixed long values not preserved in literal nodes.
- Fixed root icons in breadcrumbs in the graph editor window. [BOLT-1290](https://issuetracker.unity3d.com/issues/wrong-icons-when-opening-a-script-graph)
- Fixed graph nodes icons
- Fixed project settings will not show when looking for graphs
- Fixed exception when user double clicks on a graph
- Raise warnings at edit time when a MouseEvent node is used when targeting handheld devices instead of build time.

## [1.7.1] - 2021-05-07
### Removed
- For performance reasons, the BackgroundWorker attribute is now obsolete and won't have any effect. Use BackgroundWorker.Schedule() directly

### Changed
- Renamed the VSSettingsProvider assembly to Unity.VisualScripting.SettingsProvider.Editor
- Variables Saver GameObject no longer appears until a variable is created or changed. [BOLT-1343](https://jira.unity3d.com/browse/BOLT-1343)
- Renamed Singleton GameObjects created by Visual Scripting to use "VisualScripting ---" names.
- All internal plugin and product versions have been normalized to use the package version.
- NotEquals node in non-scalar mode is now consistent with Equals
- SuperUnits have been renamed into Subgraphs
- No longer have a hard dependency on any of the following built-in modules: ai, animation, particlesystem, physics, physics2d
- ScriptMachine is now displayed as "Script Machine" instead of "Flow Machine" in the Gizmo window.
- Update, Start, Fixed Update and Late Update nodes have been renamed into On Update, On Start, On Fixed Update and On Late Update.
- Moved project settings from Assets directory to the ProjectSettings directory in Unity projects
- Renamed control schemes to Default/Alternate
- The UI references to 'Unit' were changed to 'Node' without any change to the underlying types
- Nodes from Timeline, Cinemachine and InputSystem packages are now automatically included, with their assemblies part of the default assemblyOptions.
- Progress bar titles for initial node generation have been tweaked to better indicate that it is a one-time process
- Various optimizations to reduce the duration of domain reloads

### Added
- Added workflows to create new graphs directly from the Graph Window
- SetScriptGraph node
- SetStateGraph node
- Support for RenamedFrom attribute on enum members
- GetStateGraphs node
- GetScriptGraphs node
- GetScriptGraph node
- GetStateGraph node
- HasStateGraph node
- HasScriptGraph node

### Fixed
- Fixed the problem were on Linux the fuzzy window would remains above all others. [BOLT-1197](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1197)
- There is no more crash when the user navigates quickly between fuzzy finder levels on Linux [BOLT-1197](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1196)
- Fixed variable type turns to null when clicked outside of the graph
- Fixed rearranging variables, if type is not set, it sets to the type that is bellow it
- Lots of miscellaneous migration fixes and quality of life changes
- Fixed unexpected error when exceptions are thrown by flow graph units and caught by the TryCatch unit [BOLT-1392](https://issuetracker.unity3d.com/issues/graph-fails-with-recursion-error-trycatch-unit-catches-exception-from-throw-unit)

## [1.6.1] - 2021-03-30
### Fixed
- Fixed bug caused by Editor API transitioning from private to public

## [1.6.0] - 2021-03-23
### Changed
- Updated graph migration process

## [1.5.2] - 2021-03-05
### Changed
- User interface updated
- Names in different UI elements made to be more consistent with new naming schemes

## [1.5.1] - 2021-02-23
### Added
- Warn the user when an Input System Package event is referencing an action of the wrong type for that event
- A warning is raised when adding more than one Input unit in a SuperUnit
- "Open" inspector button and double clicking a graph in the project browser now opens the visual scripting editor
- A warning is raised when the step's default value of the For unit is set to 0.

### Fixed
- Fixed "Restore to Defaults" buttons in the Project Settings window
- Fixed ThreadAbortException when entering Play Mode while searching in the Fuzzy Finder
- Fixed Visual Scripting Preferences being searchable [BOLT-1218](https://issuetracker.unity3d.com/issues/visual-scripting-preferences-are-not-searchable-when-using-search-in-the-preferences-window)
- Fixed ScalarAdd unit migration from 1.4.13 to 1.4.14 and above
- Fixed Open the graph window no longer causes Unity UI to stop processing mouse clicks" [BOLT-1159](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1159),
- Fixed Fuzzy finder no longer blinks when trying to add a node [BOLT-1157](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1157),
- Fixed Fuzzy search no longer drops keyboard inputs and respond slowly [BOLT-1214](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1214),
- Fixed Fuzzy finder search window no longer remains above all other windows [BOLT-1197](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1197)"
- Fixed Dropdown icon is not clipped with TextField under "Get Variable"
- Fixed Scale groups when zoom is not at 1x
- Fixed graph getting corrupted when adding "Get Action Map" unit
- Fixed node description being sometimes clipped
- Fixed warnings overflow in the console when deleting and adding a boolean variable in the blackboard
- Fixed warnings when entering play mode when the "Script Changes While Playing" is set to Recompile And Continue Playing
- Fixed resize cursor rect on group when graph window is zoomed
- Fixed VisualScripting.Generated folder is removed when removing the VisualScripting package.
- Fixed error when executing "Fix Missing Scripts" in a HDRP project
- Visual Scripting Preferences spacing has been adjusted to avoid overlaps
- Fixed rendering of inactive ObjectFields
- Fixed sidebar (graph inspector/blackboard) resize when a vertical scrollbar is needed
- Fixed variable type reset to Enum when changing from Enum to GameObject when both Blackbaord and Variables inspector are displayed
- Help button in the visual scripting Assets and Behaviours inspector now link to the package documentation.
- FlowMachine type is now back in usable types.
- Fixed GraphPointerException occurs when nesting graph within itself [BOLT-1257](https://issuetracker.unity3d.com/issues/visual-scripting-graphpointerexception-occurs-when-nesting-graph-within-itself)
- Fixed RenamedFrom attribute does not function correctly on array references to a renamed type [BOLT-1149](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1149)
- Fixed error message when custom inspectors are generated
- Fixed missing succession for Cooldown. Output of Cooldown completed is treated as unentered.  [BOLT-725](https://issuetracker.unity3d.com/issues/bolt-1-output-of-cooldown-completed-is-treated-as-unentered)
- Fixed infinite loop when setting the For unit's step's default value to 0. Instead, the unit won't be executed and the exit output will be triggered directly.
- Fixed Object Variables tabs not updated when creating a Prefab
- Fixed console errors when deleting a Prefab with a Visual Script
- Fixed console errors when editing nested graphs during Play Mode
- Fixed console errors when opening the standalone profiler window

## [1.5.1-pre.5] - 2021-01-20
### Changed
- Removed code referring to an unused SceneManagement.PrefabStage API

## [1.5.1-pre.3] - 2020-12-07
### Added
- Added Visual Scripting as built-in package as of Unity 2021.1
- Added New Input System Support. You can import the Input System package, activate the back-end and regenerate units to use.
- Added AOT Pre-Compile to automatically run when building AOT platforms
- Improved UI for deprecated built-in nodes
- Added automatic unit generation the first time the graph window is opened
### Changed
- Switched to delivering source instead of pre-built .NET 3/4 assemblies
- Updated Documentation
- Renamed assemblies to match Unity.VisualScripting naming scheme (Ex: Bolt.Core -> Unity.VisualScripting.Core)
- Merged Ludiq.Core and Ludiq.Graphs into Unity.VisualScripting.Core
- Moved Setup Wizard contents from pop-up on Editor startup to Player Settings. You can change the default settings from "Player Settings > Visual Scripting"
- Renamed "Assembly Options" to "Node Library"
- Renamed "Flow Graph" to "Script Graph"
- Renamed "Flow Machine" to "Script Machine"
- Renamed "Macro" graphs to "Graph" in machine source configuration and "GraphAsset" in Assets
- Renamed "Control Input/Output" to "Trigger Input/Output"
- Renamed "Value Input/Output" to "Data Input/Output"
- Updated built-in nodes. The Fuzzy Finder still accepts earlier version names of nodes.
- Renamed "Branch" node to "If"
- Renamed "Self" node to "This"
- Deprecated the previous Add unit. The Sum unit has been renamed to Add.
- Updated Window Naming   
- Changed "Variables" window to "Blackboard"
- Changed "Graph" window to "Script Graph" and "State Graph"
- Updated Bolt Preferences
- Renamed Bolt Preferences to "Visual Scripting"
- Removed BoltEx
- Moved settings previously accessed from "Window > Bolt" to preferences
- Renamed Control Schemes from "Unity/Unreal" to "Default/Alternate" (Neither control scheme currently matches their respective editors' controls and will be updated in a future release)
- Consolidated Graph editor, Blackboard and Graph Inspector into a single window
- Updated Third-Party Notices
- Plugin version information has been removed from the Visual Scripting settings window. This information can be retrieved from the Package Manager.
### Fixed
- Corrected UGUI event management to trickle down correctly when the hierarchy contains a Unity Message Listener [BOLT-2](https://issuetracker.unity3d.com/issues/bolt-1-unity-message-listener-blocks-proper-trickling-of-ugui-events-in-hierarchies)
- Fixed backup failures with large projects [BOLT-10](https://issuetracker.unity3d.com/issues/bolt-1-backup-fails-to-complete)
- Fixed "Null Reference" when opening the Graph Window for the first time [BOLT-996](https://issuetracker.unity3d.com/issues/nullreferenceexception-when-graph-window-is-opened-on-a-new-project)
- Fixed IL2CPP build crash on startup [BOLT-1036](https://issuetracker.unity3d.com/issues/bolt-bolt-1-il2cpp-release-build-crashes-on-startup-when-there-is-at-least-1-node-present-in-a-graph)
- Fixed IL2CPP issue around converting certain managed types [BOLT-8](https://issuetracker.unity3d.com/issues/bolt-1-il2cpp-encountered-a-managed-type-which-it-cannot-convert-ahead-of-time)
- Fixed deserialization issues when undoing graphs with Wait nodes [BOLT-679](https://issuetracker.unity3d.com/issues/bolt-deserialization-error-and-nodes-missing-after-pressing-undo-when-update-coroutine-with-wait-node-is-present-in-graph)
- Fixed "SelectOnEnum" node behavior enums containing non-unique values e.g. "RuntimePlatform" [BOLT-688](https://issuetracker.unity3d.com/issues/select-on-enum-doesnt-work-with-the-runtimeplatform-enum)
