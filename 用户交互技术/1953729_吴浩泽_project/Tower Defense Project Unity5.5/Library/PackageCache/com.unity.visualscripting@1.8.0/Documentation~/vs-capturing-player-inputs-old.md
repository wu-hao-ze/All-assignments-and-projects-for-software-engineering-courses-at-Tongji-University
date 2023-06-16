# Capture input with the Input Manager

> [!NOTE]
> You must configure your Project Settings to use the Input Manager with Visual Scripting. For more information, see [Input Manager prerequisites](vs-capture-player-input.md#input-manager-prerequisites).

To create a basic Script Graph that uses the Input Manager to capture input: 

<ol>
<li><p><a href="vs-open-graph-edit.md">Open</a> or <a href="vs-create-graph.md">create</a> a Script Graph attached to the GameObject that you want your users to move.</p></li>
<li><p>If there isn't an <strong>On Update</strong> or similar Event node in your graph:</p>
    <ol type="a">
        <li><p>[!include[open-fuzzy-finder](./snippets/vs-open-fuzzy-finder.md)]</p></li>
        <li><p>Go to <strong>Events</strong> &gt; <strong>Lifecycle</strong>, or enter <strong>On Update</strong> in the search field.</p></li>
        <li><p>Select the <strong>On Update</strong> Event node to add it to the graph.</p></li>
    </ol>
</li>
<li><p>[!include[open-fuzzy-finder](./snippets/vs-open-fuzzy-finder.md)]</p></li>
<div class="TIP"><h5>TIP</h5><p>If you right-click and the context menu appears, select <strong>Add Node</strong> to open the fuzzy finder.</p></div>
<li><p>Go to <strong>Codebase</strong> &gt; <strong>Unity Engine</strong> &gt; <strong>Input</strong>, or enter <strong>Get Axis</strong> in the search field.</p></li>
<li><p>Select <strong>Get Axis (Axis Name)</strong> to add the Get Axis node to the graph.</p></li>
<li><p>Repeat Steps 3 through 5 to create a second <strong>Get Axis (Axis Name)</strong> node.</p></li>
<li><p>On the first Get Axis node, in the <strong>Axis Name</strong> input field, enter <code>Horizontal</code>.</p></li>
<li><p>On the second Get Axis node, in the <strong>Axis Name</strong> input field, enter <code>Vertical</code>.</p></li>
<div class="NOTE"><h5>NOTE</h5><p>If an Axis Name doesn't match the name in the Input Manager's Project Settings, Visual Scripting displays an error in the Graph Inspector. When you enter Play mode, the Unity Editor also displays an error in the Console window.</p></div>
<li><p>[!include[open-fuzzy-finder](./snippets/vs-open-fuzzy-finder.md)]</p></li>
<li><p>Go to <strong>Codebase</strong> &gt; <strong>Unity Engine</strong> &gt; <strong>Transform</strong> or search for <strong>Translate</strong>.</p></li>
<li><p>Select <strong>Translate (X, Y, Z)</strong> to add a Translate node to the graph.</p></li>
<li><p>Select the <strong>Result</strong> float output port on the <code>Horizontal</code> Get Axis node.</p></li>
<li><p><a href="vs-creating-connections.md">Make a connection</a> to the <strong>X</strong> input port on the <strong>Translate</strong> node.</p></li>
<li><p>Select the <strong>Result</strong> float output port on the <code>Vertical</code> Get Axis node.</p></li>
<li><p><a href="vs-creating-connections.md">Make a connection</a> to the <strong>Z</strong> input port on the <strong>Translate</strong> node. <br/>The finished graph looks similar to the following image:</p></li>
<p><img src="images/vs-input-old-system-example.png" alt="An image of the Graph window, that displays the final result of a simple input capture graph using the Input Manager. An On Update node connects to the Trigger input port on a Transform Translate node. The Result port on an Input Get Axis node with an Axis Name of Horizontal connects to the X input port on the Translate node. The Result port on another Input Get Axis node with an Axis Name of Vertical connects to the Z input port."></p>
<li><p>To enter Play mode, select <strong>Play</strong> from the <a href="https://docs.unity3d.com/Manual/Toolbar.html">Unity Editor's Toolbar</a>.</p></li>
<li><p>While in the <a href="https://docs.unity3d.com/Manual/GameView.html">Game view</a>, press a key mapped as a <strong>Negative Button</strong> or <strong>Positive Button</strong> from the <a href="https://docs.unity3d.com/Documentation/Manual/class-InputManager.html">Input Manager's virtual axes</a>.</p></li>
</ol>

The GameObject moves along the X or Z axis in the Game view, based on the key pressed and the [Input Manager Project Settings](https://docs.unity3d.com/Documentation/Manual/class-InputManager.html). 

## Additional resources

- [Capture user input in an application](vs-capture-player-input.md)
- [Capture input with the Input System package](vs-capturing-player-inputs-new.md)
- [On Button Input node](vs-nodes-events-on-button-input.md)
- [On Keyboard Input node](vs-nodes-events-on-keyboard-input.md)
- [On Mouse Down node](vs-nodes-events-on-mouse-down.md)
- [On Mouse Drag node](vs-nodes-events-on-mouse-drag.md)
- [On Mouse Enter node](vs-nodes-events-on-mouse-enter.md)
- [On Mouse Exit node](vs-nodes-events-on-mouse-exit.md)
- [On Mouse Input node](vs-nodes-events-on-mouse-input.md)
- [On Mouse Over node](vs-nodes-events-on-mouse-over.md)
- [On Mouse Up As Button node](vs-nodes-events-on-mouse-up-button.md)
- [On Mouse Up node](vs-nodes-events-on-mouse-up.md)

