# Add the Inspectable attribute to a custom type

If you have access to the source code for a custom type, add the `[Inspectable]` attribute to its fields and classes. The `[Inspectable]` attribute makes these fields and classes accessible to the [Inspector window](https://docs.unity3d.com/Manual/UsingTheInspector.html) in the Unity Editor. You don't need to create a [custom PropertyDrawer](vs-create-custom-drawer.md) as Unity generates a basic UI for the custom type.

For more information about how to use custom types in Visual Scripting, see [Use a custom type](vs-using-custom-types.md) or [Custom types](vs-custom-types.md)

To add the `[Inspectable]` attribute to the source code for a custom type: 

1. [!include[open-existing-external-code](./snippets/vs-open-existing-external-code.md)]

2. In your external editor, on a line above your `public class` definition, add the `[Inspectable]` attribute.

1. On a line above the properties you want to have available in the Unity Inspector, add the `[Inspectable]` attribute. 

1. [!include[regen-node-library](./snippets/vs-regen-node-library.md)]

The following is an example of a public class, with fields `name` and `amount` that are accessible and can be modified through Unity's Inspector window. 

    ```csharp

    using System;
    using UnityEngine; 
    using Unity.VisualScripting;

    [Inspectable]
    public class MyClass
    {
        [Inspectable]
        public string name;

        [Inspectable]
        public int amount;

        public string dontShowThis;
    }

    ```
