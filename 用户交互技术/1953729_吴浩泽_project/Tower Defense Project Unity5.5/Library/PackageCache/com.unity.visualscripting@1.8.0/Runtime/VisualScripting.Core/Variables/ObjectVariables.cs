using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class ObjectVariables
    {
        public static VariableDeclarations Declarations(GameObject source, bool autoAddComponent, bool throwOnMissing)
        {
            Ensure.That(nameof(source)).IsNotNull(source);

            var variables = source.GetComponent<Variables>();

            if (variables == null && autoAddComponent)
            {
                variables = source.AddComponent<Variables>();
            }

            if (variables != null)
            {
                return variables.declarations;
            }

            if (throwOnMissing)
            {
                throw new InvalidOperationException($"Game object '{source.name}' does not have variables.");
            }
            else
            {
                return null;
            }
        }
    }
}
