# Create a custom PropertyDrawer for a custom type

If you want to use a custom type from a custom class in Visual Scripting, and you don't have access to its source code, you must create a custom PropertyDrawer. 

You can't assign a value to a custom type inside the Editor or initialize the value for a variable with a custom type if it doesn't have a PropertyDrawer. 

> [!NOTE]
> The class for your custom type must have the `[Serializable]` tag in its source code to create a custom PropertyDrawer.

To create a custom PropertyDrawer: 

1. [!include[open-project-window](./snippets/vs-open-project-window.md)]

2. [!include[create-c-script-project](./snippets/vs-create-c-script-project.md)]

3. Enter a name, such as `CounterDrawer`, for the new script file. 

4. Press Enter.

5. [!include[open-new-external-code](./snippets/vs-open-new-external-code.md)]

6. Remove the `Start` and `Update` functions and their comments from the script file. 

7. Above the line that defines your new `public class`, add a `[CustomPropertyDrawer]` attribute.

8. In the parameters for the `[CustomPropertyDrawer]` attribute, specify a `type of` parameter with the name of the type you want to assign to this PropertyDrawer, exactly as it appears in Unity. 

9. Change the `MonoBehaviour` class at the end of your `public class` definition to `PropertyDrawer`.

> [!NOTE]
> After you create a custom PropertyDrawer, you must generate the required property provider scripts from your Visual Scripting Project Settings. For more information, see [Configure project settings](vs-configuration.md).

The following is an example of a finished PropertyDrawer script: 

```csharp
    using UnityEditor;
    using UnityEngine;
    [CustomPropertyDrawer](type of(<Counter>))]
    public class CounterDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
    
            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    
            // Don't indent child fields
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
    
            // Calculate rects
            var amountRect = new Rect(position.x, position.y, 30, position.height);
            var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
            var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);
    
            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("unit"), GUIContent.none);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
    
            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
    
            EditorGUI.EndProperty();
        }
    }
 ```

To create the rest of your custom PropertyDrawer, you must decide what fields you must display, and how you want them to display in the Editor's interface. For example, you might want to use the UIElements module to create your PropertyDrawer, or decide to use Unity's IMGUI module.

For more information on how to create and design a custom PropertyDrawer, see the [PropertyDrawer class](https://docs.unity3d.com/ScriptReference/PropertyDrawer.html) in the main Unity Scripting API and its related methods.

