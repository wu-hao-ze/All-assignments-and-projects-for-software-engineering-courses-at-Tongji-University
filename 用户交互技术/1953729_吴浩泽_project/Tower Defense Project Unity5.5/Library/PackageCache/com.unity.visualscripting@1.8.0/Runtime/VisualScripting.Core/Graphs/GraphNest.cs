using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public sealed class GraphNest<TGraph, TMacro> : IGraphNest
        where TGraph : class, IGraph, new()
        where TMacro : Macro<TGraph>
    {
        [DoNotSerialize]
        public IGraphNester nester { get; set; }

        [DoNotSerialize]
        private GraphSource _source = GraphSource.Macro;

        [DoNotSerialize]
        private TMacro _macro;

        [DoNotSerialize]
        private TGraph _embed;

        [Serialize]
        public GraphSource source
        {
            get => _source;
            set
            {
                if (value == source)
                {
                    return;
                }

                BeforeGraphChange();

                _source = value;

                AfterGraphChange();
            }
        }

        [Serialize]
        public TMacro macro
        {
            get => _macro;
            set
            {
                if (value == macro)
                {
                    return;
                }

                BeforeGraphChange();

                _macro = value;

                AfterGraphChange();
            }
        }

        [Serialize]
        public TGraph embed
        {
            get => _embed;
            set
            {
                if (value == embed)
                {
                    return;
                }

                BeforeGraphChange();

                _embed = value;

                AfterGraphChange();
            }
        }

        [DoNotSerialize]
        public TGraph graph
        {
            get
            {
                switch (source)
                {
                    case GraphSource.Embed:
                        return embed;

                    case GraphSource.Macro:
                        return macro?.graph;

                    default:
                        throw new UnexpectedEnumValueException<GraphSource>(source);
                }
            }
        }

        IMacro IGraphNest.macro
        {
            get => macro;
            set => macro = (TMacro)value;
        }

        IGraph IGraphNest.embed
        {
            get => embed;
            set => embed = (TGraph)value;
        }

        IGraph IGraphNest.graph => graph;

        Type IGraphNest.graphType => typeof(TGraph);

        Type IGraphNest.macroType => typeof(TMacro);

        // TODO: Use these in the editor when appropriate to minimize change events
        public void SwitchToEmbed(TGraph embed)
        {
            if (source == GraphSource.Embed && this.embed == embed)
            {
                return;
            }

            BeforeGraphChange();

            _source = GraphSource.Embed;
            _embed = embed;
            _macro = null;

            AfterGraphChange();
        }

        public void SwitchToMacro(TMacro macro)
        {
            if (source == GraphSource.Macro && this.macro == macro)
            {
                return;
            }

            BeforeGraphChange();

            _source = GraphSource.Macro;
            _embed = null;
            _macro = macro;

            AfterGraphChange();
        }

        public event Action beforeGraphChange;

        public event Action afterGraphChange;

        private void BeforeGraphChange()
        {
            if (graph != null)
            {
                nester.UninstantiateNest();
            }

            beforeGraphChange?.Invoke();
        }

        private void AfterGraphChange()
        {
            afterGraphChange?.Invoke();

            if (graph != null)
            {
                nester.InstantiateNest();
            }
        }

        #region Serialization

        public IEnumerable<ISerializationDependency> deserializationDependencies
        {
            get
            {
                if (macro != null)
                {
                    yield return macro;
                }
            }
        }

        #endregion


        #region Poutine

        public IEnumerable<object> GetAotStubs(HashSet<object> visited)
        {
            return LinqUtility.Concat<object>(graph?.GetAotStubs(visited));
        }

        [DoNotSerialize]
        public bool hasBackgroundEmbed => source == GraphSource.Macro && embed != null;

        #endregion
    }
}
