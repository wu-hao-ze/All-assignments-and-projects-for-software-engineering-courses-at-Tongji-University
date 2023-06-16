# Custom C# node attributes reference 

You can add attributes to a node class and port variable definitions to customize the look of a Custom C# node. 

## Node class attributes

You can customize the titles that appear on a node, where it appears in the fuzzy finder, and its icon. Node class attributes must be placed above the node class definition in a node's C# script. 

Visual Scripting has 5 node class attributes: 

- [UnitTitle](#unittitle)
- [UnitShortTitle](#unitshorttitle)
- [UnitSubtitle](#unitsubtitle)
- [UnitCategory](#unitcategory)
- [TypeIcon](#typeicon)


Usually, Visual Scripting automatically applies any changes you make to a node's class attributes after you save the C# file. 

### UnitTitle

![An image of the Graph window. The opened Script Graph has two groups that each contain a single Custom C# node: a yellow Before group and a purple After group. In the Before group, the title of the Custom C# node is "My Node." In the After group, the title of the Custom C# node is "My New Title."](images/vs-my-node-custom-node-unit-title.png)

You can specify a `[UnitTitle]` to display a different title than the node's class name on the node when it appears in a Script Graph, and when you view details about the node in the Graph Inspector: 

```C#
using System;
using Unity.VisualScripting;
using UnityEngine;


[UnitTitle("My New Title")]
public class MyNodeAfter : Unit
{
    ...

   }
}
```

The `[UnitTitle]` attribute overrides the node's class name. 

### UnitShortTitle

![An image of the Graph window. The opened Script Graph has two groups that each contain a single Custom C# node: a yellow Before group and a purple After group. In the Before group, the title of the Custom C# node is "My Node." In the After group, the title of the Custom C# node is "Short Title."](images/vs-my-node-custom-node-short-title.png)

You can specify a `[UnitShortTitle]` to display a different title on the node when it appears in a Script Graph:

```C#
using System;
using Unity.VisualScripting;
using UnityEngine;


[UnitShortTitle("Short Title")]
[UnitTitle("My New Title")]
public class MyNodeAfter : Unit
{
    ...

   }
}
```

The `[UnitShortTitle]` only appears on the node in a Script Graph. The `[UnitTitle]` or node class name still displays in the Graph Inspector.

### UnitSubtitle

![An image of the Graph window. The opened Script Graph has two groups that each contain a single Custom C# node: a yellow Before group and a purple After group. In the Before group, the Custom C# node doesn't have a subtitle. In the After group, the Custom C# node has a subtitle of "It's a subtitle!" that displays underneath the node title.](images/vs-my-node-custom-node-subtitle.png)

You can add a `[UnitSubtitle]` to add a line of text below the `[UnitTitle]`, `[UnitShortTitle]`, or node class name when a node appears in a Script Graph: 

```C#
using System;
using Unity.VisualScripting;
using UnityEngine;


[UnitSubtitle("It's a subtitle!")]
[UnitShortTitle("Short Title")]
[UnitTitle("My New Title")]
public class MyNodeAfter : Unit
{
    ...

   }
}
```

The `[UnitSubtitle]` doesn't appear in the Graph Inspector.

### UnitCategory

![An image of the Graph window. The opened Script Graph has two groups that each contain an open fuzzy finder: a yellow Before group and a purple After group. In the Before group, the fuzzy finder displays the Custom C# node, My Node, at the end of the top level of available nodes and node categories in the fuzzy finder. In the After group, the Custom C# node, My Node, has moved to the My Nodes category.](images/vs-my-node-custom-node-fuzzy-finder-level.png)

You can specify a `[UnitCategory]` to tell Visual Scripting where to place the node in the fuzzy finder: 

```C#
using System;
using Unity.VisualScripting;
using UnityEngine;


[UnitCategory("FirstLevel/SecondLevel")]
public class MyNodeAfter : Unit
{
    ...

   }
}
```

Replace `FirstLevel` with the name of the top-level category in the fuzzy finder where you want Visual Scripting to place the node. Replace `SecondLevel` with the name of a subcategory. Visual Scripting creates the categories if they don't already exist in the fuzzy finder. 

> [!NOTE]
> You must [regenerate your Node Library](vs-configuration.md) for changes made to a node's `[UnitCategory]` to take effect. 

### TypeIcon

![An image of the Graph window. The opened Script Graph has two groups that each contain a single Custom C# node: a yellow Before group and a purple After group. In the Before group, the Custom C# node, My Node, displays with the standard icon for a custom node. The icon is a white rectangle with two white circles attached to the left side, and one white circle on the right, as a simplified representation of a node. In the After group, the Custom C# node has a ToggleValue icon. The ToggleValue icon has two white circles beside a rounded, hollow rectangle with a green circle at the bottom, to represent a switch or toggle.](images/vs-my-node-custom-node-type-icon.png)

You can use the `[TypeIcon]` attribute to change the icon that appears on a node when it appears in a Script Graph: 

```C#
using System;
using Unity.VisualScripting;
using UnityEngine;


[TypeIcon(typeof(ToggleValue))]
public class MyNodeAfter : Unit
{
    ...

   }
}
```
The icon for the node changes in the Graph Inspector, too. 

> [!NOTE]
> You can't point to your own custom icons from this attribute. You must use an icon from the Visual Scripting icons library, which includes all Unity types. 


## Port attributes 

Custom nodes have one mandatory port attribute and one optional port attribute: [DoNotSerialize](#donotserialize) and [PortLabelHidden](#portlabelhidden), respectively. Port attributes must be placed above your variable declarations for each port variable in the node. 

Visual Scripting automatically applies any changes you make to a node's port attributes after you save the script file.

### DoNotSerialize

`[DoNotSerialize]` is a mandatory attribute for all ports on custom nodes. Add this attribute to avoid serialization of data that shouldn't be serialized: 

```C#
using System;
using Unity.VisualScripting;
using UnityEngine;


[UnitShortTitle("Short Title")]
[UnitTitle("My New Title")]
[UnitCategory("My Nodes")]
[UnitSubtitle("It's a subtitle!")]
[TypeIcon(typeof(Color))]

public class MyNodeAfter : Unit
{
   [DoNotSerialize]
   public ControlInput inputTrigger;

   [DoNotSerialize]
   public ControlOutput outputTrigger;

   [DoNotSerialize]
   public ValueInput myValueA;

   [DoNotSerialize]
   public ValueInput myValueB;

   [DoNotSerialize]
   public ValueOutput result;

   private string resultValue;

   protected override void Definition()
   {
       ...
   }
}
```

### PortLabelHidden

![An image of the Graph window. The opened Script Graph has two groups that each contain a single Custom C# node: a yellow Before group and a purple After group. In the Before group, every label for each port on the Custom C# node, My Node, displays on the node. In the After group, all labels for every port on the node are hidden and are only represented by their port type icons.](images/vs-my-node-custom-node-port-labels-hidden.png)

You can add the `[PortLabelHidden]` attribute to hide the name label for any port on a node when it appears in a Script Graph: 

```C#
using System;
using Unity.VisualScripting;
using UnityEngine;


[UnitShortTitle("Short Title")]
[UnitTitle("My New Title")]
[UnitCategory("My Nodes")]
[UnitSubtitle("It's a subtitle!")]
[TypeIcon(typeof(Color))]

public class MyNodeAfter : Unit
{
   [DoNotSerialize]
   [PortLabelHidden]
   public ControlInput inputTrigger;

   [DoNotSerialize]
   [PortLabelHidden]
   public ControlOutput outputTrigger;

   [DoNotSerialize]
   public ValueInput myValueA;

   [DoNotSerialize]
   public ValueInput myValueB;

   [DoNotSerialize]
   public ValueOutput result;

   private string resultValue;

   protected override void Definition()
   {
       ...
   }
}
```

The port's label is still visible in the Graph Inspector. Use the same name in a port's variable definition and the port's key in the `Definition` method for the node's class, as shown: 

```C#
using System;
using Unity.VisualScripting;
using UnityEngine;

public class MyNode : Unit
{
    ...

   [DoNotSerialize, PortLabelHidden]
   public ValueInput myValueA;

   [DoNotSerialize, PortLabelHidden]
   public ValueInput myValueB;

    ...

   protected override void Definition()
   {
       ...

       myValueA = ValueInput<string>("myValueA", "Hello ");
       myValueB = ValueInput<string>("myValueB", String.Empty);

    ...
   }
}
```