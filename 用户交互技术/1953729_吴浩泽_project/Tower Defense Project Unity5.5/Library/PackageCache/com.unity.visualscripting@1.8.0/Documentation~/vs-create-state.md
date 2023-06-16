# Create a new state 

You can create three types of State nodes in a State Graph: [Script States](#create-a-script-state), [Any States](#create-an-any-state), and [Super States](#create-a-super-state). For more information on the types of State nodes, see State Graphs in [Graphs](vs-graph-types.md#state-graphs).

You can also add a [Sticky Note](vs-sticky-notes.md) to add comments to a graph.

## Create a Script State

To create a new blank Script State: 

<ol>
<li><p>[!include[open-state-menu](./snippets/vs-open-state-menu.md)]</p></li>
<li><p>Select <strong>Create Script State</strong>. <br/>Visual Scripting creates a new Script State node.</p></li>
<li><p>Open the <a href="vs-interface-overview.md#the-graph-inspector">Graph Inspector</a>.</p></li>
<li><p>In the Graph Inspector, choose a source for the Script State node:</p>
<ul>
<li><p><strong>Embed</strong>: The graph only exists on the Script State node. You can only modify the graph from the node in its parent State Graph.</p></li>
<li><p><strong>Graph</strong>: The graph exists in a separate file. You can modify the graph outside of its parent State Graph and reuse the graph in other areas of your application.</p></li>
</ul>
</li>
<li><p>If you chose <strong>Graph</strong>:</p>
<ol type="a">
<li><p>Select <strong>New</strong>.</p></li>
<li><p>Enter a name for the graph file.</p></li>
<li><p>Choose where you want to save the new graph.</p></li>
<li><p>Select <strong>Save</strong>.</p></li>
</ol></li>
</ol>

![An image of a State Graph with a new blank Script State node.](images/vs-blank-graph-script-state-example.png)


To create a Script State from an existing Script Graph: 

<ol>
<li><p>[!include[open-state-menu](./snippets/vs-open-state-menu.md)]</p></li>
<li><p>Select <strong>Create Script State</strong>. <br/>Visual Scripting creates a new Script State node.</p></li>
<li><p>Open the <a href="vs-interface-overview.md#the-graph-inspector">Graph Inspector</a>.</p></li>
<li><p>In the Graph Inspector, set the source for the Script State node to <strong>Graph</strong>.</p></li>
<li><p>Do one of the following:</p>
<ul>
<li><p>Select the object picker (circle icon) and choose a compatible Script Graph from your project.</p></li>
<li><p>Click and drag a Script Graph file from your Project window and release on the <strong>Graph</strong> field.</p></li>
</ul></li>
</ol>

![An image of a State Graph with a Script State node with an existing Script Graph.](images/vs-existing-graph-example-script-state.png)

> [!TIP]
> Click and drag the Script Graph from your Project window into the Graph Editor to automatically create a Script State node. 


## Create an Any State 

To create a new Any State node: 

1. [!include[open-state-menu](./snippets/vs-open-state-menu.md)] 

2. Select **Create Any State**. 

![An image of a State Graph with a new Any State node](images/vs-states-any-state-node.png)


## Create a Super State 

To create a new blank Super State: 

<ol>
<li><p>[!include[open-state-menu](./snippets/vs-open-state-menu.md)]</p></li>
<li><p>Select <strong>Create Super State</strong>. <br/>Visual Scripting creates a new Super State node.</p></li>
<li><p>Open the <a href="vs-interface-overview.md#the-graph-inspector">Graph Inspector</a>.</p></li>
<li><p>In the Graph Inspector, choose a source for the Super State node:</p>
<ul>
<li><p><strong>Embed</strong>: The graph only exists on the Super State node. You can only modify the graph from the node in its parent State Graph.</p></li>
<li><p><strong>Graph</strong>: The graph exists in a separate file. You can modify the graph outside of its parent State Graph and reuse the graph in other areas of your application.</p></li>
</ul>
</li>
<li><p>If you chose <strong>Graph</strong>:</p>
<ol type="a">
<li><p>Select <strong>New</strong>.</p></li>
<li><p>Enter a name for the graph file.</p></li>
<li><p>Choose where you want to save the new graph.</p></li>
<li><p>Select <strong>Save</strong>.</p></li>
</ol></li>
</ol>

![An image of a State Graph with a new blank Super State node.](images/vs-blank-graph-super-state-example.png)

To create a Super State from an existing State Graph: 

<ol>
<li><p>[!include[open-state-menu](./snippets/vs-open-state-menu.md)]</p></li>
<li><p>Select <strong>Create Super State</strong>. <br/>Visual Scripting creates a new Super State node.</p></li>
<li><p>Open the <a href="vs-interface-overview.md#the-graph-inspector">Graph Inspector</a>.</p></li>
<li><p>In the Graph Inspector, set the source for the Super State node to <strong>Graph</strong>.</p></li>
<li><p>Do one of the following:</p>
<ul>
<li><p>Select the object picker (circle icon) and choose a compatible State Graph from your project.</p></li>
<li><p>Click and drag a State Graph file from your Project window and release on the <strong>Graph</strong> field.</p></li>
</ul></li>
</ol>

![An image of a State Graph with a Super State node with an existing State Graph.](images/vs-existing-graph-example-super-state.png)

> [!TIP]
> Click and drag the State Graph from your Project window into the Graph Editor to automatically create a Super State node. 
