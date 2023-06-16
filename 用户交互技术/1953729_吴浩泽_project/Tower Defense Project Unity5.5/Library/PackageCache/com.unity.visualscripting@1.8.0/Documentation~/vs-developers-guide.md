# Developer's guide



## _Misc

| Node                         | Description                                                  |
| ---------------------------- | ------------------------------------------------------------ |
| Formula                      | Outputs the result of the formula entered, which has a user specified number of input ports. Can create vector 2,3,4 and access all variables by name (object, graph, scene, application, saved). |
| GetApplicationVariable       | Gets an application variable by name.                        |
| GetGraphVariable             | Gets a graph variable by name.                               |
| GetMember                    | Invokes a getter via reflection (field or property, static or instance). |
| GetObjectVariable            | Gets an object variable by name.                             |
| GetSavedVariable             | Gets a saved variable by name.                               |
| GetSceneVariable             | Gets a scene variable by name.                               |
| GetVariable                  | Gets a graph variable by name.                               |
| InvokeMember                 | Invokes a method via reflection (static or instance).        |
| IsApplicationVariableDefined | Returns true if the variable is defined.                     |
| IsGraphVariableDefined       | Returns true if the variable is defined.                     |
| IsObjectVariableDefined      | Returns true if the variable is defined.                     |
| IsSavedVariableDefined       | Returns true if the variable is defined.                     |
| IsSceneVariableDefined       | Returns true if the variable is defined.                     |
| IsVariableDefined            | Returns true if the variable is defined.                     |
| This (formerly Self)         | Provides a reference to the GameObject that has the Machine Component running the graph. |
| SetApplicationVariable       | Sets an application variable by name.                        |
| SetGraphVariable             | Sets a graph variable by name.                               |
| SetMember                    | Invokes a setter via reflection (field or property, static or instance). |
| SetObjectVariable            | Sets an object variable by name.                             |
| SetSavedVariable             | Sets a saved variable by name.                               |
| SetSceneVariable             | Sets a scene variable by name.                               |
| SetVariable                  | Sets a variable by name.                                     |



## Collections

| Node       | Description                                                |
| ---------- | ---------------------------------------------------------- |
| CountItems | Provides a count of the number of items in the collection. |
| FirstItem  | Gets a reference to the first item in the collections.     |
| LastItem   | Gets a reference to the last item in the collection.       |



### Collections, Dictionaries

| Node                  | Description                                                  |
| --------------------- | ------------------------------------------------------------ |
| AddDictionaryItem     | Adds Key/Value pair into dictionary.                         |
| ClearDictionary       | Removes all elements from the dictionary.                    |
| CreateDictionary      | Creates a local dictionary.                                  |
| DictionaryContainsKey | Returns true if the dictionary contains an entry with a matching key. |
| GetDictionaryItem     | Return value for a dictionary entry with the provided keys.  |
| MergeDictionaries     | Combines the contents of two dictionaries into a single dictionary. |
| RemoveDictionaryItem  | Removes an entry from the dictionary with the provided key.  |
| SetDictionaryItem     | Replaces the value of an existing entry with the provided key. |



### Collections, Lists

| Node             | Description                                                  |
| ---------------- | ------------------------------------------------------------ |
| AddListItem      | Adds an item to the list.                                    |
| ClearList        | Removes all elements from the list.                          |
| CreateList       | Creates a local list.                                        |
| GetListItem      | Gets the item in a list at the specified position provided by the index. |
| InsertListItem   | Inserts an item into a list at the specified position provided by the index. |
| ListContainsItem | Returns true if the item is contained in the list.           |
| MergeLists       | Combines the contents of two lists into a single list.       |
| RemoveListItem   | Removes the item from the list (if it is present).           |
| RemoveListItemAt | Removes the item from the list that is at the specified position provided by the index. |
| SetListItem      | Replaces the item in the list with a new item at the specified position provided by the index. |



## Control

