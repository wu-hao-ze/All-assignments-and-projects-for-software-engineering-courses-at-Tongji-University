# Add a Trigger or Data port to a Script Graph

A Script Graph used as a Subgraph can receive data and logic from its parent graph. Add and define ports on a graph to choose what data graphs can send and receive. 

For more information about Subgraphs, see [Subgraphs and State Units](vs-nesting-subgraphs-state-units.md).

![An image of the Graph Inspector, that displays the options for how to add ports to an open Script Graph.](images/vs-data-trigger-subgraph-ports.png)

## Add ports from a graph

To add a Trigger Input, Trigger Output, Data Input, or Data Output port to a Script Graph:

<ol>
<li><p><a href="vs-open-graph-edit.md">Open the Script Graph</a> you want to edit in the Graph window.</p></li>
<li><p>With no nodes or groups selected in the graph, open the <a href="vs-interface-overview.md#the-graph-inspector">Graph Inspector</a>.</p></li>
<li><p>Select <strong>Add</strong> (+) under the port type you want to add:</p>
<ul>
    <li><p><strong>Trigger Inputs</strong></p></li>
    <li><p><strong>Trigger Outputs</strong></p></li>
    <li><p><strong>Data Inputs</strong></p></li>
    <li><p><strong>Data Outputs</strong></p></li>
</ul></li>
<li><p>In the <strong>Key</strong> field, enter a unique key name for the port. The Key value can't match the Key of any existing ports on the current Script Graph.</p></li>
    <div class="NOTE"><h5>NOTE</h5><p>If two <strong>Key</strong> values are the same on the same graph, Visual Scripting ignores the second port definition and displays a warning in the Graph Inspector. If you change the <strong>Key</strong> value for a port after you've made a connection to that port in a graph, the connections break and you must reconnect them.</p></div>
<li><p>In the <strong>Label</strong> field, enter a label to display for the port. The label displays on the Subgraph node and its Input or Output node.</p></li>
    <div class="NOTE"><h5>NOTE</h5><p>If you don't set a <strong>Label</strong>, Visual Scripting uses the value from the <strong>Key</strong> field.</p></div>
<li><p>In the <strong>Summary</strong> field, enter a brief summary of the port to display in the Graph Inspector when you select the Subgraph node, Input node, or Output node.</p></li>
<li><p>Toggle <strong>Hide Label</strong> to do the following:</p>
<ul>
    <li><p>Enable <strong>Hide Label</strong> to hide the port label on any Subgraph node, Input node, or Output node.</p></li> 
    <li><p>Disable <strong>Hide Label</strong> to display the data from the <strong>Label</strong> field.</p></li>
</ul></li>
<li><p>(Data Inputs and Data Outputs Only) Set a data type for the port:</p>
<ol type="a">
    <li><p>Select the <strong>Type</strong> list to open the Type menu.</p></li>
    <li><p>Select a data type from the list to set the data type the port accepts.</p></li>
</ol>
</li>
<li><p>(Data Inputs Only) Enable <strong>Has Default Value</strong> to display the <strong>Default Value</strong> field. Disable <strong>Has Default Value</strong> to hide the <strong>Default Value</strong> field.</p>
<ol type="a">
    <li><p>In the <strong>Default Value</strong> field, enter the default value the port uses if it doesn't receive a data input while the Script Graph runs.</p></li>
</ol>
</li>
</ol>

## Add ports with Input and Output nodes

You can also use an [Input node](vs-nesting-input-node.md) or an [Output node](vs-nesting-output-node.md) to define ports on a Script Graph:

<ol>
<li><p><a href="vs-open-graph-edit.md">Open the Script Graph</a> you want to edit in the Graph window.</p></li>
<li><p>[!include[open-fuzzy-finder](./snippets/vs-open-fuzzy-finder.md)]</p></li>
<li><p>Go to <strong>Nesting</strong>.</p></li> 
<li><p>Do one of the following:</p>
    <ul>
        <li><p>To add a Trigger Input or Data Input port to the graph, select <strong>Input</strong>.</p></li>
        <li><p>To add a Trigger Output or Data Output port to the graph, select <strong>Output</strong>.</p></li>
    </ul>
</li>
<li><p>Select the new Input or Output node in the graph.</p></li>
<li><p>Open the <a href="vs-interface-overview.md#the-graph-inspector">Graph Inspector</a>.</p></li>
<li><p>In the <strong>Key</strong> field, enter a unique key name for the port. The Key value can't match the Key of any existing ports on the current Script Graph.</p></li>
    <div class="NOTE"><h5>NOTE</h5><p>If two <strong>Key</strong> values are the same on the same graph, Visual Scripting ignores the second port definition and displays a warning in the Graph Inspector. If you change the <strong>Key</strong> value for a port after you've made a connection to that port in a graph, the connections break and you must reconnect them.</p></div>
<li><p>In the <strong>Label</strong> field, enter a label to display for the port. The label displays on the Subgraph node and its Input or Output node.</p></li>
    <div class="NOTE"><h5>NOTE</h5><p>If you don't set a <strong>Label</strong>, Visual Scripting uses the value from the <strong>Key</strong> field.</p></div>
<li><p>In the <strong>Summary</strong> field, enter a brief summary of the port to display in the Graph Inspector when you select the Subgraph node, Input node, or Output node.</p></li>
<li><p>Toggle <strong>Hide Label</strong> to do the following:</p>
<ul>
    <li><p>Enable <strong>Hide Label</strong> to hide the port label on any Subgraph node, Input node, or Output node.</p></li> 
    <li><p>Disable <strong>Hide Label</strong> to display the data from the <strong>Label</strong> field.</p></li>
</ul></li>
<li><p>(Data Inputs and Data Outputs Only) Set a data type for the port:</p>
<ol type="a">
    <li><p>Select the <strong>Type</strong> list to open the Type menu.</p></li>
    <li><p>Select a data type from the list to set the data type the port accepts.</p></li>
</ol>
</li>
<li><p>(Data Inputs Only) Enable <strong>Has Default Value</strong> to display the <strong>Default Value</strong> field. Disable <strong>Has Default Value</strong> to hide the <strong>Default Value</strong> field.</p>
<ol type="a">
    <li><p>In the <strong>Default Value</strong> field, enter the default value the port uses if it doesn't receive a data input while the Script Graph runs.</p></li>
</ol>
</li>
</ol>

## Next steps

Add the Script Graph as a Subgraph in another Script Graph. For more information on how to add a Script Graph as a Subgraph, see [Add a Subgraph to a Script Graph](vs-nesting-add-subgraph.md). 

For more information on the port types on a Script Graph, see [Subgraph node](vs-nesting-subgraph-node.md). 

The defined Trigger and Data ports affect the ports on the Input and Output nodes in a Script Graph. For more information, see [Input node](vs-nesting-input-node.md) and [Output node](vs-nesting-output-node.md).