# Configure your preferences

You can configure specific preferences in Visual Scripting to control the behavior of the [Graph window](vs-interface-overview.md) and your nodes. 

To configure your preferences for Visual Scripting: 

1. Go to **Edit** &gt; **Preferences**. 
1. Select **Visual Scripting**. 

## Core preferences

The following preferences control general behaviors across all graph types in Visual Scripting. 

<table>
<thead>
<tr>
<th><strong>Preference</strong></th>
<th colspan="2"><strong>Description</strong></th>
</tr>
</thead>
<tbody>
<tr>
<td><strong>Dim Inactive Nodes</strong></td>
<td colspan="2">Enable <strong>Dim Inactive Nodes</strong> to dim any nodes in the <a href="vs-interface-overview.md#the-graph-editor">Graph Editor</a> that aren't connected to the logic flow in a graph. This provides you with a visual cue that a dimmed node isn't used in the graph in its current configuration. <br/>Disable <strong>Dim Inactive Nodes</strong> to display all nodes as active, regardless of their connection state. <div class="NOTE"><h5>NOTE</h5><p>You can also control this preference from the Graph toolbar. For more information, see <a href="vs-interface-overview.md#the-graph-toolbar">The interface</a>.</p></div></td>
</tr>
<tr>
<td><strong>Dim Incompatible Nodes</strong></td>
<td colspan="2">Enable <strong>Dim Incompatible Nodes</strong> to dim all nodes that don't have a compatible connection port when you create a new edge. <br/>Disable <strong>Dim Incompatible Nodes</strong> to display all nodes as active for a new edge.</td>
</tr>
<tr>
<td><strong>Show Variables Help</strong></td>
<td colspan="2">Enable <strong>Show Variables Help</strong> to display a brief explanation of the selected variable scope in the <a href="vs-interface-overview.md#the-blackboard">Blackboard</a>. <br/>Disable <strong>Show Variables Help</strong> to hide these explanations.</td>
</tr>
<tr>
<td><strong>Create Scene Variables</strong></td>
<td colspan="2">Enable <strong>Create Scene Variables</strong> to automatically create a <strong>Scene Variables</strong> GameObject with a <strong>Variables</strong> component and a Scene Variables script component after you create a <a href="vs-variables.md#variable-scopes">Scene variable</a>. A GameObject with these components is required to use Scene variables in a project. <br/>Disable <strong>Create Scene Variables</strong> to create these components on a GameObject manually.</td>
</tr>
<tr>
<td><strong>Show Grid</strong></td>
<td colspan="2">Enable <strong>Show Grid</strong> to display a grid on the background of the <a href="vs-interface-overview.md#the-graph-editor">Graph Editor</a>. <br/>Disable <strong>Show Grid</strong> to hide the grid.</td>
</tr>
<tr>
<td><strong>Snap to Grid</strong></td>
<td colspan="2">Enable <strong>Snap to Grid</strong> to force nodes to stick or snap to points on a grid in the <a href="vs-interface-overview.md#the-graph-editor">Graph Editor</a>. <br/>Disable <strong>Snap to Grid</strong> to move nodes freely and disable the snap-to-point behavior.</td>
</tr>
<tr>
<td><strong>Pan Speed</strong></td>
<td colspan="2">Set a <strong>Pan Speed</strong> to control how quickly the view in the <a href="vs-interface-overview.md#the-graph-editor">Graph Editor</a> moves when you pan vertically with the scroll wheel.</td>
</tr>
<tr>
<td><strong>Drag Pan Speed</strong></td>
<td colspan="2">Set a <strong>Drag Pan Speed</strong> to control how quickly the view in the <a href="vs-interface-overview.md#the-graph-editor">Graph Editor</a> moves when you move a node to the edge of the Graph window.</td>
</tr>
<tr>
<td><strong>Zoom Speed</strong></td>
<td colspan="2">Set a <strong>Zoom Speed</strong> to control how quickly the <a href="vs-interface-overview.md#the-graph-editor">Graph Editor</a> zooms in or zooms out while you change the zoom level in the Graph window. For more information on how to change the zoom level in the Graph Editor, see <a href="vs-control-schemes.md#zoom-inzoom-out">Choose a control scheme</a>.</td>
</tr>
<tr>
<td><strong>Overview Smoothing</strong></td>
<td colspan="2">Set an <strong>Overview Smoothing</strong> to control how gradually the <a href="vs-interface-overview.md#the-graph-editor">Graph Editor</a> zooms or pans after you select the <strong>Overview</strong> option in the <a href="vs-interface-overview.md#the-graph-toolbar">Graph toolbar</a>.</td>
</tr>
<tr>
<td><strong>Carry Children</strong></td>
<td colspan="2">Enable <strong>Carry Children</strong> to move all connected child nodes when you move a parent node in the <a href="vs-interface-overview.md#the-graph-editor">Graph Editor</a>. <br/>Disable <strong>Carry Children</strong> to only move the currently selected node in the Graph Editor. <br/> <div class="NOTE"><h5>NOTE</h5><p>You can also change this setting from the Graph toolbar in the Graph window. For more information, see <a href="vs-interface-overview.md#the-graph-toolbar">The interface</a>.</p></div></td>
</tr>
<tr>
<td><strong>Disable Playmode Tint</strong></td>
<td colspan="2">Enable <strong>Disable Playmode Tint</strong> to display all nodes in the Graph window as normal while the Unity Editor is in Play mode. <br/>Disable <strong>Disable Playmode Tint</strong> to add a tint to all nodes in the Graph window while the Editor is in Play mode. For more information on Play mode, see <a href="https://docs.unity3d.com/Manual/GameView.html">The Game view</a> in the Unity User Manual.</td>
</tr>
<tr>
<td rowspan="3"><strong>Control Scheme</strong></td>
<td colspan="2">Select a Visual Scripting control scheme. For more information, see <a href="vs-control-schemes.md">Choose a control scheme</a>.</td>
</tr>
<tr>
<td><strong>Default</strong></td>
<td>Use the Default Visual Scripting control scheme.</td>
</tr>
<tr>
<td><strong>Alternate</strong></td>
<td>Use the Alternate Visual Scripting control scheme.</td>
</tr>
<tr>
<td><strong>Clear Graph Selection</strong></td>
<td colspan="2">Enable <strong>Clear Graph Selection</strong> to clear any graph displayed in the Graph window after you select a GameObject with no set graph or graphs. <br/>Disable <strong>Clear Graph Window</strong> to keep the last displayed graph if the selected GameObject has no set graph assets.<br/> <div class="NOTE"><h5>NOTE</h5><p>Visual Scripting always updates the Graph window to display the set graph on a selected GameObject, regardless of your chosen <strong>Clear Graph Selection</strong> setting.</p></td>
</tr>
<tr>
<td><strong>Human Naming</strong></td>
<td colspan="2">Enable <strong>Human Naming</strong> to convert all displayed method names from camel case to title case. For example, <code>camelCase</code> becomes <code>Camel Case</code>. <br/>Disable <strong>Human Naming</strong> to leave all names in camel case.</td>
</tr>
<tr>
<td><strong>Max Search Results</strong></td>
<td colspan="2">Set a <strong>Max Search Results</strong> value to specify the maximum number of search results returned by <a href="vs-interface-overview.md#the-fuzzy-finder">the fuzzy finder</a> after you use the search bar.</td>
</tr>
<tr>
<td><strong>Group Inherited Members</strong></td>
<td colspan="2">Enable <strong>Group Inherited Members</strong> to group together inherited nodes from a parent or base class to your current search term in <a href="vs-interface-overview.md#the-fuzzy-finder">the fuzzy finder</a>. <br/>For example, an <code>Audio Source</code> is a <code>Component</code>: it has its own specific methods and nodes, but you can interact with it as a <code>Component</code> with <code>Component</code> nodes. While you perform a search in the fuzzy finder, Visual Scripting groups the nodes inherited from <code>Component</code> and displays them in grey. <br/>Disable <strong>Group Inherited Members</strong> to display nodes in the search results without grouping these inherited nodes.</td>
</tr>
<tr>
<td><strong>Developer Mode</strong></td>
<td colspan="2">Enable <strong>Developer Mode</strong> to display additional preferences in the Preferences window and add additional features in the Graph window and other areas of the Unity Editor. For more information on the additional Developer Mode preferences, see <a href="#additional-developer-mode-preferences">Additional Developer Mode preferences</a>.</td>
</tr>
<tr>
<td><strong>AOT Safe Mode</strong></td>
<td colspan="2">Enable <strong>AOT Safe Mode</strong> to exclude nodes from search results in <a href="vs-interface-overview.md#the-fuzzy-finder">the fuzzy finder</a> that might cause problems for platforms that require ahead of time (AOT) compilation. For example, Visual Scripting excludes nodes that use the <code>Generic</code> type. <br/>Disable <strong>AOT Safe Mode</strong> to display all nodes and types in the fuzzy finder.</td>
</tr>
</tbody>
</table>


