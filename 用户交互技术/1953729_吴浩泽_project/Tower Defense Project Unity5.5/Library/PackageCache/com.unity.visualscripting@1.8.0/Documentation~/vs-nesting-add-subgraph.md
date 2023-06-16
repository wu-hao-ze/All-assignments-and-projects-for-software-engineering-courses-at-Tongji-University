# Add a Subgraph to a Script Graph

A Subgraph is a Script Graph nested inside of another Script Graph. A Subgraph appears as a single node inside a parent Script Graph. For more information on the Subgraph node, see [Subgraph node](vs-nesting-subgraph-node.md).

You can add a Subgraph to a Script Graph in two ways: create a new Script Graph, or add an existing Script Graph file.

## Add a new Subgraph to a Script Graph

To add a new blank Subgraph to an existing Script Graph: 

<ol>
<li><p>[!include[with-graph-open-ff](./snippets/vs-with-graph-open-ff.md)]</p></li>
<li><p>Go to <strong>Nesting</strong>.</p></li>
<li><p>Select <strong>Subgraph</strong> to add a Subgraph node to the graph.</p></li>
<li><p>Open the <a href="vs-interface-overview.md#the-graph-inspector">Graph Inspector</a>.</p></li>
<li><p>In the Graph Inspector, choose the source for the Subgraph:</p>
<ul>
    <li><p><strong>Embed</strong>: The Subgraph only exists on the Subgraph node. You can only change the Subgraph from the node in its parent graph.</p></li>
    <li><p><strong>Graph</strong>: The Subgraph exists in a separate file. You can change the Subgraph outside of its parent graph and reuse the graph in other areas of an application.</p></li>
</ul>
</li>
<li><p>If you chose <strong>Graph</strong>:</p>
<ol type="a">
    <li><p>In the Graph Inspector, select <strong>New</strong>.</p></li>
    <li><p>Enter a name for the graph file.</p></li>
    <li><p>Choose where you want to save the graph file in the project.</p></li>
    <li><p>Select <strong>Save</strong>.</p></li>
</ol>
</li>
</ol>

![An image of the Graph window, that displays a new blank Subgraph node added to a Script Graph](images/vs-blank-graph-subgraph-example.png)

## Add an existing Script Graph as a Subgraph

To add an existing graph file as a Subgraph in a Script Graph: 

> [!NOTE]
> You can't nest a Script Graph as a Subgraph in its own graph file. 

1. [!include[with-graph-open-ff](./snippets/vs-with-graph-open-ff.md)] 

1. Go to **Nesting**.

1. Select **Subgraph** to add the Subgraph node to the graph.

2. Open the [Graph Inspector](vs-interface-overview.md#the-graph-inspector).

1. In the Graph Inspector, set the **Source** to **Graph**. 

3. Do one of the following: 
    - In the **Graph** field, select the object picker (circle icon) and choose a compatible Script Graph from the project. 
    - Click and drag a Script Graph file from the Project window and release on the **Graph** field. 

![An image of the Graph window, that displays a new Subgraph node created from an existing Script Graph added to another Script Graph file.](images/vs-existing-graph-example-subgraph.png)

> [!TIP] 
> For a faster way to add a Script Graph as a Subgraph:
> - Click and drag a Script Graph asset from the Project window into the Graph Editor to automatically create a Subgraph node.
> - Right-click to open the fuzzy finder. Go to **Graphs** and select a graph file.

## Next steps

To open the new Subgraph and edit the graph, select **Edit Graph**. 

After you've added a Subgraph to a Script Graph, define its ports. For more information, see [Add a Trigger or Data port to a Script Graph](vs-nesting-add-triggers-data-graph.md).