| Node                 | Description                                                  |
| -------------------- | ------------------------------------------------------------ |
| If (Formerly Branch) | Executes the True branch if the provided input is true, otherwise executes the False branch. |
| Break                | Immediately exits the current loop.                          |
| Cache                | Flow node reads its input value whenever a value from any source is entered and outputs it when its output port is pulled. |
| For                  | A loop control. The Body branch is executed, providing an Index equal to First. If execution isn't interrupted (for example, by Break), the Index is incremented by Step. If the Index is less than Last, the Body branch is executed again. This process repeats until the Index is greater than Last, at which point the Exit branch is executed and the loop terminates. |
| ForEach              | A loop control that executes Body once for every item (provided as Item) contained in the provided collection. Once completed, the Exit branch is executed. |
| Once                 | Flow node with an internal state that triggers its output only the first time it is entered. Entering a second time does not trigger the output flow. After a reset, the next time you enter, will trigger the output flow. |
| SelectOnEnum         | Provides the object associated to the enum value provided as an input. |
| SelectOnFlow         | Provides the object associated to the incoming flow branch that triggered the node. |
| SelectOnInteger      | Data branching based on an input integer.                    |
| SelectOnString       | Data branching based on an input string.                     |
| SelectUnit           | Data branching based on an input enum.                       |
| Sequence             | Executes a series of branches in order. To determine the order, specify the number of steps in the sequence, labelled with their order (e.g. 0, 1, 2, 3). |
| SwitchOnEnum         | Executes the branch associated to the provided enum value.   |
| SwitchOnInteger      | Flow branching based on an input integer.                    |
| SwitchOnString       | Flow branching based on an input string.                     |
| Throw                | Causes an exception with the provided message.               |
| ToggleFlow           | Flow branching based on whether the node is On or Off, with inputs to set or toggle its On/Off state. |
| ToggleValue          | Flow branching based on whether a value got set to its associated On/Off value. |
| TryCatch             | Executes the Try branch. If an exception occurs in that branch, execute the Catch branch. Once the Try branch completes without exceptions or the Catch branch completes, the Finally branch is executed. |
| While                | Executes the Body branch. Repeat this step as long as the provide condition is true. When that condition is no longer true, execute the Exit branch. |



## Events

| Node               | Description                      |
| ------------------ | -------------------------------- |
| TriggerCustomEvent | Triggers a custom event by name. |



## Logic

| Node           | Description                                                  |
| -------------- | ------------------------------------------------------------ |
| And            | Logical AND (equivalent to &&) returns true if both operands are true and returns false otherwise. |
| Comparison     | All comparisons in one node: inputs A/B, outputs < <= > >= == !=, and so on. |
| Equal          | Logical AND (equivalent to &&) returns true if both operands are true and returns false otherwise. |
| ExclusiveOr    | Exclusive OR (equivalent to ^) compares the first operand to the second operand and returns true only when inputs differ (one is true, the other is false). |
| Greater        | Greater than (equivalent to >) returns false if the relationship in the expression is false; otherwise, returns true. |
| GreaterOrEqual | Greater than or equal to (equivalent to >=) returns false if the relationship in the expression is false; otherwise, returns true. |
| Less           | Less than (equivalent to <) returns false if the relationship in the expression is false; otherwise, returns true. |
| LessOrEqual    | Less than or equal to (equivalent to <=) returns false if the relationship in the expression is false; otherwise, returns true. |
| Negate         | Negation (equivalent to !) reverses the meaning of its operand. |
| NotEqual       | The not equal to operator (equivalent to. !=) returns true if the operands don't have the same value; otherwise, it returns false. |
| Or             | Logical OR (equivalent to \|\|) returns true if either or both operands is true and returns false otherwise. |



## Math, Generic

| Node     | Description                     |
| -------- | ------------------------------- |
| Add      | Calls the + operator on inputs. |
| Divide   | Calls the / operator on inputs. |
| Modulo   | Calls the % operator on inputs. |
| Multiply | Call the * operator on inputs.  |
| Subtract | Calls the - operator on inputs. |



## Math, Scalar

