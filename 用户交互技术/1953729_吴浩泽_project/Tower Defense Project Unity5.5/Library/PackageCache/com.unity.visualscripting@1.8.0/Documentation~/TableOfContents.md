* [About Visual Scripting](index.md)

    *[Configure project settings](vs-configuration.md)

        *[Add or remove available nodes](vs-add-remove-node-library.md)

        *[Add or remove types](vs-add-remove-type-options.md)

        *[Create or restore a backup](vs-create-restore-backups.md)

    *[Choose a control scheme](vs-control-schemes.md)

    *[Configure your preferences](vs-set-preferences.md)
    
    *[Update Visual Scripting](vs-update.md)
    
    *[Version control systems](vs-version-control.md)
    
    *[Use Visual Scripting with Unity Cloud Build](vs-aot.md)

* [Basic concepts in Visual Scripting](vs-basic-concepts.md)

    *[The interface](vs-interface-overview.md)

    *[Nodes](vs-nodes.md)

    *[Graphs](vs-graph-types.md)

        *[Subgraphs and State Units](vs-nesting-subgraphs-state-units.md)

        *[Transitions](vs-transitions.md)

    *[Script Machines and State Machines](vs-graph-machine-types.md)

    *[Object types](vs-types.md)

        *[Custom types](vs-custom-types.md)
    
    *[Variables](vs-variables.md)

* [Develop application logic with Script Graphs](vs-script-graphs-intro.md)

    *[Create a new graph file](vs-create-graph.md)

        *[Create a new blank graph with the Project window](vs-create-graph-project-window.md)

        *[Create a new unassigned graph with the empty graph creation flow](vs-create-graph-unassigned-flow.md)

        *[Create and assign a graph to an existing GameObject](vs-create-graph-assign-existing-gameobject.md)

        *[Create and assign a graph to a new GameObject](vs-create-graph-assign-new-gameobject.md)

        *[Create a graph on a Script Machine or State Machine](vs-create-graph-on-machine.md)

    *[Attach a graph file to a Script Machine or State Machine](vs-attach-graph-machine.md)

    *[Open a graph file](vs-open-graph-edit.md)

        *[Add a node to a Script Graph](vs-add-node-to-graph.md)

        *[Connect nodes in a Script Graph](vs-creating-connections.md)

        *[Create and add a variable to a Script Graph](vs-add-variable-graph.md)

        *[Create node groups](vs-groups.md)
        
        *[Add comments to a graph](vs-sticky-notes.md)

    *[Add a Subgraph to a Script Graph](vs-nesting-add-subgraph.md)

       *[Add a Trigger or Data port to a Script Graph](vs-nesting-add-triggers-data-graph.md)
    
    *[Add a State Unit to a Script Graph](vs-nesting-add-state-unit.md)

    *[Custom Events](vs-custom-events.md)

        *[Add a Custom Event node](vs-add-custom-event-node.md)

        *[Add a Trigger Custom Event node](vs-add-custom-event-node-trigger.md)

    *[Capture user input in an application](vs-capture-player-input.md)

        *[Capture input using the Input Manager](vs-capturing-player-inputs-old.md)

        *[Add and configure a Player Input component](vs-capture-player-input-add-component.md)

        *[Capture input using the Input System package](vs-capturing-player-inputs-new.md)
    
    *[Use relations to debug](vs-relations.md)
    
        *[Predictive and live debugging](vs-debugging.md)
        
        *[Working with debug messages](vs-debug-messages.md)
        
    *[Live edit](vs-live.md)
    
         *[Live edit during runtime](vs-live-edit-runtime.md)
    
* [Develop logic transitions with state graphs](vs-state-graphs-intro.md)

    *[Create a new state](vs-create-state.md)
    
    *[Create a transition between states](vs-creating-transition.md)

* [Advanced customization and development](vs-advanced-topics-intro.md)

    *[Refactor a C# script with Visual Scripting](vs-refactoring.md)

        *[Add the RenamedFrom attribute to a C# script](vs-refactor-add-attribute.md)

    *[Custom C# nodes](vs-create-custom-node.md)

        *[Create a new simple Custom C# node](vs-create-custom-node-empty.md)

        *[Add ports to your Custom C# node](vs-create-custom-node-add-ports.md)
        
        *[Add logic to your Custom C# node](vs-create-custom-node-add-logic.md)

        *[Add relations to your Custom C# node](vs-create-custom-node-add-relations.md)

        *[Add documentation to your Custom C# node](vs-create-custom-node-add-docs.md)

        *[Custom C# node attributes reference](vs-create-custom-node-attributes-reference.md)

    *[Create a Custom Scripting Event node](vs-create-own-custom-event-node.md)

        *[Create a Custom Scripting Event Sender node](vs-create-own-custom-event-send-node.md)

        *[Trigger a Custom Scripting Event from a C# script](vs-create-own-custom-event-node-trigger-code.md)

        *[Listen to a Custom Scripting Event from a C# script](vs-create-own-custom-event-listen-code.md)

    *[Use a custom type](vs-using-custom-types.md)

        *[Add the Inspectable attribute to a custom type](vs-add-inspectable-attribute-custom-types.md)
            
        *[Create a custom PropertyDrawer for a custom type](vs-create-custom-drawer.md)
        
* [Node reference](vs-nodes-reference.md)

    *[This node](vs-self.md)
    
    *[Control node](vs-control.md)
    
    *[Time node](vs-time.md)

    *Events

        *[Event nodes](vs-events-reference.md)

        *[Input Event nodes](vs-input-nodes.md)

            *[On Input System Event Button](vs-nodes-events-input-system-button.md)

            *[On Input System Event Float](vs-nodes-events-input-system-float.md)

            *[On Input System Event Vector 2](vs-nodes-events-input-system-vector2.md)
            
            *[On Button Input](vs-nodes-events-on-button-input.md)

            *[On Keyboard Input](vs-nodes-events-on-keyboard-input.md)

            *[On Mouse Down](vs-nodes-events-on-mouse-down.md)

            *[On Mouse Drag](vs-nodes-events-on-mouse-drag.md)

            *[On Mouse Enter](vs-nodes-events-on-mouse-enter.md)

            *[On Mouse Exit](vs-nodes-events-on-mouse-exit.md)

            *[On Mouse Input](vs-nodes-events-on-mouse-input.md)

            *[On Mouse Over](vs-nodes-events-on-mouse-over.md)

            *[On Mouse Up As Button](vs-nodes-events-on-mouse-up-button.md)

            *[On Mouse Up](vs-nodes-events-on-mouse-up.md)
    
    *[Variable node](vs-variables-reference.md)
    
    *[Nulls node](vs-nulls.md)
    
    *[Formula node](vs-formula.md)

    *[Nesting](vs-nesting-nodes.md)

        *[Input node](vs-nesting-input-node.md)

        *[Output node](vs-nesting-output-node.md)

        *[State Unit node](vs-nesting-state-unit-node.md)

        *[Subgraph node](vs-nesting-subgraph-node.md)

    *[Script graph nodes](vs-scripts-reference.md)

    *[State graph nodes](vs-states-reference.md)    

* [Developer's guide](vs-developers-guide.md)

