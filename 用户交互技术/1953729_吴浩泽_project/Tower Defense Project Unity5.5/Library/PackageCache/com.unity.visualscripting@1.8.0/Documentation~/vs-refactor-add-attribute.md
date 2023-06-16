# Add the RenamedFrom attribute to a C# script

To use nodes generated from a custom C# script in a project after you rename a member, class, struct, type, or enum, add the `[RenamedFrom]` attribute to the relevant API element in the script file. For more information on the `[RenamedFrom]` attribute, see [Refactor a C# script with Visual Scripting](vs-refactoring.md).

To add the attribute to a C# script:

<ol>
<li><p>[!include[open-project-window](./snippets/vs-open-project-window.md)]</p></li>
<li><p>In the Project window, double-click the C# script file you want to refactor. Unity opens the file in the program you specified in your preferences, under <strong>External Script Editor</strong>.</p>
<div class="NOTE"><h5>NOTE</h5><p>For more information on script editors in Unity, see <a href="https://docs.unity3d.com/Manual/ScriptingToolsIDEs.html">Integrated development environment (IDE) support</a> in the Unity User Manual.</p></div>
</li>
<li><p>In your external editor, do the following:</p>
<ol type="a">
<li><p>Add the <code>[RenamedFrom]</code> attribute above the definition of the part of the script you want to rename.</p></li>
<li><p>Add the element's old name as a string to the <code>[RenamedFrom]</code> attribute, as its parameter. For example:</p></li>
<pre><code class="lang-C#">
using UnityEngine; 
using Unity.VisualScripting; 

[RenamedFrom(&quot;Character&quot;)]
public class Player : MonoBehaviour
{
    [RenamedFrom(&quot;InflictDamage&quot;)]
    public void TakeDamage(int damage)
    {
        //...
    }
}
</code></pre>
</ol></li>
<li><p>[!include[save-script](./snippets/vs-save-script.md)]</p></li>
<li><p>[!include[return-unity](./snippets/vs-return-unity.md)]</p></li>
<li><p>[!include[regen-node-library](./snippets/vs-regen-node-library.md)]</p></li>
</ol>

> [!NOTE]
> If you change the namespace or namespaces used in your script, you must include the old namespace or namespaces to use the `[RenamedFrom]` attribute. 

## Next steps

Unity recommends that you leave the attribute in the script file, even after a successful recompile. Nodes that use your C# script no longer have errors related to a missing member, class, struct, type, or enum.

## Additional resources

- [Refactor a C# script with Visual Scripting](vs-refactor-add-attribute.md)
- [Configure project settings](vs-configuration.md)
- [Add or remove types from your Type Options](vs-add-remove-type-options.md)
- [Custom C# nodes](vs-create-custom-node.md)
- [Custom events](vs-custom-events.md)