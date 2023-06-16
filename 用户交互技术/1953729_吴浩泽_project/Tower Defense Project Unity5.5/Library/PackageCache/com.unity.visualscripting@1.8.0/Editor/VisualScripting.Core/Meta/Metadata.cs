using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public abstract class Metadata : IAttributeProvider, IList, IDictionary
    {
        protected Metadata(object subpath, Metadata parent)
        {
            Ensure.That(nameof(subpath)).IsNotNull(subpath);

            if (isRoot)
            {
                Ensure.That(nameof(parent)).IsNull(parent);
            }
            else
            {
                Ensure.That(nameof(parent)).IsNotNull(parent);
            }

            if (parent == null && !(this is RootMetadata))
            {
                throw new ArgumentNullException(nameof(parent));
            }

            this.subpath = subpath;
            this.parent = parent;
            subhash = parent == null ? base.GetHashCode() : subpath.GetHashCode();
            children = new Children();
        }

        protected virtual bool isRoot => false;

        public GUIContent label { get; protected set; }

        public virtual bool isEditable { get; set; } = true;

        public void RecordUndo()
        {
            RecordUndo($"Modify {label?.text ?? "metadata value"}");
        }

        public void RecordUndo(string name)
        {
            UndoUtility.RecordEditedObject(name);
        }

        public void InferOwnerFromParent()
        {
            UnityObjectOwnershipUtility.CopyOwner(parent?.value, value);
        }

        #region Lifecycle

        public bool isLinked => isRoot || parent != null;

        public void Unlink()
        {
            if (isRoot)
            {
                throw new InvalidOperationException("Cannot unlink root metadata.");
            }

            if (!isLinked)
            {
                return;
            }

            var toString = ToString();

            UnlinkChildren();

            parent?.children.Remove(this);

            if (isPrefabInstanceWithDefinition)
            {
                prefabDefinition.Unlink();
            }

            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.trackMetadataState)
            {
                Debug.LogWarning($"Unlinked metadata node:\n{toString}");
            }
        }

        public void UnlinkChildren()
        {
            while (children.Any())
            {
                children.First().Unlink();
            }
        }

        public void EnsureLinked()
        {
            if (!isLinked)
            {
                throw new InvalidOperationException($"Metadata node has been unlinked: '{this}'.");
            }
        }

        #endregion

        #region Hierarchy

        public class Children : KeyedCollection<int, Metadata>, IKeyedCollection<int, Metadata>
        {
            protected override int GetKeyForItem(Metadata item)
            {
                return item.subhash;
            }

            public new bool TryGetValue(int key, out Metadata value)
            {
                if (Dictionary == null)
                {
                    value = default(Metadata);
                    return false;
                }

                return Dictionary.TryGetValue(key, out value);
            }
        }

        public Children children { get; private set; }
        public Metadata parent { get; }
        public string path { get; private set; }

        public Metadata root
        {
            get
            {
                var level = this;

                while (level.parent != null)
                {
                    level = level.parent;
                }

                return level;
            }
        }

        public Metadata Ancestor(Func<Metadata, bool> predicate, bool includeSelf = false)
        {
            var level = includeSelf ? this : parent;

            while (level != null && !predicate(level))
            {
                level = level.parent;
            }

            return level;
        }

        public T Ancestor<T>(bool includeSelf = false) where T : Metadata
        {
            return (T)Ancestor(metadata => metadata is T, includeSelf);
        }

        public IEnumerable<Metadata> Descendants(Func<Metadata, bool> predicate)
        {
            var result = children.Where(predicate);

            foreach (var child in children)
            {
                result = result.Concat(child.Descendants(predicate));
            }

            return result;
        }

        public IEnumerable<T> Descendants<T>() where T : Metadata
        {
            return Descendants(metadata => metadata is T).Cast<T>();
        }

        #endregion

        #region Path

        private readonly int subhash;

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                var level = this;

                do
                {
                    hash = hash * 23 + level.subhash;
                    level = level.parent;
                }
                while (level != null);

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Metadata;

            if (other == null)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }

        protected object subpath { get; private set; }

        private void CachePath()
        {
            path = SubpathToString();

            if (parent != null)
            {
                path = parent.path + "." + path;
            }
        }

        protected virtual string SubpathToString()
        {
            return subpath.ToString();
        }

        public override string ToString()
        {
            return path ?? ("(?)." + SubpathToString());
        }

        #endregion

        #region Defined type

        public Type definedType
        {
            get
            {
                EnsureLinked();

                return _definedType;
            }
            protected set
            {
                _definedType = value;

                instantiator = value.Instantiator();
            }
        }

        protected Type _definedType;

        #endregion

        #region Null Shield

        public Func<object> instantiator { get; set; }

        public bool instantiate { get; set; }

        private bool canInstantiate => instantiate && instantiator != null;

        private object instantiatedValue
        {
            get
            {
                var rawValue = this.rawValue;

                if (rawValue == null && canInstantiate)
                {
                    var fallbackValue = instantiator();

                    if (fallbackValue == null)
                    {
                        throw new InvalidOperationException("Metadata instantiator returns null. Aborting to prevent stack overflow.");
                    }

                    this.rawValue = rawValue = fallbackValue;
                }

                return rawValue;
            }
            set
            {
                if (value == null && canInstantiate)
                {
                    value = instantiator();

                    if (value == null)
                    {
                        throw new InvalidOperationException("AutoMetadata instantiator returns null. Aborting to prevent stack overflow.");
                    }
                }

                rawValue = value;
            }
        }

        #endregion

        #region Value

        private bool obtainedValue;

        private object lastObservedValue;

        protected abstract object rawValue { get; set; }

        public object value
        {
            get
            {
                EnsureLinked();

                try
                {
                    var cachedInstantiatedValue = instantiatedValue;

                    if (!obtainedValue)
                    {
                        lastObservedValue = cachedInstantiatedValue;
                        obtainedValue = true;
                    }
                    else if (!Equals(cachedInstantiatedValue, lastObservedValue))
                    {
                        var previousValue = lastObservedValue;
                        lastObservedValue = cachedInstantiatedValue;
                        OnValueChange(previousValue);
                    }

                    return cachedInstantiatedValue;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to get metadata value for:\n" + this, ex);
                }
            }
            set
            {
                EnsureLinked();

                try
                {
                    var previousValue = instantiatedValue;

                    lastObservedValue = instantiatedValue = value;

                    if (!Equals(value, previousValue))
                    {
                        OnValueChange(previousValue);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to set metadata value for:\n" + this, ex);
                }
            }
        }

        protected virtual void OnValueChange(object previousValue)
        {
            // Force a value type change check by calling the getter.
            var forceWatchValueType = valueType;

            if (PluginContainer.initialized && BoltCore.Configuration.developerMode && BoltCore.Configuration.trackMetadataState)
            {
                Debug.LogFormat
                    (
                        "Value changed on metadata node: {0}\n{1} => {2}",
                        this,
                        previousValue != null ? previousValue.ToString() : "(null)",
                        value != null ? value.ToString() : "(null)"
                    );
            }

            foreach (var child in children)
            {
                child.OnParentValueChange(previousValue);
            }

            if (_valueChanged != null)
            {
                _valueChanged(previousValue);
            }
        }

        protected virtual void OnParentValueChange(object previousValue) { }

        private event Action<object> _valueChanged;

        public event Action<object> valueChanged
        {
            add
            {
                lastObservedValue = instantiatedValue;
                obtainedValue = true;
                value(this.value);
                _valueChanged += value;
            }
            remove
            {
                _valueChanged -= value;
            }
        }

        #endregion

        #region Value Type

        private bool obtainedValueType;

        private Type rawValueType
        {
            get
            {
                var cachedInstantiatedValue = instantiatedValue;

                return cachedInstantiatedValue != null ? cachedInstantiatedValue.GetType() : definedType;
            }
        }

        private Type lastObservedValueType;

        public Type nullableValueType => instantiatedValue?.GetType();

        public Type valueType
        {
            get
            {
                EnsureLinked();

                var cachedRawValueType = rawValueType;

                if (!obtainedValueType)
                {
                    lastObservedValueType = cachedRawValueType;
                    obtainedValueType = true;
                }
                else if (cachedRawValueType != lastObservedValueType)
                {
                    var previousValueType = lastObservedValueType;
                    lastObservedValueType = cachedRawValueType;
                    OnValueTypeChange(previousValueType);
                }

                return cachedRawValueType;
            }
        }

        private void AnalyzeCollection()
        {
            isEnumerable = typeof(IEnumerable).IsAssignableFrom(valueType);

            if (isEnumerable)
            {
                enumerableType = valueType;
                enumerableElementType = TypeUtility.GetEnumerableElementType(enumerableType, true);
            }
            else
            {
                enumerableType = null;
                enumerableElementType = null;
            }

            isList = typeof(IList).IsAssignableFrom(valueType);

            if (isList)
            {
                listType = valueType;
                listElementType = TypeUtility.GetListElementType(listType, true);
            }
            else
            {
                listType = null;
                listElementType = null;
            }

            isDictionary = typeof(IDictionary).IsAssignableFrom(valueType);
            isOrderedDictionary = typeof(IOrderedDictionary).IsAssignableFrom(valueType);

            if (isDictionary)
            {
                dictionaryType = valueType;
                dictionaryKeyType = TypeUtility.GetDictionaryKeyType(dictionaryType, true);
                dictionaryValueType = TypeUtility.GetDictionaryValueType(dictionaryType, true);
            }
            else
            {
                dictionaryType = null;
                dictionaryKeyType = null;
                dictionaryValueType = null;
            }
        }

        protected virtual void OnValueTypeChange(Type previousType)
        {
            if (PluginContainer.initialized && BoltCore.Configuration.developerMode && BoltCore.Configuration.trackMetadataState)
            {
                Debug.LogFormat
                    (
                        "Value type changed on metadata node: {0}\n{1} => {2}",
                        this,
                        previousType != null ? previousType.CSharpName(false) : "(null)",
                        valueType != null ? valueType.CSharpName(false) : "(null)"
                    );
            }

            AnalyzeCollection();

            foreach (var child in children)
            {
                child.OnParentValueTypeChange(previousType);
            }

            if (_valueTypeChanged != null)
            {
                _valueTypeChanged(previousType);
            }
        }

        protected virtual void OnParentValueTypeChange(Type previousType) { }

        private event Action<Type> _valueTypeChanged;

        public event Action<Type> valueTypeChanged
        {
            add
            {
                lastObservedValueType = rawValueType;
                obtainedValueType = true;
                value(valueType);
                _valueTypeChanged += value;
            }
            remove
            {
                _valueTypeChanged -= value;
            }
        }

        #endregion

        #region Digging

        // Using TSubpath to avoid boxing and alloc of object
        protected TMetadata Dig<TSubpath, TMetadata>(TSubpath subpath, Func<Metadata, TMetadata> constructor, bool createInPrefab, Metadata prefabInstance = null) where TMetadata : Metadata
        {
            if (subpath == null)
            {
                throw new ArgumentNullException(nameof(subpath));
            }

            if (constructor == null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            var subhash = subpath.GetHashCode();

            Metadata child;

            if (children.TryGetValue(subhash, out child))
            {
                if (child is TMetadata)
                {
                    return (TMetadata)child;
                }
                else
                {
                    throw new InvalidOperationException($"Metadata mismatch: expected '{typeof(TMetadata).Name}', found '{child.GetType().Name}'.");
                }
            }
            else
            {
                try
                {
                    var containsBefore = children.Contains(subhash);

                    child = constructor(this);

                    if (!containsBefore && children.Contains(child.subhash))
                    {
                        throw new InvalidOperationException($"Children didn't contain '{subpath}' subpath before the constructor but now does.");
                    }

                    child.CachePath();
                    child.AnalyzeCollection();

                    if (isPrefabInstanceWithDefinition)
                    {
                        if (createInPrefab)
                        {
                            child.prefabDefinition = prefabDefinition.Dig(subpath, constructor, false, child);
                        }
                        else if (prefabDefinition.children.Contains(subhash))
                        {
                            child.prefabDefinition = prefabDefinition.children[subhash];
                        }
                        else
                        {
                            child.isPrefabInstanceWithoutDefinition = true;
                        }
                    }
                    else if (isPrefabInstanceWithoutDefinition)
                    {
                        child.isPrefabInstanceWithoutDefinition = true;
                    }

                    children.Add(child);

                    if (PluginContainer.initialized && BoltCore.Configuration.developerMode && BoltCore.Configuration.trackMetadataState)
                    {
                        Debug.LogWarningFormat
                            (
                                "Created {0} node{1}:\n{2}",
                                child.GetType().CSharpName(false),
                                child.isPrefabInstance ? " (prefab instance)" : (prefabInstance != null ? " (prefab definition)" : ""),
                                child
                            );
                    }

                    return (TMetadata)child;
                }
                catch
                {
                    // If digging fails and we're creating a prefab definition mirror,
                    // we will simply notify the instance metadata that it has no hierarchy equivalent.
                    if (prefabInstance != null)
                    {
                        prefabInstance.isPrefabInstanceWithoutDefinition = true;
                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Prefabs

        public Metadata prefabDefinition { get; protected set; }

        public bool isPrefabInstance => isPrefabInstanceWithDefinition || isPrefabInstanceWithoutDefinition;

        public bool isPrefabInstanceWithDefinition => prefabDefinition != null;

        public bool isPrefabInstanceWithoutDefinition { get; private set; }

        public bool isPrefabRoot => isPrefabInstanceWithDefinition && (parent == null || !parent.isPrefabInstanceWithDefinition);

        public bool isPrefabDiff => isPrefabInstanceWithoutDefinition || (isPrefabInstanceWithDefinition && !Equals(value, prefabDefinition.value));

        public bool isRevertibleToPrefab => isPrefabInstanceWithDefinition;

        public void RevertToPrefab()
        {
            RecordUndo($"Revert {label?.text ?? "metadata value"} to prefab");

            value = prefabDefinition.value.CloneViaSerialization(isPrefabRoot ? (UnityObject)value : null);
        }

        public void MatchWithPrefab()
        {
            var unityObject = value as UnityObject;

            if (unityObject == null)
            {
                throw new InvalidOperationException("Trying to match a non Unity object metadata with a prefab.");
            }

            if (unityObject.IsConnectedPrefabInstance())
            {
                var definition = unityObject.GetPrefabDefinition();
                // Prefab could have been deleted
                prefabDefinition = definition != null ? Root().StaticObject(definition) : null;
            }
        }

        #endregion

        #region Attributes

        public abstract Attribute[] GetCustomAttributes(bool inherit = true);

        public bool AncestorHasAttribute(Type attributeType, bool inherit = true)
        {
            var level = this;

            while (level != null)
            {
                if (level.HasAttribute(attributeType, inherit))
                {
                    return true;
                }

                level = level.parent;
            }

            return false;
        }

        public Attribute GetAncestorAttribute(Type attributeType, bool inherit = true)
        {
            var level = this;

            Attribute attribute = null;

            while (level != null)
            {
                attribute = level.GetAttribute(attributeType, inherit);

                if (attribute != null)
                {
                    break;
                }

                level = level.parent;
            }

            return attribute;
        }

        public IEnumerable<Attribute> GetAncestorAttributes(Type attributeType, bool inherit)
        {
            var level = this;

            while (level != null)
            {
                foreach (var attribute in level.GetAttributes(attributeType, inherit))
                {
                    yield return attribute;
                }

                level = level.parent;
            }
        }

        public bool AncestorHasAttribute<TAttribute>(bool inherit = true) where TAttribute : Attribute
        {
            return AncestorHasAttribute(typeof(TAttribute), inherit);
        }

        public TAttribute GetAncestorAttribute<TAttribute>(bool inherit = true) where TAttribute : Attribute
        {
            return (TAttribute)GetAncestorAttribute(typeof(TAttribute), inherit);
        }

        public IEnumerable<TAttribute> GetAncestorAttributes<TAttribute>(bool inherit = true) where TAttribute : Attribute
        {
            return GetAncestorAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
        }

        #endregion

        #region Indexers

        public MemberMetadata this[string name] => Member(name);

        public IndexMetadata this[int index] => Index(index);

        #endregion

        #region Shared

        public IEnumerator GetEnumerator()
        {
            if (isDictionary)
            {
                return dictionary.GetEnumerator();
            }
            if (isList)
            {
                return list.GetEnumerator();
            }
            else if (isEnumerable)
            {
                return enumerable.GetEnumerator();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public int Count
        {
            get
            {
                if (isDictionary)
                {
                    return dictionary.Count;
                }
                if (isList)
                {
                    return list.Count;
                }
                else if (isEnumerable)
                {
                    return enumerable.Cast<object>().Count();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool IsFixedSize
        {
            get
            {
                if (isDictionary)
                {
                    return dictionary.IsFixedSize;
                }
                else if (isList)
                {
                    return !listType.IsArray && list.IsFixedSize;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (isDictionary)
                {
                    return dictionary.IsReadOnly;
                }
                else if (isList)
                {
                    return list.IsReadOnly;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool IsSynchronized
        {
            get
            {
                if (isDictionary)
                {
                    return dictionary.IsSynchronized;
                }
                else if (isList)
                {
                    return list.IsSynchronized;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public object SyncRoot
        {
            get
            {
                if (isDictionary)
                {
                    return dictionary.SyncRoot;
                }
                else if (isList)
                {
                    return list.SyncRoot;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool Contains(object value)
        {
            if (isDictionary)
            {
                return dictionary.Contains(value);
            }
            else if (isList)
            {
                return list.Contains(value);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void Remove(object value)
        {
            if (isDictionary)
            {
                dictionary.Remove(value);
                UnlinkDictionaryChildren();
            }
            else if (isList)
            {
                var list = GetResizableList();
                list.Remove(value);
                ApplyResizableList();
                UnlinkListChildren();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void Clear()
        {
            if (isDictionary)
            {
                dictionary.Clear();
                UnlinkDictionaryChildren();
            }
            else if (isList)
            {
                var list = GetResizableList();
                list.Clear();
                ApplyResizableList();
                UnlinkListChildren();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        #endregion

        #region Enumerable

        public bool isEnumerable { get; private set; }

        public Type enumerableType { get; private set; }

        public Type enumerableElementType { get; private set; }

        private IEnumerable enumerable
        {
            get
            {
                if (!isEnumerable)
                {
                    throw new InvalidOperationException();
                }

                return (IEnumerable)value;
            }
        }

        #endregion

        #region List

        public bool isList { get; private set; }

        public Type listType { get; private set; }

        public Type listElementType { get; private set; }

        private IList list
        {
            get
            {
                if (!isList)
                {
                    throw new InvalidOperationException();
                }

                return (IList)value;
            }
        }

        object IList.this[int index]
        {
            get
            {
                if (!isList)
                {
                    throw new InvalidOperationException();
                }

                return Index(index).value;
            }
            set
            {
                if (!isList)
                {
                    throw new InvalidOperationException();
                }

                Index(index).value = value;
            }
        }

        private void UnlinkListChildren()
        {
            var listChildren = children.OfType<IndexMetadata>().ToArray();

            foreach (var listChild in listChildren)
            {
                listChild.Unlink();
            }
        }

        private IList resizableList;

        protected IList GetResizableList()
        {
            if (listType.IsArray)
            {
                if (resizableList == null)
                {
                    resizableList = new List<object>();
                }
                else
                {
                    resizableList.Clear();
                }

                foreach (var item in list)
                {
                    resizableList.Add(item);
                }

                return resizableList;
            }
            else
            {
                return list;
            }
        }

        protected void ApplyResizableList()
        {
            if (listType.IsArray)
            {
                var array = Array.CreateInstance(listElementType, resizableList.Count);
                resizableList.CopyTo(array, 0);
                value = array;
            }
        }

        public int Add(object value)
        {
            if (!isList)
            {
                throw new InvalidOperationException();
            }

            var list = GetResizableList();
            var newIndex = list.Add(value);
            ApplyResizableList();
            return newIndex;
        }

        public void Insert(int index, object value)
        {
            if (!isList)
            {
                throw new InvalidOperationException();
            }

            var list = GetResizableList();
            list.Insert(index, value);
            ApplyResizableList();
        }

        public int IndexOf(object value)
        {
            if (!isList)
            {
                throw new InvalidOperationException();
            }

            return list.IndexOf(value);
        }

        public void RemoveAt(int index)
        {
            if (!isList)
            {
                throw new InvalidOperationException();
            }

            var list = GetResizableList();
            list.RemoveAt(index);
            ApplyResizableList();
            UnlinkListChildren();
        }

        public void CopyTo(Array array, int index)
        {
            if (!isList)
            {
                throw new InvalidOperationException();
            }

            list.CopyTo(array, index);
        }

        public void Move(int sourceIndex, int destinationIndex)
        {
            if (!isList)
            {
                throw new InvalidOperationException();
            }

            if (destinationIndex > sourceIndex)
            {
                destinationIndex--;
            }

            var list = GetResizableList();
            var item = list[sourceIndex];
            list.RemoveAt(sourceIndex);
            list.Insert(destinationIndex, item);
            ApplyResizableList();

            UnlinkListChildren();
        }

        public void Duplicate(int index)
        {
            if (!isList)
            {
                throw new InvalidOperationException();
            }

            object newItem;

            var list = GetResizableList();

            if (Index(index).valueType.IsValueType)
            {
                newItem = list[index];
            }
            else if (typeof(ICloneable).IsAssignableFrom(Index(index).valueType))
            {
                newItem = ((ICloneable)list[index]).Clone();
            }
            else
            {
                newItem = list[index].CloneViaSerialization();
            }

            var keyCollection = list as VariableDeclarationCollection;
            var currentItem = newItem as VariableDeclaration;
            if (keyCollection != null && currentItem != null)
            {
                var currentName = currentItem.name;
                var count = 0;

                while (keyCollection.TryGetValue(currentItem.name, out _))
                {
                    count++;
                    var newName = $"{currentName} ({count})";
                    currentItem = new VariableDeclaration(newName, currentItem.value);
                }
                newItem = currentItem;
            }

            list.Insert(index + 1, newItem);

            ApplyResizableList();
        }

        #endregion

        #region Dictionary

        public bool isDictionary { get; private set; }

        public bool isOrderedDictionary { get; private set; }

        public Type dictionaryType { get; private set; }

        public Type dictionaryKeyType { get; private set; }

        public Type dictionaryValueType { get; private set; }

        private IDictionary dictionary
        {
            get
            {
                if (!isDictionary)
                {
                    throw new InvalidOperationException();
                }

                return (IDictionary)value;
            }
        }

        public ICollection Keys
        {
            get
            {
                if (!isDictionary)
                {
                    throw new InvalidOperationException();
                }

                return dictionary.Keys;
            }
        }

        public ICollection Values
        {
            get
            {
                if (!isDictionary)
                {
                    throw new InvalidOperationException();
                }

                return dictionary.Values;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (!isDictionary)
                {
                    throw new InvalidOperationException();
                }

                return Indexer(key).value;
            }
            set
            {
                if (!isDictionary)
                {
                    throw new InvalidOperationException();
                }

                Indexer(key).value = value;
            }
        }

        public void Add(object key, object value)
        {
            if (!isDictionary)
            {
                throw new InvalidOperationException();
            }

            dictionary.Add(key, value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            if (!isDictionary)
            {
                throw new InvalidOperationException();
            }

            return dictionary.GetEnumerator();
        }

        public Metadata KeyMetadata(int index)
        {
            if (!isDictionary)
            {
                throw new InvalidOperationException();
            }

            return DictionaryKeyAt(index);
        }

        public Metadata ValueMetadata(int index)
        {
            if (!isDictionary)
            {
                throw new InvalidOperationException();
            }

            return DictionaryValueAt(index);
        }

        private void UnlinkDictionaryChildren()
        {
            var dictionaryChildren = children.Where(c =>
                c is IndexerMetadata ||
                c is DictionaryIndexMetadata).ToArray();

            foreach (var dicitonaryChild in dictionaryChildren)
            {
                dicitonaryChild.Unlink();
            }
        }

        #endregion

        #region Digs

        private abstract class NoAllocDelegate<T>
        {
            protected T @delegate;
        }

        private abstract class NoAllocDig<T> : NoAllocDelegate<Func<Metadata, T>> where T : Metadata { }

        private class DigMember : NoAllocDig<MemberMetadata>
        {
            public DigMember()
            {
                @delegate = parent => new MemberMetadata(name, bindingFlags, parent);
            }

            private string name;
            private BindingFlags bindingFlags;

            public Func<Metadata, MemberMetadata> Get(string name, BindingFlags bindingFlags)
            {
                this.name = name;
                this.bindingFlags = bindingFlags;
                return @delegate;
            }
        }

        private class DigIndex : NoAllocDig<IndexMetadata>
        {
            public DigIndex()
            {
                @delegate = parent => new IndexMetadata(index, parent);
            }

            private int index;

            public Func<Metadata, IndexMetadata> Get(int index)
            {
                this.index = index;
                return @delegate;
            }
        }

        private class DigIndexer : NoAllocDig<IndexerMetadata>
        {
            public DigIndexer()
            {
                @delegate = parent => new IndexerMetadata(indexer, parent);
            }

            private object indexer;

            public Func<Metadata, IndexerMetadata> Get(object indexer)
            {
                this.indexer = indexer;
                return @delegate;
            }
        }

        private class DigCast : NoAllocDig<CastMetadata>
        {
            public DigCast()
            {
                @delegate = parent => new CastMetadata(type, parent);
            }

            private Type type;

            public Func<Metadata, CastMetadata> Get(Type type)
            {
                this.type = type;
                return @delegate;
            }
        }

        private class DigStaticObject : NoAllocDig<ObjectMetadata>
        {
            public DigStaticObject()
            {
                @delegate = parent => new ObjectMetadata(@object, definedType, parent);
            }

            private object @object;
            private Type definedType;

            public Func<Metadata, ObjectMetadata> Get(object @object, Type definedType)
            {
                this.@object = @object;
                this.definedType = definedType;
                return @delegate;
            }
        }

        private static readonly DigMember digMember = new DigMember();
        private static readonly DigIndex digIndex = new DigIndex();
        private static readonly DigIndexer digIndexer = new DigIndexer();
        private static readonly DigCast digCast = new DigCast();
        private static readonly DigStaticObject digStaticObject = new DigStaticObject();

        public static Metadata Root()
        {
            var root = new RootMetadata();
            root.CachePath();
            return root;
        }

        public ObjectMetadata StaticObject(object @object, Type definedType)
        {
            return Dig(@object, digStaticObject.Get(@object, definedType), false);
        }

        public ObjectMetadata StaticObject(object @object)
        {
            Ensure.That(nameof(@object)).IsNotNull(@object);

            return StaticObject(@object, @object.GetType());
        }

        public ObjectMetadata Object(string name, object @object, Type definedType)
        {
            return Dig(name, parent => new ObjectMetadata(name, @object, definedType, parent), false);
        }

        public ObjectMetadata Object(string name, object @object)
        {
            return Object(name, @object, typeof(object));
        }

        public MemberMetadata Member(string name, BindingFlags bindingFlags = MemberMetadata.DefaultBindingFlags)
        {
            return Dig(name, digMember.Get(name, bindingFlags), true);
        }

        public IndexMetadata Index(int index)
        {
            return Dig(index, digIndex.Get(index), true);
        }

        public IndexerMetadata Indexer(object indexer)
        {
            return Dig(indexer, digIndexer.Get(indexer), true);
        }

        public CastMetadata Cast(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            return Dig(type, digCast.Get(type), true);
        }

        public CastMetadata Cast<T>()
        {
            return Cast(typeof(T));
        }

        public DictionaryKeyAtIndexMetadata DictionaryKeyAt(int index)
        {
            return Dig("__keyAt." + index, parent => new DictionaryKeyAtIndexMetadata(index, parent), false);
        }

        public DictionaryValueAtIndexMetadata DictionaryValueAt(int index)
        {
            return Dig("__valueAt." + index, parent => new DictionaryValueAtIndexMetadata(index, parent), false);
        }

        public ProxyMetadata Proxy(object subpath, Metadata binding)
        {
            return Dig(subpath, parent => new ProxyMetadata(subpath, binding, parent), false);
        }

        public EditorPrefMetadata EditorPref(PluginConfiguration configuration, MemberInfo member)
        {
            return Dig(member, parent => new EditorPrefMetadata(configuration, member, parent), false);
        }

        public ProjectSettingMetadata ProjectSetting(PluginConfiguration configuration, MemberInfo member)
        {
            return Dig(member, parent => new ProjectSettingMetadata(configuration, member, parent), false);
        }

        public Metadata AutoDig(string path)
        {
            var metadata = this;

            foreach (var pathPart in path.Split('.'))
            {
                string fieldName;
                int index;

                if (SerializedPropertyUtility.IsPropertyIndexer(pathPart, out fieldName, out index))
                {
                    metadata = metadata.Member(fieldName);
                    metadata = metadata.Index(index);
                }
                else
                {
                    metadata = metadata.Member(fieldName);
                }
            }

            return metadata;
        }

        public static Metadata FromProperty(SerializedProperty property)
        {
            return Root().StaticObject(property.serializedObject.targetObject).AutoDig(property.FixedPropertyPath());
        }

        #endregion
    }
}
