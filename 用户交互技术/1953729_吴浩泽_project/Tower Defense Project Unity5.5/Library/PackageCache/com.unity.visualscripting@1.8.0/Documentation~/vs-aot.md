# Use Visual Scripting with Unity Cloud Build

At build time, Unity removes any code that isn't used by a project to reduce build size. This can cause problems with Visual Scripting because Unity can remove code that's necessary for Script Graphs to run in a project. 

For versions 1.7.x, Visual Scripting generates an `AotStubs.cs` file, which stores the Unity APIs that graphs use. With the `AotStubs.cs` file, Unity doesn't remove any Unity APIs used in a graph from a build. You can generate this file when you create a build of a project. 

Builds through Unity Cloud Build can fail because Cloud Build prevents domain reload between the prebuild and build phases of the project. Without a domain reload, the build doesn't include the generated `AotStubs.cs` file. For more information about domain reload, see [Domain Reloading](https://docs.unity3d.com/Manual/DomainReloading.html) in the Unity User Manual.

To build a Visual Scripting project with Cloud Build, do the following: 

1. Build the project locally for your desired platform. <br/>For more information on how to build a project, see the relevant section for each platform in [Platform development](https://docs.unity3d.com/Documentation/Manual/PlatformSpecific.html) in the User Manual. 

2. After the build, do one of the following: 
    - Open the project files in the system file explorer.
    - Open the [Project window](https://docs.unity3d.com/Manual/ProjectView.html) in the Unity Editor.

3. In the project files, go to **Assets** &gt; **Unity.VisualScripting.Generated** &gt; **VisualScripting.Core**.

4. Locate the `AotStubs.cs` file. 

5. Add the `AotStubs.cs` file to your source control system. 

Your Cloud Build settings might automatically trigger a new build of the project after you commit the `AotStubs.cs` file. For more information about version control systems and Cloud Build, see [Unity Cloud Build](https://docs.unity3d.com/Manual/UnityCloudBuild.html) in the User Manual.
