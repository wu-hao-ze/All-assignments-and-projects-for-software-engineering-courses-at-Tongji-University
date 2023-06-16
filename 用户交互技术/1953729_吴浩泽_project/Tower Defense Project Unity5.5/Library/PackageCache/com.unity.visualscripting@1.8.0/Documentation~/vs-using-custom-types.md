# Use a custom type

Visual Scripting supports every class and struct type available in Unity. By default, the most common are available in [the fuzzy finder](vs-interface-overview.md#the-fuzzy-finder). Add additional Unity assemblies, such as custom types and classes, through your [project settings](vs-configuration.md).

You must write some additional code to use a custom type or class in a graph. You can't assign a value to a variable with a custom type from Unity's [Inspector window](https://docs.unity3d.com/Manual/UsingTheInspector.html), or initialize it from inside the Unity Editor if this additional code isn't available. You must assign a default value for a custom type through the Inspector window to use that type for a variable in Visual Scripting.

You have two options to enable variable assignment and initialization: 

- If you have access to the source code, you can add the `[Inspectable]` attribute to the classes and fields that you want to display and modify in the Editor. 
- If you don't have access to the source code, you must create a custom PropertyDrawer and generate the required property provider scripts.

## Add the [Inspectable] attribute

Add the `[Inspectable]` attribute to the code for your custom class to display its available properties in the Inspector window and Visual Scripting's [Graph Inspector](vs-interface-overview.md#the-graph-inspector). You can't view your classes and fields in the Inspector window without the `[Inspectable]` attribute. 

Unity provides a basic UI for your types in the Inspector window, which might not give the aesthetic results you want. If you or your users want to configure a property for a custom type with a slider, for example, don't use the `[Inspectable]` attribute method. For more information on how to add the `[Inspectable]` attribute to a custom class, see [Add the Inspectable attribute to the source code for a custom type](vs-add-inspectable-attribute-custom-types.md).

## Create a custom PropertyDrawer

Create a custom PropertyDrawer to choose how to display each property for a custom class in the Inspector window. 

Without access to the source code, you must create a PropertyDrawer to interact with custom-typed variables in Visual Scripting. If you see an error in the Unity Editor's Inspector window when you try to use a type from a third-party package, you must create a custom PropertyDrawer.

> [!NOTE]
> If you are a package developer, or plan to provide your custom classes and types to other users and want those types to be available in Visual Scripting, create a custom PropertyDrawer to get the best results for your users. 

For more information on how to create a custom PropertyDrawer, see [Create a custom PropertyDrawer for a custom type](vs-create-custom-drawer.md).

After you create a custom PropertyDrawer for a custom type, you must generate the necessary property provider scripts. For more information, see the **Generate** option in [Configure project settings](vs-configuration.md).