## Script Graphs preferences 

The following preferences change the behavior of Script Graphs in the Graph window.

<table>
<thead>
<tr>
<th><strong>Preference</strong></th>
<th><strong>Description</strong></th>
</tr>
</thead>
<tbody>
<tr>
<td><strong>Update Nodes Automatically</strong></td>
<td><div class="NOTE"><h5>NOTE</h5><p>This feature is experimental.</p></div>Enable <strong>Update Nodes Automatically</strong> to let Visual Scripting automatically update your Node Library when it detects a change in any script inside your project's <strong>Assets</strong> folder. <br/>Disable <strong>Update Nodes Automatically</strong> to manually regenerate your Node Library after you make a change to a script. For more information on how to regenerate your Node Library, see <a href="vs-configuration.md">Configure project settings</a>.</td>
</tr>
<tr>
<td><strong>Predict Potential Null References</strong></td>
<td>A predictive debugging feature. Enable <strong>Predict Potential Null References</strong> to display warnings about potential <code>null</code> value inputs in your graphs. <br/>Disable <strong>Predict Potential Null References</strong> to disable these warnings. <br/> <div class="NOTE"><h5>NOTE</h5><p>Sometimes, predictive debugging might return false positive results when you enable this setting.</p></div></td>
</tr>
<tr>
<td><strong>Predict Potential Missing Components</strong></td>
<td>A predictive debugging feature. Enable <strong>Predict Potential Missing Components</strong> to display warnings about potential missing components in your graphs, such as a missing node input. <br/>Disable <strong>Predict Potential Missing Components</strong> to disable these warnings. <br/> <div class="NOTE"><h5>NOTE</h5><p>Sometimes, predictive debugging might return false positive results when you enable this setting.</p></div></td>
</tr>
<tr>
<td><strong>Show Connection Values</strong></td>
<td>Enable <strong>Show Connection Values</strong> to display the input and output values sent between nodes while the Editor is in Play mode. This can make it easier to debug your scripts. <br/>Disable <strong>Show Connection Values</strong> to hide these value labels while in Play mode. For more information on Play mode, see <a href="https://docs.unity3d.com/Manual/GameView.html">The Game view</a> in the User Manual. <div class="NOTE"><h5>NOTE</h5><p>You can also control this preference from the Graph toolbar. For more information, see <a href="vs-interface-overview.md#the-graph-toolbar">The interface</a>.</p></div></td>
</tr>
<tr>
<td><strong>Predict Connection Values</strong></td>
<td>Enable <strong>Predict Connection Values</strong> to have the Graph Editor predict what input and output values your graph sends between nodes while the Unity Editor is in Play mode. For example, Visual Scripting would display the value currently set for a variable in your script, though that value might change before it's used by a node. <br/>Disable <strong>Predict Connection Values</strong> to hide these predicted input and output values.</td>
</tr>
<tr>
<td><strong>Hide Port Labels</strong></td>
<td>Enable <strong>Hide Port Labels</strong> to hide the name labels for node input and output ports. <br/>Disable <strong>Hide Port Labels</strong> to display these name labels.</td>
</tr>
<tr>
<td><strong>Animate Control Connections</strong></td>
<td>Enable <strong>Animate Control Connections</strong> to display a droplet animation across node control port edges while the Editor is in Play mode. <br/>Disable <strong>Animate Control Connections</strong> to disable the animations. For more information about the different node port types and edges, see <a href="vs-nodes.md">Nodes</a>. For more information on Play mode, see <a href="https://docs.unity3d.com/Manual/GameView.html">The Game view</a> in the User Manual.</td>
</tr>
<tr>
<td><strong>Animate Value Connections</strong></td>
<td>Enable <strong>Animate Value Connections</strong> to display a droplet animation across node data port edges while the Editor is in Play mode. <br/>Disable <strong>Animate Value Connections</strong> to disable the animations. For more information about the different node port types and edges, see <a href="vs-nodes.md">Nodes</a>. For more information on Play mode, see <a href="https://docs.unity3d.com/Manual/GameView.html">The Game view</a> in the User Manual.</td>
</tr>
<tr>
<td><strong>Skip Context Menu</strong></td>
<td>Enable <strong>Skip Context Menu</strong> to always open <a href="vs-interface-overview.md#the-fuzzy-finder">the fuzzy finder</a> when you right-click in the Graph Editor. To access the context menu, use Shift+right-click. <br/>Disable <strong>Skip Context Menu</strong> to open the fuzzy finder when you right-click with no nodes or groups selected in the Graph Editor. The context menu opens when you right-click with a node or group selected.</td>
</tr>
</tbody>
</table>


