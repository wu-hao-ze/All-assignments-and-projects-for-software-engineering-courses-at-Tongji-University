# Create node groups

You can organize the nodes in your Script Graphs and State Graphs with node groups.

![An image of the Graph window. The Graph Inspector is open, and a group named "Compare gold" is selected. The group is set to the default black color, doesn't have a comment, and contains a Set Variable node that retrieves the value of the Gold Saved variable, a Comparison node, and an Integer node with a value of 5.](images/vs-groups.png)

## Create a new group 

To create a new group:

1. [Open the graph](vs-open-graph-edit.md) where you want to create a new group. 

1. In an empty area of the Graph Editor, Ctrl+click (macOS: Cmd+click) and drag to create a selection. 

1. Release the mouse to create the group.

After you create a group, you can: 

- [Change the group name](#change-a-group-name)
- [Add a comment to the group](#add-a-comment-to-a-group)
- [Change the color of the group](#change-the-color-of-a-group)
- [Resize the group](#resize-a-group)
- [Move the group](#move-a-group)

## Change a group name

To change the name of the group in your graph: 

2. In the group header, click **Group**. 

1. Enter a new name for the group.

1. Press **Enter** to save your changes. 

## Add a comment to a group

To add a comment to a group in a graph: 

1. Select the group where you want to add a comment. 
 
1. Open the [Graph Inspector](vs-interface-overview.md#the-graph-inspector). 

1. In the **(Comment)** field in the Graph Inspector, enter comments or information about the group. 

> [!NOTE]
> Group comments are only visible in the Graph Inspector. To add comments to a graph that are always visible, use a [Sticky Note](vs-sticky-notes.md).

## Change the color of a group 

To change the color of a group in a graph: 

1. Select the group you want to edit. 

1. Open the [Graph Inspector](vs-interface-overview.md#the-graph-inspector). 

1. In the Graph Inspector, select the **Color** field.

1. Select a new color for your group through one of the following methods: 
    
    - Use the color picker. 
    - Use the sliders or RGBA value fields. 
    - Enter a hexadecimal color value. 
    - Select the eyedropper icon and select a color from anywhere on your screen. 

## Resize a group 

To resize a group in a graph: 

1. Click and drag from any edge or corner on the group. 

## Move a group 

To move a group and its nodes: 

1. Click and drag the group's header to a new location in the Graph Editor. 

> [!NOTE]
> You can also move a group without moving any of the nodes inside, but the required input changes based on your chosen control scheme. For more information, see [Choose a control scheme](vs-control-schemes.md)

## Next steps

After you've created a node group, you can [add nodes to your Script Graph](vs-add-node-to-graph.md), [create and add variables](vs-add-variable-graph.md), or [add a Subgraph](vs-nesting-add-subgraph.md).

You can also add a [Sticky Note](vs-sticky-notes.md) to add comments to a graph.