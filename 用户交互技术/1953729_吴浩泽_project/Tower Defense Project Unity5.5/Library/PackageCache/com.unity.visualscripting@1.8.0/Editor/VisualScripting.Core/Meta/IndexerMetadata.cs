using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class IndexerMetadata : Metadata
    {
        public IndexerMetadata(object indexer, Metadata parent) : base(indexer, parent)
        {
            if (indexer == null)
            {
                throw new ArgumentNullException(nameof(indexer));
            }

            this.indexer = indexer;
            indexers = new[] { indexer };

            Reflect(true);
        }

        private readonly object[] indexers;

        public object indexer { get; private set; }

        public PropertyInfo indexerProperty { get; private set; }

        protected override object rawValue
        {
            get
            {
                return indexerProperty.GetValue(parent.value, indexers);
            }
            set
            {
                indexerProperty.SetValue(parent.value, value, indexers);
            }
        }

        protected override void OnParentValueChange(object previousValue)
        {
            base.OnParentValueChange(previousValue);

            Reflect(false);
        }

        private void Reflect(bool throwOnFail)
        {
            indexerProperty = parent.valueType.GetProperties().FirstOrDefault(property =>
            {
                var indexParameters = property.GetIndexParameters();

                if (indexParameters.Length != 1)
                {
                    return false;
                }

                if (property.Name != "Item")
                {
                    return false;
                }

                if (indexParameters[0].ParameterType.IsInstanceOfType(indexer))
                {
                    return true;
                }

                return false;
            });

            if (indexerProperty == null)
            {
                if (throwOnFail)
                {
                    throw new InvalidOperationException("Parent of reflected indexer is does not have a matching indexer property:\n" + this);
                }
                else
                {
                    Unlink();
                    return;
                }
            }

            definedType = indexerProperty.PropertyType;
            label = new GUIContent(parent.label);
        }

        public override Attribute[] GetCustomAttributes(bool inherit = true)
        {
            return parent.GetCustomAttributes(inherit);
        }
    }
}
