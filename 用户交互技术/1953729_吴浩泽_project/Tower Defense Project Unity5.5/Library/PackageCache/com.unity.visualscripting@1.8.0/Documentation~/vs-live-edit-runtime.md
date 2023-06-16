# Live edit during runtime

Live editing in visual scripting goes beyond adjusting values in real-time. Live editing also includes the ability to add and remove nodes as well as connectors; you can code while the game is playing and immediately affect the gameplay.

Remember that:

- Changes you make to embed graphs are **reverted** when you exit play mode; they live inside components.
- Changes you make to graphs are **saved** when you exit play mode; they live inside assets.
- Graph variables are saved when not in an embed graph.
- The following variables are not saved:
  - Object
  - Scene
  - App
  - Saved

> [!NOTE]
> If you’ve used an embed graph and do not want to lose your modifications, copy all the changes you made to the embed graph before exiting Play mode. Paste them back in when in edit mode. You can’t do this for any changed variables.

As a visual aid, connectors in Live mode display their execution flow with animated directional droplets going in the direction of execution. The speed and number of droplets does not represent the frequency or speed of execution.

## To adjust the graph during runtime

With a graph open do any or all of the following:

- In the Inspector, click in any field to change a component’s value. The values are not persistent and won’t save when you leave Play mode.
- Right-click in an empty spot in the graph and add a node.
- Connect nodes.
- Delete any connectors.
- Add an extension.
- Add and link Debug nodes.
- Change values directly in a node.

> [!TIP]
> Select any GameObject that contains a script graph to see and work with the values of the selected GameObject during runtime.

> [!NOTE]
> When you modify anything after a Start event during runtime you won’t get the update on the GameObject during that session. You need to restart the session for Unity to execute the new logic after the start event.

The changes you make to a graph are instantly shared across all instances of that saved graph asset.