| Node         | Description                                                  |
| ------------ | ------------------------------------------------------------ |
| Absolute     | Absolute returns the absolute value of an integer.           |
| Add          | Sum of two floats.                                           |
| Average      | Average of all float inputs.                                 |
| Divide       | Divides the first float by the second float and returns the result. |
| Exponentiate | Raises the base to an exponent.                              |
| Lerp         | Interpolates within a range based on a parameter.            |
| Maximum      | Computes the maximum values passed in its argument.          |
| Minimum      | Computes the minimum values passed in its argument.          |
| Modulo       | Returns the remainder of a float divided by another float.   |
| MoveTowards  | Returns the result of moving Current towards Target by up to Max Delta. |
| Multiply     | Multiplies two floats.                                       |
| Normalize    | Equivalent to MathF.Sign.                                    |
| PerSecond    | Multiplies the value by the duration of the frame (for example, to move smoothly from frame to frame with a fixed speed). |
| Root         | Calculates x to the root of n.                               |
| Round        | Rounds a float - ceil, floor or closest integer.             |
| Subtract     | Subtracts the first float from the second float and returns the result. |
| Sum          | Sum of multiple floats.                                      |



## Math, Vector2

| Node        | Description                                                  |
| ----------- | ------------------------------------------------------------ |
| Absolute    | Gives both the x and y values of a Vector2 a positive sign.  |
| Add         | Sum of two vectors.                                          |
| Angle       | Angle between two directional vectors.                       |
| Average     | Average of a series of Vector2 value.                        |
| Distance    | Distance between two Vector2 points.                         |
| Divide      | Divide the components of one Vector2 by the corresponding components of a second Vector2. |
| DotProduct  | Dot Product between two Vector2 values.                      |
| Lerp        | Interpolates within a range based on a parameter.            |
| Maximum     | Computes the maximum values passed in its argument.          |
| Minimum     | Computes the minimum values passed in its argument.          |
| Modulo      | Returns a Vector2 where the components of the first vector are moduled by the corresponding components of the second vector. |
| MoveTowards | Returns the result of moving Current towards Target by up to Max Delta. |
| Multiply    | Multiplies the corresponding components of two Vector2 values. |
| Normalize   | Returns a Vector2 with a magnitude of 1 that retains the same direction. |
| PerSecond   | Multiplies the value by the duration of the frame (for example, to move smoothly from frame to frame with a fixed speed). |
| Project     | Projects one vector onto another vector.                     |
| Round       | Rounds the components of a Vector2 to closest integer value. |
| Subtract    | Subtracts one Vector2 value from another Vector2 value.      |
| Sum         | Adds two Vector2 values.                                     |



## Math, Vector3

| Node        | Description                                                  |
| ----------- | ------------------------------------------------------------ |
| Absolute    | Gives both the x and y values of a Vector3 a positive sign.  |
| Add         | Sum of two vectors.                                          |
| Angle       | Angle between two directional vectors.                       |
| Average     | Average of a series of Vector3 value.                        |
| Distance    | Distance between two Vector3 points.                         |
| Divide      | Divide the components of one Vector3 by the corresponding components of a second Vector3. |
| DotProduct  | Dot Product between two Vector3 values.                      |
| Lerp        | Interpolates within a range based on a parameter.            |
| Maximum     | Computes the maximum values passed in its argument.          |
| Minimum     | Computes the minimum values passed in its argument.          |
| Modulo      | Returns a Vector3 where the components of the first vector are moduled by the corresponding components of the second vector. |
| MoveTowards | Returns the result of moving Current towards Target by up to Max Delta. |
| Multiply    | Multiplies the corresponding components of two Vector3 values. |
| Normalize   | Returns a Vector3 with a magnitude of 1 that retains the same direction. |
| PerSecond   | Multiplies the value by the duration of the frame (for example, to move smoothly from frame to frame with a fixed speed). |
| Project     | Projects one vector onto another vector.                     |
| Round       | Rounds the components of a Vector3 to closest integer value. |
| Subtract    | Subtracts one Vector3 value from another Vector3 value.      |
| Sum         | Adds two Vector3 values.                                     |



## Math, Vector4