## State Graphs preferences

The following preferences change the behavior of State Graphs in the Graph window. 
<table>
<thead>
<tr>
<th><strong>Preference</strong></th>
<th colspan="2"><strong>Description</strong></th>
</tr>
</thead>
<tbody>
<tr>
<td rowspan="8"><strong>States Reveal</strong></td>
<td colspan="2">Use the dropdown to choose when a Script State node displays a list of events from its graph.<br/>
If you have many Script State nodes in a State Graph, you might want to change this setting.</td>
</tr>
<tr>
<td><strong>Never</strong></td>
<td>Script State nodes never display their list of events.</td>
</tr>
<tr>
<td><strong>Always</strong></td>
<td>Script State nodes always display their list of events.</td>
</tr>
<tr>
<td><strong>On Hover</strong></td>
<td>Script State nodes only display their list of events when you hover over the node in the Graph window.</td>
</tr>
<tr>
<td><strong>On Hover with Alt</strong></td>
<td>Script State nodes only display their list of events when you hover over the node while you hold Alt.</td>
</tr>
<tr>
<td><strong>When Selected</strong></td>
<td>Script State nodes only display their list of events when you select the node in the Graph window.</td>
</tr>
<tr>
<td><strong>On Hover or Selected</strong></td>
<td>Script State nodes display their list of events when you hover over the node, or when you select the node in the Graph window.</td>
</tr>
<tr>
<td><strong>On Hover with Alt or Selected</strong></td>
<td>Script State nodes display their list of events when you hover over the node while you hold Alt, or when you select the node in the Graph window.</td>
</tr>
<tr>
<td rowspan="8"><strong>Transitions Reveal</strong></td>
<td colspan="2">Use the dropdown to choose when a transition displays a list of events from its graph.<br/>
If you have many transitions in a State Graph, you might want to change this setting.</td>
</tr>
<tr>
<td><strong>Never</strong></td>
<td>Transitions never display a list of events.</td>
</tr>
<tr>
<td><strong>Always</strong></td>
<td>Transitions always display a list of events.</td>
</tr>
<tr>
<td><strong>On Hover</strong></td>
<td>Transitions only display a list of events when you hover over the transition in the Graph window.</td>
</tr>
<tr>
<td><strong>On Hover with Alt</strong></td>
<td>Transitions only display a list of events when you hover over the transition while you hold Alt.</td>
</tr>
<tr>
<td><strong>When Selected</strong></td>
<td>Transitions only display a list of events when you select the transition in the Graph window.</td>
</tr>
<tr>
<td><strong>On Hover or Selected</strong></td>
<td>Transitions display a list of events when you hover over the transition, or when you select the transition in the Graph window.</td>
</tr>
<tr>
<td><strong>On Hover with Alt or Selected</strong></td>
<td>Transitions display a list of events when you hover over the transition while you hold Alt, or when you select the transition in the Graph window.</td>
<tr>
<td><strong>Transitions End Arrow</strong></td>
<td colspan="2">Enable <strong>Transitions End Arrow</strong> to add an arrow to the end of each transition edge in a State Graph. Disable <strong>Transitions End Arrow</strong> to display edges between transitions as simple lines. <br/>If you have many transitions in your State Graphs, you might want to disable this setting.</td>
</tr>
<tr>
<td><strong>Animate Transitions</strong></td>
<td colspan="2">Enable <strong>Animate Transitions</strong> to display a droplet animation across transition edges when the Editor is in Play mode. Disable <strong>Animate Transitions</strong> to disable the animations. For more information on Play mode, see <a href="https://docs.unity3d.com/Manual/GameView.html">The Game view</a> in the User Manual.</td>
</tr>
</tbody>
</table>

## Additional Developer Mode preferences

> [!NOTE]
> You can only access the following preferences after you have enabled **Developer Mode* in your [Core preferences](#core-preferences).

These Developer Mode preferences provide help with developing extensions or custom nodes for Visual Scripting. Their continued support in the Visual Scripting package isn't guaranteed. 

|**Preference** |**Description** |
|:---|:---|
|**Debug** | Enable **Debug** to add additional logging and visual overlays to help you debug element rendering in the Graph window. For example, if you created a custom node, use this setting to help debug your UI. <br/>Disable **Debug** to disable the logging and hide these overlays.|
|**Track Metadata State** | Enable **Track Metadata State** to add more information to logging. This can assist in debugging. <br/>Disable **Track Metadata State** to hide this additional information.|
|**Debug Inspector UI** | Enable **Debug Inspector UI** to add more overlays and additional details. The information available is greater than what Visual Scripting provides with the **Debug** setting, and affects more areas of the Editor's UI. Only enable this setting if you need more in-depth debugging feedback. <br/>Disable **Debug Inspector UI** to hide this information.|
