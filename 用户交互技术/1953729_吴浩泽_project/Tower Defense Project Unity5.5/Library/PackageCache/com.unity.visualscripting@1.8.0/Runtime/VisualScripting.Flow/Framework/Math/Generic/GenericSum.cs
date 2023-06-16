using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the sum of two objects.
    /// </summary>
    [UnitCategory("Math/Generic")]
    [UnitTitle("Add")]
    public sealed class GenericSum : Sum<object>
    {
        public override object Operation(object a, object b)
        {
            return OperatorUtility.Add(a, b);
        }

        public override object Operation(IEnumerable<object> values)
        {
            var valueList = values.ToList();
            var result = OperatorUtility.Add(valueList[0], valueList[1]);

            for (int i = 2; i < valueList.Count; i++)
            {
                result = OperatorUtility.Add(result, valueList[i]);
            }

            return result;
        }
    }
}