| Node        | Description                                                  |
| ----------- | ------------------------------------------------------------ |
| Absolute    | Gives both the x and y values of a Vector4 a positive sign.  |
| Add         | Sum of two vectors.                                          |
| Angle       | Angle between two directional vectors.                       |
| Average     | Average of a series of Vector4 value.                        |
| Distance    | Distance between two Vector4 points.                         |
| Divide      | Divide the components of one Vector4 by the corresponding components of a second Vector4. |
| DotProduct  | Dot Product between two Vector4 values.                      |
| Lerp        | Interpolates within a range based on a parameter.            |
| Maximum     | Computes the maximum values passed in its argument.          |
| Minimum     | Computes the minimum values passed in its argument.          |
| Modulo      | Returns a Vector4 where the components of the first vector are moduled by the corresponding components of the second vector. |
| MoveTowards | Returns the result of moving Current towards Target by up to Max Delta. |
| Multiply    | Multiplies the corresponding components of two Vector4 values. |
| Normalize   | Returns a Vector4 with a magnitude of 1 that retains the same direction. |
| PerSecond   | Multiplies the value by the duration of the frame (for example, to move smoothly from frame to frame with a fixed speed). |
| Project     | Projects one vector onto another vector.                     |
| Round       | Rounds the components of a Vector4 to closest integer value. |
| Subtract    | Subtracts one Vector4 value from another Vector4 value.      |
| Sum         | Adds two Vector4 values.                                     |



## Nesting

| Node                   | Description                                                  |
| ---------------------- | ------------------------------------------------------------ |
| GraphInput             | Gets the value of a graph input when the graph is used as a Subgraph. |
| GraphOutput            | Gets the value of a graph output when the graph is used as a Subgraph. |
| StateUnit              | References another state machine graph as a state in the current graph. |
| Subgraph             | References another flow graph as a Subgraph in the current graph. |
| TriggerStateTransition | In a transition graph, triggers the transition to the target graph in the parent state machine graph. |



## Nulls

| Node         | Description                                                  |
| ------------ | ------------------------------------------------------------ |
| Null         | Null literal.                                                |
| NullCheck    | Branching based on the input value being null.               |
| NullCoalesce | Returns the input value or a default value if the input value is null. |



## Time

| Node                  | Description                                                  |
| --------------------- | ------------------------------------------------------------ |
| Cooldown              | Coroutine node that can re-trigger its output only after a certain cooldown time interval. |
| Timer                 | Coroutine node that triggers its output after a time interval. |
| WaitForEndOfFrameUnit | Coroutine node that yields return new WaitForEndOfFrame().   |
| WaitForFlow           | Coroutine node that waits until the input flow port is triggered. |
| WaitForNextFrameUnit  | Coroutine node that returns null.                            |
| WaitForSecondsUnit    | Coroutine node that returns new WaitForSeconds().            |
| WaitUntilUnit         | Coroutine node that returns new WaitUntill(() => value).     |
| WaitWhileUnit         | Coroutine node that returns new WaitUntill(() => !value).    |



## Variables

| Node          | Description                                                  |
| ------------- | ------------------------------------------------------------ |
| SaveVariables | Forces saved variables to be saved to the PlayerPrefs (this is useful on platforms that do not support automatic save on quit). |



## Events

| Node        | Description                                               |
| ----------- | --------------------------------------------------------- |
| UnityEvent  | Called when a UnityEvent is pointed to TriggerUnityEvent. |
| CustomEvent | Bolt's custom events, defined by name.                    |



### Events, Animation 

| Node                  | Description                                                  |
| --------------------- | ------------------------------------------------------------ |
| Animation Event       | Called when an animation event points to TriggerAnimationEvent. This version allows you to use the string parameter as the event name |
| Named Animation Event | Called when an animation event points to TriggerAnimationEvent. This version allows you to use the string parameter as the event name. |
| OnAnimatorIK          | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnAnimatorIK.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnAnimatorIK.html) |
| OnAnimatorMove        | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnAnimatorMove.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnAnimatorMove.html) |



### Events, Application

