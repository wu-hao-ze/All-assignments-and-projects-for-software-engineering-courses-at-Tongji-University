#if VISUAL_SCRIPT_INTERNAL
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class RuntimeGraphBase<TMacro, TGraph, TCanvas, TMachine>
        where TMacro : LudiqScriptableObject, IMacro
        where TGraph : IGraph
        where TCanvas : ICanvas
        where TMachine : IMachine
    {
        protected const string k_AssetPath = "Assets/test.asset";

        protected TMacro m_Macro;
        protected TGraph m_Graph;
        protected TCanvas m_Canvas;
        protected TMachine m_Machine;
        protected GraphReference m_Reference;
        protected GameObject m_GameObject;
    }

    public abstract class RuntimeFlowGraph : RuntimeGraphBase<ScriptGraphAsset, FlowGraph, FlowCanvas, ScriptMachine>
    {
        protected void CreateGraph()
        {
            m_Macro = ScriptableObject.CreateInstance<ScriptGraphAsset>();
            AssetDatabase.CreateAsset(m_Macro, k_AssetPath);

            m_Graph = m_Macro.graph;
            m_Canvas = new FlowCanvas(m_Graph);
            m_Reference = GraphReference.New(m_Macro, false);
            m_GameObject = new GameObject();

            m_Machine = m_GameObject.AddComponent<ScriptMachine>();
            m_Machine.nest.macro = m_Macro;
        }

        protected void AddUnit(IUnit unit)
        {
            AddUnit(unit, Vector2.down);
        }

        protected void AddUnit(IUnit unit, Vector2 position)
        {
            Undo.IncrementCurrentGroup();
            LudiqEditorUtility.editedObject.BeginOverride(m_Reference.serializedObject);
            m_Canvas.AddUnit(unit, position);
            LudiqEditorUtility.editedObject.EndOverride();
        }

        protected void Connect(ControlOutput source, ControlInput destination)
        {
            Undo.IncrementCurrentGroup();
            LudiqEditorUtility.editedObject.BeginOverride(m_Reference.serializedObject);
            var widget = new ControlOutputWidget(m_Canvas, source);
            var connectMethodInfo = GetConnectionMethodInfo(widget.GetType());
            connectMethodInfo?.Invoke(widget, new object[] { widget.port, destination });
            LudiqEditorUtility.editedObject.EndOverride();
        }

        protected void Connect(ValueOutput source, ValueInput destination)
        {
            Undo.IncrementCurrentGroup();
            LudiqEditorUtility.editedObject.BeginOverride(m_Reference.serializedObject);
            var widget = new ValueOutputWidget(m_Canvas, source);
            var connectMethodInfo = GetConnectionMethodInfo(widget.GetType());
            connectMethodInfo?.Invoke(widget, new object[] { widget.port, destination });
            LudiqEditorUtility.editedObject.EndOverride();
        }

        static MethodInfo GetConnectionMethodInfo(Type type)
        {
            while (true)
            {
                if (!(type is null))
                {
                    foreach (var mi in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
                    {
                        if (mi.Name == "FinishConnection")
                            return mi;
                    }

                    type = type.BaseType;
                }
            }
        }
    }
}
#endif
