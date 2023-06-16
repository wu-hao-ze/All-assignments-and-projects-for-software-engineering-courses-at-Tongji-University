# Variables API

> [!NOTE]
> For versions 2019/2020 LTS, download the Visual Scripting package from the [Unity Asset Store](https://assetstore.unity.com/packages/tools/visual-bolt-163802).

Visual scripting provides an easy API to handle variables, to get or set their value and verify if they are defined. All these operations are available from the Variables class.

For example:

```
Variables.Application.Set("score", 100);
```

***

## Usings

Add the following usings to your C# script to access the API:

```
using Unity.VisualScripting;
```

***

## Scopes

### Graph

To access variables on a graph, create a graph reference. This is basically a path to the nested graph from its root machine.

To get the root graph on a machine:

```
var graphReference = GraphReference.New(flowMachine, true);
```

To access nested graphs, pass their parent nodes as additional parameters:

```
var graphReference = GraphReference.New(flowMachine, new IGraphParentElement[] { subGraph }, true);
```

To pass a graph reference:

```
Variables.Graph(graphReference)
```

### Object

To access variables on an object:

```
Variables.Object(gameObject)
```

### Scene

To access scene variables, do one of the following:

```
Variables.Scene(scene)
```

Or:

```
Variables.Scene(gameObjectInScene)
```

Or:

```
Variables.ActiveScene
```

### Application

To access application variables:

```
Variables.Application
```

### Saved

To access saved variables:

```
Variables.Saved
```

***

## Operations

In these examples, the lowercase scope refers to one of the previous scopes.

### Get

To get the value of a variable, use the Get method with a name parameter:

```
scope.Get("name");
```

Note that variables are not strongly typed; they need to be cast manually. For example:

```
int health = (int)Variables.Object(player).Get("health")
```

### Set

To set the value of a variable, use the Set method with the name and value parameters:

```
scope.Set("name", value);
```

For example:

```
Variables.Object(player).Set("health", 100);
```

Because variables are not strongly typed, pass any value to the second parameter, even if the variable currently is of a different type.

> [!NOTE]
> Using the set method with a variable name that does not yet exist defines a new variable.

### Is Defined

To check if a variable is defined, use the IsDefined method with a name parameter:

```
scope.IsDefined("name");
```

For example:

```
if (Variables.Application.IsDefined("score"))
{
    // ...
}
```

