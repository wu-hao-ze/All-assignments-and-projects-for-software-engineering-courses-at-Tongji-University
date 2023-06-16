# Add or remove types

Type Options specify which node inputs and outputs Visual Scripting supports. After you [add a new assembly](vs-add-remove-node-library.md), you must add types specific to those nodes to your Type Options. Add types to make the nodes and their types accessible in the [fuzzy finder](vs-interface-overview.md#the-fuzzy-finder) and [the Blackboard](vs-interface-overview.md#the-blackboard).

You can't use a node that has an input or output type that isn't listed in your Type Options. 

## Add a type to your Type Options 

To add a new type to your Type Options list: 

1. [!include[open-project-settings](./snippets/vs-open-project-settings.md)]
1. Select **Visual Scripting**.
1. Expand **Type Options**. 
1. At the end of the types list, select **Add** (+).
1. In the new type entry, select **(No Type)** to open the **Type** menu. 
1. Select an available type from the **Type** menu. 

Visual Scripting adds the new type to your Type Options. To use nodes with the type in your project, [regenerate your Node Library](vs-configuration.md#Regen).

## Remove a type from your Type Options 

To remove a type from your Type Options list: 

1. [!include[open-project-settings](./snippets/vs-open-project-settings.md)]
1. Select **Visual Scripting**.
1. Expand **Type Options**. 
1. In the types list, locate the entry for the type you want to remove. 
1. Select **Remove** (-).

Visual Scripting removes the type from your Type Options. To make sure that your change appears in your project, [regenerate your Node Library](vs-configuration.md#Regen). 