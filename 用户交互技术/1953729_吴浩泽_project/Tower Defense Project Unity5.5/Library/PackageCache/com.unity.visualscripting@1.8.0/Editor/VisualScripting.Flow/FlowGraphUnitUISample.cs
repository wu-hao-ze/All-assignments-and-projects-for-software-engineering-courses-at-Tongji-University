#if VISUAL_SCRIPT_INTERNAL
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class FlowGraphUnitUISample : RuntimeFlowGraph
{
    [MenuItem("Tools/Visual Scripting/Internal/Create Node UI Samples", priority = LudiqProduct.DeveloperToolsMenuPriority + 403)]

    public static void CreateUnitUISamples()
    {
        (new FlowGraphUnitUISample()).CreateGraphUISample();
    }

    private void CreateGraphUISample()
    {
        CreateGraph();

        IEnumerable<Type> GetEventUnitTypes() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => typeof(IUnit).IsAssignableFrom(t))).Where(t => t.IsClass && !t.IsAbstract);

        Vector2 position = Vector2.zero;

        int index = 0;

        foreach (var unitType in GetEventUnitTypes())
        {
            try
            {
                string name = unitType.Assembly.GetName().Name;
                string space = unitType.FullName;

                var unit = Activator.CreateInstance(name, space);

                IUnit b = (IUnit)unit.Unwrap();

                b.position = position;

                if (index % 10 == 0)
                {
                    position.x = 0;
                    position.y += 180;
                }

                position.x += 180;

                AddUnit(b, position);

                index++;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
#endif
