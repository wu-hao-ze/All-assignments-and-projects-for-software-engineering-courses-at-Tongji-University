using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [Canvas(typeof(FlowGraph))]
    public sealed class FlowCanvas : VisualScriptingCanvas<FlowGraph>
    {
        public FlowCanvas(FlowGraph graph) : base(graph) { }


        #region Clipboard

        public override void ShrinkCopyGroup(HashSet<IGraphElement> copyGroup)
        {
            copyGroup.RemoveWhere(element =>
            {
                if (element is IUnitConnection)
                {
                    var connection = (IUnitConnection)element;

                    if (!copyGroup.Contains(connection.source.unit) ||
                        !copyGroup.Contains(connection.destination.unit))
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        #endregion


        #region Window

        public override void OnToolbarGUI()
        {
            showRelations = GUILayout.Toggle(showRelations, "Relations", LudiqStyles.toolbarButton);

            EditorGUI.BeginChangeCheck();

            BoltFlow.Configuration.showConnectionValues = GUILayout.Toggle(BoltFlow.Configuration.showConnectionValues, "Values", LudiqStyles.toolbarButton);

            BoltCore.Configuration.dimInactiveNodes = GUILayout.Toggle(BoltCore.Configuration.dimInactiveNodes, "Dim", LudiqStyles.toolbarButton);

            if (EditorGUI.EndChangeCheck())
            {
                BoltFlow.Configuration.Save();

                BoltCore.Configuration.Save();
            }

            base.OnToolbarGUI();
        }

        #endregion


        #region View

        protected override bool shouldEdgePan => base.shouldEdgePan || isCreatingConnection;

        public const float inspectorZoomThreshold = 0.7f;

        #endregion


        #region Lifecycle

        public override void Close()
        {
            base.Close();

            CancelConnection();
        }

        protected override void HandleHighPriorityInput()
        {
            if (isCreatingConnection)
            {
                if (e.IsMouseDown(MouseButton.Left))
                {
                    connectionEnd = mousePosition;
                    NewUnitContextual();
                    e.Use();
                }
                else if (e.IsFree(EventType.KeyDown) && e.keyCode == KeyCode.Escape)
                {
                    CancelConnection();
                    e.Use();
                }
            }

            base.HandleHighPriorityInput();
        }

        private void CompleteContextualConnection(IUnitPort source, IUnitPort destination)
        {
            source.ValidlyConnectTo(destination);
            Cache();
            var unitPosition = this.Widget<IUnitWidget>(destination.unit).position.position;
            var portPosition = this.Widget<IUnitPortWidget>(destination).handlePosition.center.PixelPerfect();
            var offset = portPosition - unitPosition;
            destination.unit.position -= offset;
            this.Widget(destination.unit).Reposition();
            connectionSource = null;
            GUI.changed = true;
        }

        public void NewUnitContextual()
        {
            var filter = UnitOptionFilter.Any;
            filter.GraphHashCode = graph.GetHashCode();

            if (connectionSource is ValueInput)
            {
                var valueInput = (ValueInput)connectionSource;
                filter.CompatibleOutputType = valueInput.type;
                filter.Expose = false;
                filter.NoConnection = false;
                NewUnit(mousePosition, GetNewUnitOptions(filter), (unit) => CompleteContextualConnection(valueInput, unit.CompatibleValueOutput(valueInput.type)));
            }
            else if (connectionSource is ValueOutput)
            {
                var valueOutput = (ValueOutput)connectionSource;
                filter.CompatibleInputType = valueOutput.type;
                filter.NoConnection = false;
                NewUnit(mousePosition, GetNewUnitOptions(filter), (unit) => CompleteContextualConnection(valueOutput, unit.CompatibleValueInput(valueOutput.type)));
            }
            else if (connectionSource is ControlInput)
            {
                var controlInput = (ControlInput)connectionSource;
                filter.NoControlOutput = false;
                filter.NoConnection = false;
                NewUnit(mousePosition, GetNewUnitOptions(filter), (unit) => CompleteContextualConnection(controlInput, unit.controlOutputs.First()));
            }
            else if (connectionSource is ControlOutput)
            {
                var controlOutput = (ControlOutput)connectionSource;
                filter.NoControlInput = false;
                filter.NoConnection = false;
                NewUnit(mousePosition, GetNewUnitOptions(filter), (unit) => CompleteContextualConnection(controlOutput, unit.controlInputs.First()));
            }
        }

        #endregion


        #region Context

        protected override void OnContext()
        {
            if (isCreatingConnection)
            {
                CancelConnection();
            }
            else
            {
                // Checking for Alt seems to lose focus, for some reason maybe
                // unrelated to Bolt. Shift or other modifiers seem to work though.
                if (base.GetContextOptions().Any() && (!BoltFlow.Configuration.skipContextMenu || e.shift))
                {
                    base.OnContext();
                }
                else
                {
                    NewUnit(mousePosition);
                }
            }
        }

        protected override IEnumerable<DropdownOption> GetContextOptions()
        {
            yield return new DropdownOption((Action<Vector2>)(NewUnit), "Add Node...");
            yield return new DropdownOption((Action<Vector2>)(NewSticky), "Create Sticky Note");
            foreach (var baseOption in base.GetContextOptions())
            {
                yield return baseOption;
            }
        }

        public void AddUnit(IUnit unit, Vector2 position)
        {
            UndoUtility.RecordEditedObject("Create Node");
            unit.guid = Guid.NewGuid();
            unit.position = position.PixelPerfect();
            graph.units.Add(unit);
            selection.Select(unit);
            GUI.changed = true;
        }

        private UnitOptionTree GetNewUnitOptions(UnitOptionFilter filter)
        {
            var options = new UnitOptionTree(new GUIContent("Node"));

            options.filter = filter;
            options.reference = reference;

            if (filter.CompatibleOutputType == typeof(object))
            {
                options.surfaceCommonTypeLiterals = true;
            }

            return options;
        }

        private void NewSticky(Vector2 position)
        {
            UndoUtility.RecordEditedObject("Create Sticky Note");
            var stickyNote = new StickyNote() { position = new Rect(position, new Vector2(100, 100)) };
            graph.elements.Add(stickyNote);
            selection.Select(stickyNote);
            GUI.changed = true;
        }

        private void NewUnit(Vector2 position)
        {
            var filter = UnitOptionFilter.Any;
            filter.GraphHashCode = graph.GetHashCode();
            NewUnit(position, GetNewUnitOptions(filter));
        }

        private void NewUnit(Vector2 unitPosition, UnitOptionTree options, Action<IUnit> then = null)
        {
            delayCall += () =>
            {
                var activatorPosition = new Rect(e.mousePosition, new Vector2(200, 1));

                var context = this.context;

                LudiqGUI.FuzzyDropdown
                    (
                        activatorPosition,
                        options,
                        null,
                        delegate (object _option)
                        {
                            context.BeginEdit();
                            if (_option is IUnitOption)
                            {
                                var option = (IUnitOption)_option;
                                var unit = option.InstantiateUnit();
                                AddUnit(unit, unitPosition);
                                option.PreconfigureUnit(unit);
                                then?.Invoke(unit);
                                GUI.changed = true;
                            }
                            else
                            {
                                if ((Type)_option == typeof(StickyNote))
                                {
                                    NewSticky(unitPosition);
                                }
                            }

                            context.EndEdit();
                        }
                    );
            };
        }

        #endregion


        #region Drag & Drop

        private bool CanDetermineDraggedInput(UnityObject uo)
        {
            if (uo.IsSceneBound())
            {
                if (reference.self == uo.GameObject())
                {
                    // Because we'll be able to assign it to Self
                    return true;
                }

                if (reference.serializedObject.IsSceneBound())
                {
                    // Because we'll be able to use a direct scene reference
                    return true;
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool AcceptsDragAndDrop()
        {
            if (DragAndDropUtility.Is<ScriptGraphAsset>())
            {
                return FlowDragAndDropUtility.AcceptsScript(graph);
            }

            return DragAndDropUtility.Is<UnityObject>() && !DragAndDropUtility.Is<IMacro>() && CanDetermineDraggedInput(DragAndDropUtility.Get<UnityObject>())
                || EditorVariablesUtility.isDraggingVariable;
        }

        public override void PerformDragAndDrop()
        {
            if (DragAndDropUtility.Is<ScriptGraphAsset>())
            {
                var flowMacro = DragAndDropUtility.Get<ScriptGraphAsset>();
                var superUnit = new SubgraphUnit(flowMacro);
                AddUnit(superUnit, DragAndDropUtility.position);
            }
            else if (DragAndDropUtility.Is<UnityObject>())
            {
                var uo = DragAndDropUtility.Get<UnityObject>();
                var type = uo.GetType();
                var filter = UnitOptionFilter.Any;
                filter.Literals = false;
                filter.Expose = false;
                var options = GetNewUnitOptions(filter);

                var root = new List<object>();

                if (!uo.IsSceneBound() || reference.serializedObject.IsSceneBound())
                {
                    if (uo == reference.self)
                    {
                        root.Add(new UnitOption<This>(new This()));
                    }

                    root.Add(new LiteralOption(new Literal(type, uo)));
                }

                if (uo is MonoScript script)
                {
                    var scriptType = script.GetClass();

                    if (scriptType != null)
                    {
                        root.Add(scriptType);
                    }
                }
                else
                {
                    root.Add(type);
                }

                if (uo is GameObject)
                {
                    root.AddRange(uo.GetComponents<Component>().Select(c => c.GetType()));
                }

                options.rootOverride = root.ToArray();

                NewUnit(DragAndDropUtility.position, options, (unit) =>
                {
                    // Try to assign a correct input
                    var compatibleInput = unit.CompatibleValueInput(type);

                    if (compatibleInput == null)
                    {
                        return;
                    }

                    if (uo.IsSceneBound())
                    {
                        if (reference.self == uo.GameObject())
                        {
                            // The component is owned by the same game object as the graph.

                            if (compatibleInput.nullMeansSelf)
                            {
                                compatibleInput.SetDefaultValue(null);
                            }
                            else
                            {
                                var self = new This();
                                self.position = unit.position + new Vector2(-150, 19);
                                graph.units.Add(self);
                                self.self.ConnectToValid(compatibleInput);
                            }
                        }
                        else if (reference.serializedObject.IsSceneBound())
                        {
                            // The component is from another object from the same scene
                            compatibleInput.SetDefaultValue(uo.ConvertTo(compatibleInput.type));
                        }
                        else
                        {
                            throw new NotSupportedException("Cannot determine compatible input from dragged Unity object.");
                        }
                    }
                    else
                    {
                        compatibleInput.SetDefaultValue(uo.ConvertTo(compatibleInput.type));
                    }
                });
            }
            else if (EditorVariablesUtility.isDraggingVariable)
            {
                var kind = EditorVariablesUtility.kind;
                var declaration = EditorVariablesUtility.declaration;

                UnifiedVariableUnit unit;

                if (e.alt)
                {
                    unit = new SetVariable();
                }
                else if (e.shift)
                {
                    unit = new IsVariableDefined();
                }
                else
                {
                    unit = new GetVariable();
                }

                unit.kind = kind;
                AddUnit(unit, DragAndDropUtility.position);
                unit.name.SetDefaultValue(declaration.name);
            }
        }

        public override void DrawDragAndDropPreview()
        {
            if (DragAndDropUtility.Is<ScriptGraphAsset>())
            {
                GraphGUI.DrawDragAndDropPreviewLabel(DragAndDropUtility.offsetedPosition, DragAndDropUtility.Get<ScriptGraphAsset>().name, typeof(ScriptGraphAsset).Icon());
            }
            else if (DragAndDropUtility.Is<GameObject>())
            {
                var gameObject = DragAndDropUtility.Get<GameObject>();
                GraphGUI.DrawDragAndDropPreviewLabel(DragAndDropUtility.offsetedPosition, gameObject.name + "...", gameObject.Icon());
            }
            else if (DragAndDropUtility.Is<UnityObject>())
            {
                var obj = DragAndDropUtility.Get<UnityObject>();
                var type = obj.GetType();
                GraphGUI.DrawDragAndDropPreviewLabel(DragAndDropUtility.offsetedPosition, type.HumanName() + "...", type.Icon());
            }
            else if (EditorVariablesUtility.isDraggingVariable)
            {
                var kind = EditorVariablesUtility.kind;
                var name = EditorVariablesUtility.declaration.name;

                string label;

                if (e.alt)
                {
                    label = $"Set {name}";
                }
                else if (e.shift)
                {
                    label = $"Check if {name} is defined";
                }
                else
                {
                    label = $"Get {name}";
                }

                GraphGUI.DrawDragAndDropPreviewLabel(DragAndDropUtility.offsetedPosition, label, BoltCore.Icons.VariableKind(kind));
            }
        }

        #endregion


        #region Drawing

        public bool showRelations { get; set; }

        #endregion


        #region Connection Creation

        public IUnitPort connectionSource { get; set; }

        public Vector2 connectionEnd { get; set; }

        public bool isCreatingConnection => connectionSource != null &&
        connectionSource.unit != null;                                     // Make sure the port didn't get destroyed: https://support.ludiq.io/communities/5/topics/4034-x

        public void CancelConnection()
        {
            connectionSource = null;
        }

        #endregion
    }
}
