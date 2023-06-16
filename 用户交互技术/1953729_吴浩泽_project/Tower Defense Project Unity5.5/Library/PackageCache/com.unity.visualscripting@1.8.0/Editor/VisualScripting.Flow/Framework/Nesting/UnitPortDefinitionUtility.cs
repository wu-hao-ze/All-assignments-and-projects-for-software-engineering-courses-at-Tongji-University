using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public static class UnitPortDefinitionUtility
    {
        public static string Label(this IUnitPortDefinition definition)
        {
            return StringUtility.FallbackWhitespace(definition.label, definition.key?.Filter(symbols: false, punctuation: false).Prettify() ?? "?");
        }

        public static IEnumerable<Warning> Warnings(FlowGraph graph, IEnumerable<IUnitPortDefinition> definitions = null)
        {
            if (definitions == null)
            {
                definitions = LinqUtility.Concat<IUnitPortDefinition>(graph.controlInputDefinitions,
                    graph.controlOutputDefinitions,
                    graph.valueInputDefinitions,
                    graph.valueOutputDefinitions);
            }

            var hasDuplicate = definitions.DistinctBy(d => d.key).Count() != definitions.Count();

            if (hasDuplicate)
            {
                yield return Warning.Caution("Some port definitions with non-unique keys are currently ignored.");
            }

            foreach (var definition in definitions)
            {
                if (!definition.isValid)
                {
                    yield return InvalidWarning(definition);
                }
            }
        }

        public static Warning InvalidWarning(IUnitPortDefinition definition)
        {
            if (!StringUtility.IsNullOrWhiteSpace(definition.label))
            {
                return Warning.Caution($"{definition.GetType().HumanName().ToLower().FirstCharacterToUpper()} '{definition.label}' is not properly configured and is currently ignored.");
            }
            else
            {
                return Warning.Caution($"A {definition.GetType().HumanName().ToLower()} with incomplete configuration is currently ignored.");
            }
        }
    }
}
