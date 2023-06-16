# Input Event nodes

Input nodes are an Event node type that can read input from Unity's [Input Manager](https://docs.unity3d.com/Manual/class-InputManager.html) or [Input System package](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest) for use in a Script Graph. 

For more information about how to read and capture input in Visual Scripting, see [Capture user input in an application](vs-capture-player-input.md).

## Input System package nodes 

The following nodes read and interact with Events from the Input System package: 

| **Node**  | **Description**  |
| :------   | :--------------- |
| [**On Input System Event Button**](vs-nodes-events-input-system-button.md) | The On Input System Event Button node listens for a specific Input Action from a Player Input component. It doesn't send or read any other data. |
| [**On Input System Event Float**](vs-nodes-events-input-system-float.md) | The On Input System Event Float node listens for a specific Input Action from a Player Input component. The node can output a **single float value**. |
| [**On Input System Event Vector 2**](vs-nodes-events-input-system-vector2.md) | The On Input System Event Vector 2 node listens for a specific Input Action from a Player Input component. The node can output **two values as a Vector 2**. |

## Input Manager nodes 

The following nodes read and interact with Events from Unity's Input Manager: 

| **Node**  | **Description**  |
| :------   | :--------------- |
| [**On Button Input**](vs-nodes-events-on-button-input.md) | The On Button Input node listens for a specified action on a virtual button from your Input Manager configuration. |
| [**On Keyboard Input**](vs-nodes-events-on-keyboard-input.md) | The On Keyboard Input node listens for a specified action on a keyboard key. |
| [**On Mouse Down**](vs-nodes-events-on-mouse-down.md) | The On Mouse Down node listens for a mouse click action on a specific GameObject in your application. |
| [**On Mouse Drag**](vs-nodes-events-on-mouse-drag.md) | The On Mouse Drag node listens for a mouse click and hold on a specific GameObject in your application. It triggers the next node connected to it as long as the mouse button is held down on that GameObject. |
| [**On Mouse Enter**](vs-nodes-events-on-mouse-enter.md) | The On Mouse Enter node listens for the user's mouse pointer location to enter the Collider of a specified GameObject. When the mouse enters the Collider or GUI element, the node triggers the next node connected to it. |
| [**On Mouse Exit**](vs-nodes-events-on-mouse-exit.md) | The On Mouse Exit node listens for the user's mouse pointer location to exit the Collider of a specified GameObject. When the mouse exits the Collider or GUI element, the node triggers the next node connected to it. |
| [**On Mouse Input**](vs-nodes-events-on-mouse-input.md)| The On Mouse Input node listens for a specific action on a user's mouse. The action doesn't need to happen on a specific GameObject's Collider. |
| [**On Mouse Over**](vs-nodes-events-on-mouse-over.md) | The On Mouse Over node listens for a user's mouse to land over a specified GameObject's Collider. While the user's mouse is over the Collider, it triggers the next node connected to it once every frame. |
| [**On Mouse Up As Button**](vs-nodes-events-on-mouse-up-button.md) | The On Mouse Up As Button node listens for a user to release their mouse button after they click a Collider in your application. To trigger the On Mouse Up As Button node, the user must release their mouse button over the same Collider they clicked. |
| [**On Mouse Up**](vs-nodes-events-on-mouse-up.md) | The On Mouse Up node listens for a user to release their mouse button after they click a Collider in your application. The user can release their mouse button anywhere in your application to trigger the On Mouse Up node. |

## Additional resources

- [Capturing input in your application](vs-capture-player-input.md)
- [Capture input using the Input System package](vs-capturing-player-inputs-new.md)
- [Capture input using the Input Manager](vs-capturing-player-inputs-old.md)