| Node                   | Description                                                  |
| ---------------------- | ------------------------------------------------------------ |
| OnApplicationFocus     | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnApplicationFocus.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnApplicationFocus.html) |
| OnApplicationLostFocus | Implements <br />[https://docs.unity3d.com/ScriptReference/<br />MonoBehaviour.OnApplicationFocus.html](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationFocus.html) |
| OnApplicationPause     | [https://docs.unity3d.com/ScriptReference/<br />MonoBehaviour.OnApplicationPause.html](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationPause.html) |
| OnpplicationQuit       | [https://docs.unity3d.com/ScriptReference/<br />MonoBehaviour.OnApplicationQuit.html](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html) |
| OnApplicationResume    | Implements <br />[https://docs.unity3d.com/ScriptReference/<br />MonoBehaviour.OnApplicationPause.html](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationPause.html) |



### Events, Editor

| Node                 | Description                                                  |
| -------------------- | ------------------------------------------------------------ |
| OnDrawGizmos         | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnDrawGizmos.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnDrawGizmos.html) |
| OnDrawGizmosSelected | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnDrawGizmosSelected.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnDrawGizmosSelected.html) |



### Events, GUI

| Node                     | Description                                                  |
| ------------------------ | ------------------------------------------------------------ |
| OnBeginDrag              | Implements IBeginDragHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IBeginDragHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IBeginDragHandler.html) |
| OnButtonClick            | Registers to a Button's onClick event<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />UI.Button.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.Button.html) |
| OnCancel                 | Implements ICancelHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.ICancelHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.ICancelHandler.html) |
| OnDeselect               | Implements IDeselectHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IDeselectHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IDeselectHandler.html) |
| OnDrag                   | Implements IDragHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IDragHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IDragHandler.html) |
| OnDrop                   | Implements IDropHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IDragHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IDragHandler.html) |
| OnDropdownValueChanged   | Registers to a Dropdown's onValueChanged event<br /> https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />UI.Dropdown.html |
| OnEndDrag                | Implements IEndDragHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IEndDragHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IEndDragHandler.html) |
| OnGUI                    | Triggers on MonoBehaviour.OnGUI<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />MonoBehaviour.OnGUI.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/MonoBehaviour.OnGUI.html) |
| OnInputFieldEndEdit      | Registers to an InputField's onEndEdit event<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />UI.InputField.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.InputField.html) |
| OnInputFieldValueChanged | Registers to an InputField's onValueChanged event<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />UI.InputField.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.InputField.html) |
| OnMove                   | Implements IMoveHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IMoveHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IMoveHandler.html) |
| OnPointerClick           | Implements IPointerClickHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IPointerClickHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IPointerClickHandler.html) |
| OnPointerDown            | Implements IPointerDownHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IPointerDownHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IPointerDownHandler.html) |
| OnPointerEnter           | Implements IPointerEnterHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IPointerEnterHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IPointerEnterHandler.html) |
| OnPointerExit            | Implements IPointerExitHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IPointerExitHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IPointerExitHandler.html) |
| OnPointerUp              | Implements IPointerUpHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IPointerUpHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IPointerUpHandler.html) |
| OnScroll                 | Implements IPointerScrollHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.IPointerScrollHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IPointerScrollHandler.html) |
| OnScrollbarValueChanged  | Registers to a Scrollbar's onValueChanged event<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />UI.Scrollbar.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.Scrollbar.html) |
| OnScrollRectValueChanged | Registers to a ScrollRect's onValueChanged event<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />UI.ScrollRect.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.ScrollRect.html) |
| OnSelect                 | Implements ISelectHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.ISelectHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.ISelectHandler.html) |
| OnSliderValueChanged     | Registers to a Sliders's onValueChanged event<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />UI.Slider.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.Slider.html) |
| OnSubmit                 | Implements ISubmitHandler<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />EventSystems.ISubmitHandler.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.ISubmitHandler.html) |
| OnToggleValueChanged     | Registers to a Toggle's onValueChanged event<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />UI.Toggle.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.Toggle.html) |

### Events, Hierarchy

| Node                       | Description                                                  |
| -------------------------- | ------------------------------------------------------------ |
| OnTransformChildrenChanged | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnTransformChildrenChanged.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnTransformChildrenChanged.html) |
| OnTransformParentChanged   | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnTransformParentChanged.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnTransformParentChanged.html) |



### Events, Input

