using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    [InitializeAfterPlugins]
    public static class UnitBaseStateExtensions
    {
        static UnitBaseStateExtensions()
        {
            UnitBase.staticUnitsExtensions.Add(GetStaticOptions);
            UnitBase.dynamicUnitsExtensions.Add(GetDynamicOptions);
            UnitBase.contextualUnitsExtensions.Add(GetContextualOptions);
        }

        private static IEnumerable<IUnitOption> GetStaticOptions()
        {
            yield return StateUnit.WithStart().Option();
        }

        private static IEnumerable<IUnitOption> GetDynamicOptions()
        {
            var stateMacros = UnityAPI.Await(() => AssetUtility.GetAllAssetsOfType<StateGraphAsset>().ToArray());

            foreach (var stateUnit in stateMacros.Select(statemacro => new StateUnit(statemacro)))
            {
                yield return stateUnit.Option();
            }
        }

        private static IEnumerable<IUnitOption> GetContextualOptions(GraphReference reference)
        {
            yield break;
        }
    }
}
