using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Unity.VisualScripting.Dependencies.NCalc
{
    public class Expression
    {
        private Expression()
        {
            // Fix: the original grammar doesn't include a null identifier.
            Parameters["null"] = Parameters["NULL"] = null;
        }

        public Expression(string expression, EvaluateOptions options = EvaluateOptions.None) : this()
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentException("Expression can't be empty", nameof(expression));
            }

            // Fix: The original grammar doesn't allow double quotes for strings.
            expression = expression.Replace('\"', '\'');

            OriginalExpression = expression;
            Options = options;
        }

        public Expression(LogicalExpression expression, EvaluateOptions options = EvaluateOptions.None) : this()
        {
            if (expression == null)
            {
                throw new ArgumentException("Expression can't be null", nameof(expression));
            }

            ParsedExpression = expression;
            Options = options;
        }

        public event EvaluateFunctionHandler EvaluateFunction;
        public event EvaluateParameterHandler EvaluateParameter;

        /// <summary>
        /// Textual representation of the expression to evaluate.
        /// </summary>
        protected readonly string OriginalExpression;

        protected Dictionary<string, IEnumerator> ParameterEnumerators;

        private Dictionary<string, object> _parameters;

        public EvaluateOptions Options { get; set; }

        public string Error { get; private set; }

        public LogicalExpression ParsedExpression { get; private set; }

        public Dictionary<string, object> Parameters
        {
            get
            {
                return _parameters ?? (_parameters = new Dictionary<string, object>());
            }
            set
            {
                _parameters = value;
            }
        }

        public void UpdateUnityTimeParameters()
        {
            Parameters["dt"] = Parameters["DT"] = Time.deltaTime;
            Parameters["second"] = Parameters["Second"] = 1 / Time.deltaTime;
        }

        /// <summary>
        /// Pre-compiles the expression in order to check syntax errors.
        /// If errors are detected, the Error property contains the message.
        /// </summary>
        /// <returns>True if the expression syntax is correct, otherwise false</returns>
        public bool HasErrors()
        {
            try
            {
                if (ParsedExpression == null)
                {
                    ParsedExpression = Compile(OriginalExpression, (Options & EvaluateOptions.NoCache) == EvaluateOptions.NoCache);
                }

                // In case HasErrors() is called multiple times for the same expression
                return ParsedExpression != null && Error != null;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return true;
            }
        }

        public object Evaluate(Flow flow)
        {
            if (HasErrors())
            {
                throw new EvaluationException(Error);
            }

            if (ParsedExpression == null)
            {
                ParsedExpression = Compile(OriginalExpression, (Options & EvaluateOptions.NoCache) == EvaluateOptions.NoCache);
            }

            var visitor = new EvaluationVisitor(flow, Options);
            visitor.EvaluateFunction += EvaluateFunction;
            visitor.EvaluateParameter += EvaluateParameter;
            visitor.Parameters = Parameters;

            // If array evaluation, execute the same expression multiple times
            if ((Options & EvaluateOptions.IterateParameters) == EvaluateOptions.IterateParameters)
            {
                var size = -1;

                ParameterEnumerators = new Dictionary<string, IEnumerator>();

                foreach (var parameter in Parameters.Values)
                {
                    if (parameter is IEnumerable enumerable)
                    {
                        var localsize = 0;

                        foreach (var o in enumerable)
                        {
                            localsize++;
                        }

                        if (size == -1)
                        {
                            size = localsize;
                        }
                        else if (localsize != size)
                        {
                            throw new EvaluationException("When IterateParameters option is used, IEnumerable parameters must have the same number of items.");
                        }
                    }
                }

                foreach (var key in Parameters.Keys)
                {
                    var parameter = Parameters[key] as IEnumerable;
                    if (parameter != null)
                    {
                        ParameterEnumerators.Add(key, parameter.GetEnumerator());
                    }
                }

                var results = new List<object>();

                for (var i = 0; i < size; i++)
                {
                    foreach (var key in ParameterEnumerators.Keys)
                    {
                        var enumerator = ParameterEnumerators[key];
                        enumerator.MoveNext();
                        Parameters[key] = enumerator.Current;
                    }

                    ParsedExpression.Accept(visitor);
                    results.Add(visitor.Result);
                }

                return results;
            }
            else
            {
                ParsedExpression.Accept(visitor);
                return visitor.Result;
            }
        }

        public static LogicalExpression Compile(string expression, bool noCache)
        {
            LogicalExpression logicalExpression = null;

            if (_cacheEnabled && !noCache)
            {
                try
                {
                    Rwl.AcquireReaderLock(Timeout.Infinite);

                    if (_compiledExpressions.ContainsKey(expression))
                    {
                        Trace.TraceInformation("Expression retrieved from cache: " + expression);
                        var wr = _compiledExpressions[expression];
                        logicalExpression = wr.Target as LogicalExpression;

                        if (wr.IsAlive && logicalExpression != null)
                        {
                            return logicalExpression;
                        }
                    }
                }
                finally
                {
                    Rwl.ReleaseReaderLock();
                }
            }

            if (logicalExpression == null)
            {
                var lexer = new NCalcLexer(new ANTLRStringStream(expression));
                var parser = new NCalcParser(new CommonTokenStream(lexer));

                logicalExpression = parser.ncalcExpression().value;

                if (parser.Errors != null && parser.Errors.Count > 0)
                {
                    throw new EvaluationException(String.Join(Environment.NewLine, parser.Errors.ToArray()));
                }

                if (_cacheEnabled && !noCache)
                {
                    try
                    {
                        Rwl.AcquireWriterLock(Timeout.Infinite);
                        _compiledExpressions[expression] = new WeakReference(logicalExpression);
                    }
                    finally
                    {
                        Rwl.ReleaseWriterLock();
                    }

                    CleanCache();

                    Trace.TraceInformation("Expression added to cache: " + expression);
                }
            }

            return logicalExpression;
        }

        #region Cache management

        private static bool _cacheEnabled = true;
        private static Dictionary<string, WeakReference> _compiledExpressions = new Dictionary<string, WeakReference>();
        private static readonly ReaderWriterLock Rwl = new ReaderWriterLock();

        public static bool CacheEnabled
        {
            get
            {
                return _cacheEnabled;
            }
            set
            {
                _cacheEnabled = value;

                if (!CacheEnabled)
                {
                    // Clears cache
                    _compiledExpressions = new Dictionary<string, WeakReference>();
                }
            }
        }

        /// <summary>
        /// Removes unused entries from cached compiled expression.
        /// </summary>
        private static void CleanCache()
        {
            var keysToRemove = new List<string>();

            try
            {
                Rwl.AcquireWriterLock(Timeout.Infinite);

                foreach (var de in _compiledExpressions)
                {
                    if (!de.Value.IsAlive)
                    {
                        keysToRemove.Add(de.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    _compiledExpressions.Remove(key);
                    Trace.TraceInformation("Cache entry released: " + key);
                }
            }
            finally
            {
                Rwl.ReleaseReaderLock();
            }
        }

        #endregion
    }
}
