# Choose a control scheme

You can choose from two different control schemes in Visual Scripting. Each control scheme changes how you can interact with your graphs in the Graph Editor: 

|**Action** |**Default Control Scheme** |**Alternate Control Scheme** |
|:---|:---|:---|
|[**Pan**](#pan)| Middle-click and drag | Middle-click and drag |
|[**Pan Vertically**](#pan-vertically) | Scroll | N/A |
|[**Zoom In/Zoom Out**](#zoom-inzoom-out) | Ctrl+Scroll (macOS: Cmd+Scroll) | Scroll |
|[**Frame Selected**](#frame-selected) | Home | Home |
|[**Frame All**](#frame-all) | Home | Home |
|[**Create Selection**](#create-selection) | Click and drag | Click and drag |
|[**Select All**](#select-all) | Ctrl+A (macOS: Cmd+A)| Ctrl+A (macOS: Cmd+A) |
|[**Open Context Menu**](#open-context-menu)| Right-click <br/>Ctrl+click (macOS) <br/>Ctrl+E (macOS: Cmd+E) | Right-click <br/>Ctrl + click (MacOS) <br/>Ctrl+E (macOS: Cmd+E) |
|[**Create Node Group**](#create-node-group)| Ctrl+click and drag (macOS: Cmd+click and drag) | Ctrl+click and drag (macOS: Cmd+click and drag) |
|[**Copy Selected**](#copy-selected)| Ctrl+C (macOS: Cmd+C) | Ctrl+C (macOS: Cmd+C) |
|[**Paste Selection**](#paste-selection)| Ctrl+V (macOS: Cmd+V) | Ctrl+V (macOS: Cmd+V) |
|[**Cut Selected**](#cut-selected)| Ctrl+X (macOS: Cmd+X) | Ctrl+X (macOS: Cmd+X) |
|[**Duplicate Selected**](#duplicate-selected)| Ctrl+D (macOS: Cmd+D)| Ctrl+D (macOS: Cmd+D) |
|[**Delete Selected**](#delete-selected)| Del | Del |
|[**Maximize Graph Window**](#maximize-graph-window)| Shift+Space <br/>Double-click | Shift+Space <br/>Double-click |
|[**Move Group Without Child Nodes**](#move-group-without-child-nodes)| Alt+click and drag the group's Title bar | Ctrl+click and drag the group's Title bar (macOS: Cmd+click and drag)|
|[**Move Node on One Axis**](#move-node-on-one-axis)| Shift+click and drag vertically or horizontally | Shift+click and drag vertically or horizontally |


## Pan 

Pan to move the viewable area in the Graph Editor to any part of your graph. 

## Pan Vertically 

With the **Default** control scheme, pan the view in the Graph Editor vertically with the scroll wheel on your mouse. 

## Zoom In/Zoom Out 

Change the zoom level in the Graph window to control how much of your graph is visible in the Graph Editor. 

You can also set your zoom level with the toolbar in the Graph window. For more information, see [The interface](vs-interface-overview.md#the-graph-toolbar).

## Frame Selected 

After you select a node or another item in your graph, press Home to center your selected item in the Graph Editor.

![An image of the Graph window, with a Set Variable node selected and framed in the center of the Graph Editor.](images\vs-frame-selected.png)

## Frame All 

With no nodes or items selected, press Home to center your entire graph in the Graph Editor. Your zoom level automatically adjusts to accommodate the size of your graph. 

![An image of the Graph window, with its entire Script Graph displayed in the Graph Editor. The zoom level has been set to 0.4x to display the entire graph.](images\vs-frame-all.png)

## Create Selection

Click and drag to create a selection box around any nodes or items in your graph that you want to select. 

![An image of the Graph window, with a selection box created around 4 nodes in the Graph Editor](images\vs-create-selection.png)

When you have multiple items selected, click and drag a single item to move the entire selection. 

## Select All 

Press Ctrl+A (macOS: Cmd+A) to select all items in your current graph. 

## Open Context Menu

You can open the context menu to perform certain actions on State Graphs or manipulate a selection in a Script Graph. You can create new states and add transitions.

![An image of the Graph window. The context menu is open and displays the options for a selection in a Script Graph.](images\vs-context-menu.png)

## Create Node Group 

Create a group of nodes to keep related sections of your graph together, or move multiple nodes at a time. For more information on node groups, see [Create node groups](vs-groups.md).

![An image of the Graph window. A node group has been created around three nodes and named "Variable Input."](images\vs-node-group.png)

## Copy Selected 

Copy your current selection to move it to another graph, or another location on your current graph. 

## Paste Selection

Paste the contents of a copied or cut selection into your graph. 
## Cut Selected

Cut your current selection to move it to another graph, or another location on your current graph. 

## Duplicate Selected 

Duplicate a selection to instantly create a copy of your current selection to use elsewhere in your current graph. 

![An image of the Graph window. Three selected nodes have been duplicated and sit on top of the original nodes.](images\vs-duplicate-selection.png)

## Delete Selected

Delete your current selection to remove it from your graph. 

## Maximize Graph Window 

After you dock the Graph window in the Unity Editor, press Shift+Space or double-click maximize your Graph window and take up the entire Editor window. 

## Move Group Without Child Nodes 

You can move a group in your graph without any of the nodes contained inside that group. 

![An image of the Graph window with a node group moved to another location without its child nodes.](images\vs-move-group.png)

## Move Node on One Axis 

Shift+click to move a node in only one direction at a time in the Graph Editor. The node can move either vertically or horizontally. 