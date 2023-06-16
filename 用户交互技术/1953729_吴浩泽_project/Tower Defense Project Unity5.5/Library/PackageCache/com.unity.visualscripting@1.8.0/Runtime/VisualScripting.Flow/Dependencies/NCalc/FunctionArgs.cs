using System;

namespace Unity.VisualScripting.Dependencies.NCalc
{
    public class FunctionArgs : EventArgs
    {
        private object _result;

        private Expression[] _parameters = new Expression[0];

        public object Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                HasResult = true;
            }
        }

        public bool HasResult { get; set; }

        public Expression[] Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        public object[] EvaluateParameters(Flow flow)
        {
            var values = new object[_parameters.Length];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = _parameters[i].Evaluate(flow);
            }

            return values;
        }
    }
}
