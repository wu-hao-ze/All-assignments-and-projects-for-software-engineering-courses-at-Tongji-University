# Custom C# nodes 

You can create your own Custom C# node with a C# script. Use your node in Script Graphs to run your own custom logic, and provide the script file to other users to let them use it in their own Visual Scripting projects. 

You can create nodes that change the flow of logic in your graph, nodes that manipulate data, or both. You can also create [nodes for custom events](vs-custom-events.md).

To create a Custom C# node, you must:

1. [Create a C# script](#create-the-initial-c-script). 
2. [Add ports](#add-ports).
3. [Add logic](#add-logic).
4. [Add relations](#add-relations). 


## Create the initial C# script

All custom nodes start with a C# file.

After you create a C# file, you must regenerate your Node Library through your [project settings](vs-configuration.md). This allows Visual Scripting to recognize your Custom C# node and add it to [the fuzzy finder](vs-interface-overview.md#the-fuzzy-finder) for use in your project's graphs. 

After you regenerate your Node Library the first time, you only need to regenerate your Node Library again if you change the location of a node in the fuzzy finder. You can change the location through the [[UnitCategory] attribute](vs-create-custom-node-attributes-reference.md#unitcategory).

For more information on how to create your initial C# script, see [Create a new simple Custom C# node](vs-create-custom-node-empty.md).

## Add ports 

Add ports to a node to specify what triggers Visual Scripting to run logic in a node, and decide what data it sends or receives. For more information on ports and nodes, see [Nodes](vs-nodes.md#connections-and-ports).

### Port types 

Visual Scripting has four different port types that you can add to a node: 

- `ControlInput`: Provides a connection to a previous node, which tells Visual Scripting when to start the logic in a node. 

- `ControlOutput`: Provides a connection to another node, which tells Visual Scripting when to run the logic for the next connected node in a Script Graph.  

- `ValueInput`: Provides a connection that allows you to pass data into a node for use in its logic. 

- `ValueOutput`: Provides a connection that allows you to pass data out of a node for use in other nodes. 

You can add any number of ports to a node. You can also choose what data type the `ValueInput` or `ValueOutput` ports send and receive: 

- **Generic**: The port can receive or output any data type. Corresponds to Unity's **Object** type.   

- **Type Value**: The port can only receive or output a specific data type. For example, `string`, `float`, or `integer`. 

For more information on types in Visual Scripting, see [Object types](vs-types.md). For more information on how to add ports to your node, see [Add ports to your Custom C# node](vs-create-custom-node-add-ports.md).

## Add logic 

Add logic to a node to specify what it does when it runs in a Script Graph. If there isn't any internal logic written for a node, the node can't trigger another node, or modify any of the data it receives from other nodes in a Script Graph. 

For more information on how to add logic to a node, see [Add logic to a Custom C# node](vs-create-custom-node-add-logic.md).

## Add relations

Relations help define how a node and its Play mode animations appear in Visual Scripting. Without relations, Visual Scripting doesn't know how to animate or display a node in the [Graph Editor](vs-interface-overview.md#the-graph-editor).

### Relation types 

You can add three types of relations to a node to help correctly display its internal flow of logic: 

- **Assignment**: Assignment relations are usually between a control input port and a data output port. Set an Assignment relation to tell Visual Scripting that a specific port needs to run before the node sends data to a data output port. 

- **Succession**: Succession relations are usually between a control input port and a control output port. Set a Succession relation to tell Visual Scripting that a control input port exits at a control output port. 

- **Requirement**: Requirement relations are usually between a control input port and a data port or ports. Set a Requirement relation to let Visual Scripting know that a specific data port or ports must have a value before the node can run any logic. 

> [!NOTE]
> If you don't set any Succession relations in a node, and **Dim** is enabled in [the Graph toolbar](vs-interface-overview.md#the-graph-toolbar), Visual Scripting dims your Custom C# node and any of its connected nodes in the Graph Editor during Play mode. 

For more information on how to add relations to a node, see [Add relations to a node](vs-create-custom-node-add-relations.md).

## Optional steps and customization 

After you've added relations, you can choose to [add documentation for a node](vs-create-custom-node-add-docs.md). Visual Scripting displays documentation in the [Graph Inspector](vs-interface-overview.md#the-graph-inspector) when a user selects a node in a Script Graph. Add documentation to help other users understand the purpose of each part of a node. 

You can also choose to [customize a node with node attributes](vs-create-custom-node-attributes-reference.md). You can add specific [node class attributes](vs-create-custom-node-attributes-reference.md#node-class-attributes) to customize the entire node, or add [port attributes](vs-create-custom-node-attributes-reference.md#port-attributes) to only customize specific ports. The attribute type determines the location where you must place the attribute in the node's C# file. Node class attributes must be placed above a node class definition, while port attributes must be placed above the variable definition for each port. 