| Node              | Description                                                  |
| ----------------- | ------------------------------------------------------------ |
| OnButtonInput     | On Update, checks Input.GetButtonDown/GetButtonUp/GetButton based on the specified Action type <br />[https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Input.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Input.html). |
| OnKeyboardInput   | On Update, check Input.GetKeyDown/GetKeyUp/GetKey based on the specified action type<br /> [https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Input.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Input.html) |
| OnMouseDown       | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnMouseDown.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnMouseDown.html) |
| OnMouseDrag       | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnMouseDrag.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnMouseDrag.html) |
| OnMouseEnter      | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnMouseEnter.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnMouseEnter.html) |
| OnMouseExit       | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnMouseExit.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnMouseExit.html) |
| OnMouseInput      | [https://docs.unity3d.com/ScriptReference/<br />Input.GetMouseButton.html](https://docs.unity3d.com/ScriptReference/Input.GetMouseButton.html) |
| OnMouseOver       | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnMouseOver.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnMouseOver.html) |
| OnMouseUp         | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnMouseUp.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnMouseUp.html) |
| OnMouseUpAsButton | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnMouseUpAsButton.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnMouseUpAsButton.html) |



### Events, Lifecycle

| Node        | Description                                                  |
| ----------- | ------------------------------------------------------------ |
| FixedUpdate | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.FixedUpdate.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.FixedUpdate.html) |
| LateUpdate  | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.LateUpdate.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.LateUpdate.html) |
| OnDestroy   | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnDestroy.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnDestroy.html) |
| OnDisable   | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnDisable.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnDisable.html) |
| OnEnable    | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnEnable.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnEnable.html) |
| Start       | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.Start.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.Start.html) |
| Update      | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.Update.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.Start.html) |



### Events, Navigation

| Node                 | Description                                                  |
| -------------------- | ------------------------------------------------------------ |
| OnDestinationReached | Trigger if a NavMeshAgent's remaining distance to its target is less than the provided threshold and either has a current NavMeshPathStatus of PathComplete or requireSuccess is false. <br />[https://docs.unity3d.com/2019.1/Documentation/ScriptReference/<br />AI.NavMeshAgent.html](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/AI.NavMeshAgent.html). |



### Events, Physics

| Node                    | Description                                                  |
| ----------------------- | ------------------------------------------------------------ |
| OnCollisionEnter        | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnCollisionEnter.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnCollisionEnter.html) |
| OnCollisionExit         | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnCollisionExit.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnCollisionExit.html) |
| OnCollisionStay         | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnCollisionStay.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnCollisionStay.html) |
| OnControllerColliderHit | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnControllerColliderHit.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnControllerColliderHit.html) |
| OnJointBreak            | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnJointBreak.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnJointBreak.html) |
| OnParticleCollision     | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnParticleCollision.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnParticleCollision.html) |
| OnTriggerEnter          | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnTriggerEnter.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnTriggerEnter.html) |
| OnTriggerExit           | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnTriggerExit.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnTriggerExit.html) |
| OnTriggerStay           | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnTriggerStay.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnTriggerStay.html) |



### Events, Physics 2D

| Node                      | Description                                                  |
| ------------------------- | ------------------------------------------------------------ |
| OnCollisionEnter2D        | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnCollisionEnter2D.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnCollisionEnter2D.html) |
| OnCollisionExit2D         | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnCollisionExit2D.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnCollisionExit2D.html) |
| OnCollisionStay2D         | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnCollisionStay2D.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnCollisionStay2D.html) |
| OnControllerColliderHit2D | [https://docs.unity3d.com/ScriptReference/<br />MonoBehaviour.OnControllerColliderHit.html](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnControllerColliderHit.html) |
| OnJointBreak2D            | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnJointBreak2D.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnJointBreak2D.html) |
| OnParticleCollision2D     | [https://docs.unity3d.com/ScriptReference/<br />MonoBehaviour.OnParticleCollision.html](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html) |
| OnTriggerEnter2D          | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnTriggerEnter2D.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnTriggerEnter2D.html) |
| OnTriggerExit2D           | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnTriggerExit2D.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnTriggerExit2D.html) |
| OnTriggerStay2D           | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnTriggerStay2D.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnTriggerStay2D.html) |



### Events, Rendering

| Node              | Description                                                  |
| ----------------- | ------------------------------------------------------------ |
| OnBecameInvisible | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnBecameInvisible.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnBecameInvisible.html) |
| OnBecameVisible   | [https://docs.unity3d.com/2021.1/Documentation/ScriptReference/<br />MonoBehaviour.OnBecameVisible.html](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/MonoBehaviour.OnBecameVisible.html) |



### Events, State

| Node         | Description              |
| ------------ | ------------------------ |
| OnEnterState | When a state is entered. |
| OnExitState  | When a state is exited.  |



