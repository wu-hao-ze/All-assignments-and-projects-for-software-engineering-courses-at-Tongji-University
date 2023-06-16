using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
#endif

namespace Unity.VisualScripting.FullSerializer
{
    public class fsSerializer
    {
        public fsSerializer()
        {
            _cachedConverterTypeInstances = new Dictionary<Type, fsBaseConverter>();
            _cachedConverters = new Dictionary<Type, fsBaseConverter>();
            _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();

            _references = new fsCyclicReferenceManager();
            _lazyReferenceWriter = new fsLazyCycleDefinitionWriter();

            // note: The order here is important. Items at the beginning of this
            //       list will be used before converters at the end. Converters
            //       added via AddConverter() are added to the front of the list.
            _availableConverters = new List<fsConverter>
            {
                new fsNullableConverter { Serializer = this },
                new fsGuidConverter { Serializer = this },
                new fsTypeConverter { Serializer = this },
                new fsDateConverter { Serializer = this },
                new fsEnumConverter { Serializer = this },
                new fsPrimitiveConverter { Serializer = this },
                new fsArrayConverter { Serializer = this },
                new fsDictionaryConverter { Serializer = this },
                new fsIEnumerableConverter { Serializer = this },
                new fsKeyValuePairConverter { Serializer = this },
                new fsWeakReferenceConverter { Serializer = this },
                new fsReflectedConverter { Serializer = this }
            };
            _availableDirectConverters = new Dictionary<Type, fsDirectConverter>();

            _processors = new List<fsObjectProcessor>()
            {
                new fsSerializationCallbackProcessor()
            };

#if !NO_UNITY
            _processors.Add(new fsSerializationCallbackReceiverProcessor());
#endif

            _abstractTypeRemap = new Dictionary<Type, Type>();
            SetDefaultStorageType(typeof(ICollection<>), typeof(List<>));
            SetDefaultStorageType(typeof(IList<>), typeof(List<>));
            SetDefaultStorageType(typeof(IDictionary<,>), typeof(Dictionary<,>));

            Context = new fsContext();
            Config = new fsConfig();

            // Register the converters from the registrar
            foreach (var converterType in fsConverterRegistrar.Converters)
            {
                AddConverter((fsBaseConverter)Activator.CreateInstance(converterType));
            }
        }

        /// <summary>
        /// Converters that can be used for type registration.
        /// </summary>
        private readonly List<fsConverter> _availableConverters;

        /// <summary>
        /// Direct converters (optimized _converters). We use these so we don't
        /// have to perform a scan through every item in _converters and can
        /// instead just do an O(1) lookup. This is potentially important to perf
        /// when there are a ton of direct converters.
        /// </summary>
        private readonly Dictionary<Type, fsDirectConverter> _availableDirectConverters;

        /// <summary>
        /// Processors that are available.
        /// </summary>
        private readonly List<fsObjectProcessor> _processors;

        /// <summary>
        /// Reference manager for cycle detection.
        /// </summary>
        private readonly fsCyclicReferenceManager _references;

        private readonly fsLazyCycleDefinitionWriter _lazyReferenceWriter;

        /// <summary>
        /// Allow the user to provide default storage types for interfaces and abstract
        /// classes. For example, a model could have IList{int} as a parameter, but the
        /// serialization data does not specify a List{int} type. A IList{} -> List{}
        /// remapping will cause List{} to be used as the default storage type. see
        /// https://github.com/jacobdufault/fullserializer/issues/120 for additional
        /// context.
        /// </summary>
        private readonly Dictionary<Type, Type> _abstractTypeRemap;

        // of _cachedConverters, but there is a semantic difference because
        // _cachedConverters goes
        /// <summary>
        /// Converter type to converter instance lookup table. This
        /// could likely be stored inside
        /// from serialized type to converter.
        /// </summary>
        private Dictionary<Type, fsBaseConverter> _cachedConverterTypeInstances;

        /// <summary>
        /// A cache from type to it's converter.
        /// </summary>
        private Dictionary<Type, fsBaseConverter> _cachedConverters;

        /// <summary>
        /// A cache from type to the set of processors that are interested in it.
        /// </summary>
        private Dictionary<Type, List<fsObjectProcessor>> _cachedProcessors;

        /// <summary>
        /// A context object that fsConverters can use to customize how they
        /// operate.
        /// </summary>
        public fsContext Context;

        /// <summary>
        /// Configuration options. Also see fsGlobalConfig.
        /// </summary>
        public fsConfig Config;

        private void RemapAbstractStorageTypeToDefaultType(ref Type storageType)
        {
            if ((storageType.Resolve().IsInterface || storageType.Resolve().IsAbstract) == false)
            {
                return;
            }

            if (storageType.Resolve().IsGenericType)
            {
                Type remappedGenericType;
                if (_abstractTypeRemap.TryGetValue(storageType.Resolve().GetGenericTypeDefinition(), out remappedGenericType))
                {
                    var genericArguments = storageType.GetGenericArguments();
                    storageType = remappedGenericType.Resolve().MakeGenericType(genericArguments);
                }
            }
            else
            {
                Type remappedType;
                if (_abstractTypeRemap.TryGetValue(storageType, out remappedType))
                {
                    storageType = remappedType;
                }
            }
        }

        /// <summary>
        /// Add a new processor to the serializer. Multiple processors can run at
        /// the same time in the same order they were added in.
        /// </summary>
        /// <param name="processor">The processor to add.</param>
        public void AddProcessor(fsObjectProcessor processor)
        {
            _processors.Add(processor);

            // We need to reset our cached processor set, as it could be invalid
            // with the new processor. Ideally, _cachedProcessors should be empty
            // (as the user should fully setup the serializer before actually
            // using it), but there is no guarantee.
            _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
        }

        /// <summary>
        /// Remove all processors which derive from TProcessor.
        /// </summary>
        public void RemoveProcessor<TProcessor>()
        {
            var i = 0;
            while (i < _processors.Count)
            {
                if (_processors[i] is TProcessor)
                {
                    _processors.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }

            // We need to reset our cached processor set, as it could be invalid
            // with the new processor. Ideally, _cachedProcessors should be empty
            // (as the user should fully setup the serializer before actually
            // using it), but there is no guarantee.
            _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
        }

        /// <summary>
        /// Provide a default storage type for the given abstract or interface type. If
        /// a type is deserialized which contains an interface/abstract field type and a
        /// mapping is provided, the mapped type will be used by default. For example,
        /// IList{T} => List{T} or IDictionary{TKey, TValue} => Dictionary{TKey, TValue}.
        /// </summary>
        public void SetDefaultStorageType(Type abstractType, Type defaultStorageType)
        {
            if ((abstractType.Resolve().IsInterface || abstractType.Resolve().IsAbstract) == false)
            {
                throw new ArgumentException("|abstractType| must be an interface or abstract type");
            }
            _abstractTypeRemap[abstractType] = defaultStorageType;
        }

        /// <summary>
        /// Fetches all of the processors for the given type.
        /// </summary>
        private List<fsObjectProcessor> GetProcessors(Type type)
        {
            List<fsObjectProcessor> processors;

            // Check to see if the user has defined a custom processor for the
            // type. If they have, then we don't need to scan through all of the
            // processor to check which one can process the type; instead, we
            // directly use the specified processor.
            var attr = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
            if (attr != null && attr.Processor != null)
            {
                var processor = (fsObjectProcessor)Activator.CreateInstance(attr.Processor);
                processors = new List<fsObjectProcessor>();
                processors.Add(processor);
                _cachedProcessors[type] = processors;
            }
            else if (_cachedProcessors.TryGetValue(type, out processors) == false)
            {
                processors = new List<fsObjectProcessor>();

                for (var i = 0; i < _processors.Count; ++i)
                {
                    var processor = _processors[i];
                    if (processor.CanProcess(type))
                    {
                        processors.Add(processor);
                    }
                }

                _cachedProcessors[type] = processors;
            }

            return processors;
        }

        /// <summary>
        /// Adds a new converter that can be used to customize how an object is
        /// serialized and deserialized.
        /// </summary>
        public void AddConverter(fsBaseConverter converter)
        {
            if (converter.Serializer != null)
            {
                throw new InvalidOperationException("Cannot add a single converter instance to " +
                    "multiple fsConverters -- please construct a new instance for " + converter);
            }

            // TODO: wrap inside of a ConverterManager so we can control
            //       _converters and _cachedConverters lifetime
            if (converter is fsDirectConverter)
            {
                var directConverter = (fsDirectConverter)converter;
                _availableDirectConverters[directConverter.ModelType] = directConverter;
            }
            else if (converter is fsConverter)
            {
                _availableConverters.Insert(0, (fsConverter)converter);
            }
            else
            {
                throw new InvalidOperationException("Unable to add converter " + converter +
                    "; the type association strategy is unknown. Please use either " +
                    "fsDirectConverter or fsConverter as your base type.");
            }

            converter.Serializer = this;

            // We need to reset our cached converter set, as it could be invalid
            // with the new converter. Ideally, _cachedConverters should be empty
            // (as the user should fully setup the serializer before actually
            // using it), but there is no guarantee.
            _cachedConverters = new Dictionary<Type, fsBaseConverter>();
        }

        /// <summary>
        /// Fetches a converter that can serialize/deserialize the given type.
        /// </summary>
        private fsBaseConverter GetConverter(Type type, Type overrideConverterType)
        {
            // Use an override converter type instead if that's what the user has
            // requested.
            if (overrideConverterType != null)
            {
                fsBaseConverter overrideConverter;
                if (_cachedConverterTypeInstances.TryGetValue(overrideConverterType, out overrideConverter) == false)
                {
                    overrideConverter = (fsBaseConverter)Activator.CreateInstance(overrideConverterType);
                    overrideConverter.Serializer = this;
                    _cachedConverterTypeInstances[overrideConverterType] = overrideConverter;
                }

                return overrideConverter;
            }

            // Try to lookup an existing converter.
            fsBaseConverter converter;
            if (_cachedConverters.TryGetValue(type, out converter))
            {
                return converter;
            }

            // Check to see if the user has defined a custom converter for the
            // type. If they have, then we don't need to scan through all of the
            // converters to check which one can process the type; instead, we
            // directly use the specified converter.
            {
                var attr = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
                if (attr != null && attr.Converter != null)
                {
                    converter = (fsBaseConverter)Activator.CreateInstance(attr.Converter);
                    converter.Serializer = this;
                    return _cachedConverters[type] = converter;
                }
            }

            // Check for a [fsForward] attribute.
            {
                var attr = fsPortableReflection.GetAttribute<fsForwardAttribute>(type);
                if (attr != null)
                {
                    converter = new fsForwardConverter(attr);
                    converter.Serializer = this;
                    return _cachedConverters[type] = converter;
                }
            }

            // There is no specific converter specified; try all of the general
            // ones to see which ones matches.
            if (_cachedConverters.TryGetValue(type, out converter) == false)
            {
                if (_availableDirectConverters.ContainsKey(type))
                {
                    converter = _availableDirectConverters[type];
                    return _cachedConverters[type] = converter;
                }
                else
                {
                    for (var i = 0; i < _availableConverters.Count; ++i)
                    {
                        if (_availableConverters[i].CanProcess(type))
                        {
                            converter = _availableConverters[i];
                            return _cachedConverters[type] = converter;
                        }
                    }
                }
            }

            throw new InvalidOperationException("Internal error -- could not find a converter for " + type);
        }

        /// <summary>
        /// Helper method that simply forwards the call to
        /// TrySerialize(typeof(T), instance, out data);
        /// </summary>
        public fsResult TrySerialize<T>(T instance, out fsData data)
        {
            return TrySerialize(typeof(T), instance, out data);
        }

        /// <summary>
        /// Generic wrapper around TryDeserialize that simply forwards the call.
        /// </summary>
        public fsResult TryDeserialize<T>(fsData data, ref T instance)
        {
            object boxed = instance;
            var fail = TryDeserialize(data, typeof(T), ref boxed);
            if (fail.Succeeded)
            {
                instance = (T)boxed;
            }
            return fail;
        }

        /// <summary>
        /// Serialize the given value.
        /// </summary>
        /// <param name="storageType">
        /// The type of field/property that stores the object instance. This is
        /// important particularly for inheritance, as a field storing an
        /// IInterface instance should have type information included.
        /// </param>
        /// <param name="instance">
        /// The actual object instance to serialize.
        /// </param>
        /// <param name="data">The serialized state of the object.</param>
        /// <returns>If serialization was successful.</returns>
        public fsResult TrySerialize(Type storageType, object instance, out fsData data)
        {
            return TrySerialize(storageType, null, instance, out data);
        }

        /// <summary>
        /// Serialize the given value.
        /// </summary>
        /// <param name="storageType">
        /// The type of field/property that stores the object instance. This is
        /// important particularly for inheritance, as a field storing an
        /// IInterface instance should have type information included.
        /// </param>
        /// <param name="overrideConverterType">
        /// An fsBaseConverter derived type that will be used to serialize the
        /// object instead of the converter found via the normal discovery
        /// mechanisms.
        /// </param>
        /// <param name="instance">
        /// The actual object instance to serialize.
        /// </param>
        /// <param name="data">The serialized state of the object.</param>
        /// <returns>If serialization was successful.</returns>
        public fsResult TrySerialize(Type storageType, Type overrideConverterType, object instance, out fsData data)
        {
            var processors = GetProcessors(instance == null ? storageType : instance.GetType());

            // LAZLO / LUDIQ FIX
            try
            {
                Invoke_OnBeforeSerialize(processors, storageType, instance);
            }
            catch (Exception ex)
            {
                data = new fsData();
                return fsResult.Fail(ex.ToString());
            }

            // We always serialize null directly as null
            if (ReferenceEquals(instance, null))
            {
                data = new fsData();
                Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
                return fsResult.Success;
            }

            var result = InternalSerialize_1_ProcessCycles(storageType, overrideConverterType, instance, out data);

            try
            {
                Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
            }
            catch (Exception ex)
            {
                result += fsResult.Fail(ex.ToString());
            }
            return result;
        }

        private fsResult InternalSerialize_1_ProcessCycles(Type storageType, Type overrideConverterType, object instance, out fsData data)
        {
            // We have an object definition to serialize.
            try
            {
                // Note that we enter the reference group at the beginning of
                // serialization so that we support references that are at equal
                // serialization levels, not just nested serialization levels,
                // within the given subobject. A prime example is serialization a
                // list of references.
                _references.Enter();

                // This type does not need cycle support.
                var converter = GetConverter(instance.GetType(), overrideConverterType);
                if (converter.RequestCycleSupport(instance.GetType()) == false)
                {
                    return InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
                }

                // We've already serialized this object instance (or it is
                // pending higher up on the call stack). Just serialize a
                // reference to it to escape the cycle.
                //
                // note: We serialize the int as a string to so that we don't
                //       lose any information in a conversion to/from double.
                if (_references.IsReference(instance))
                {
                    data = fsData.CreateDictionary();
                    _lazyReferenceWriter.WriteReference(_references.GetReferenceId(instance), data.AsDictionary);
                    return fsResult.Success;
                }

                // Mark inside the object graph that we've serialized the
                // instance. We do this *before* serialization so that if we get
                // back into this function recursively, it'll already be marked
                // and we can handle the cycle properly without going into an
                // infinite loop.
                _references.MarkSerialized(instance);

                // We've created the cycle metadata, so we can now serialize the
                // actual object. InternalSerialize will handle inheritance
                // correctly for us.
                var result = InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
                if (result.Failed)
                {
                    return result;
                }

                _lazyReferenceWriter.WriteDefinition(_references.GetReferenceId(instance), data);

                return result;
            }
            finally
            {
                if (_references.Exit())
                {
                    _lazyReferenceWriter.Clear();
                }
            }
        }

        private fsResult InternalSerialize_2_Inheritance(Type storageType, Type overrideConverterType, object instance, out fsData data)
        {
            // Serialize the actual object with the field type being the same as
            // the object type so that we won't go into an infinite loop.
            var serializeResult = InternalSerialize_3_ProcessVersioning(overrideConverterType, instance, out data);
            if (serializeResult.Failed)
            {
                return serializeResult;
            }

            // Do we need to add type information? If the field type and the
            // instance type are different then we will not be able to recover
            // the correct instance type from the field type when we deserialize
            // the object.
            //
            // Note: We allow converters to request that we do *not* add type
            //       information.
            if (storageType != instance.GetType() &&
                GetConverter(storageType, overrideConverterType).RequestInheritanceSupport(storageType))
            {
                var instanceType = instance.GetType();

                // LAZLO / LUDIQ
                // We need to loosen the instance type hint of Unity objects when serializing
                // to counter a very specific issue that happens when:
                //  - We serialize a reference to an object of an editor type
                //  - That object type inherits a runtime type
                //  - The field supports the runtime type, but will allow the editor type
                //  - Therefore serializing as the editor type is valid, but will fail to deserialize in builds
                // The only current example of this bug is AudioMixers and AudioMixerGroups.
                // UnityEDITOR.Audio.AudioMixerController extends UnityENGINE.Audio.AudioMixer.
                // UnityEDITOR.Audio.AudioMixerGroupController extends UnityENGINE.Audio.AudioMixerGroup.
                // Therefore, if we serialize a type hint to the editor controller, e.g. AudioMixerGroupController,
                // builds will fail to deserialize the type hint, even though they don't actually need it
                // to properly fetch the Unity Object reference, because it's provided directly by the converter.
                // We must instead serialize a type hint to the runtime, non-controller type, e.g. AudioMixer.
                // However, when loosening our type, we must make sure not to go past the compatibility
                // with the defined storage type, because if we did, we would get the "Ignoring type specifier"
                // error defined below, as the instance type hint wouldn't be assignable to the storage type on deserialization.
                // Likewise, we must make sure not to go above UnityObject itself, because we need that much hinting
                // for FullSerializer to know that the proper converter to be used is our custom UnityObjectConverter.
                // See: https://support.ludiq.io/communities/5/topics/1032-audio-mixer-reference-gets-nulled-on-il2cpp-builds
                if (instance is UnityObject)
                {
                    var looseType = instanceType;

                    do
                    {
                        instanceType = looseType;
                        looseType = looseType.BaseType;
                    }
                    while (looseType != null && instanceType != typeof(UnityObject) && storageType.IsAssignableFrom(looseType));

                    // Debug.Log($"Loosened instance type hint for {instance.GetType()} stored as {storageType} to {instanceType}");
                }

                // Add the inheritance metadata
                EnsureDictionary(data);
                data.AsDictionary[Key_InstanceType] = new fsData(RuntimeCodebase.SerializeType(instanceType));
            }

            return serializeResult;
        }

        private fsResult InternalSerialize_3_ProcessVersioning(Type overrideConverterType, object instance, out fsData data)
        {
            // note: We do not have to take a Type parameter here, since at this
            //       point in the serialization algorithm inheritance has
            // *always* been handled. If we took a type parameter, it will
            // *always* be equal to instance.GetType(), so why bother taking the
            //  parameter?

            // Check to see if there is versioning information for this type. If
            // so, then we need to serialize it.
            var optionalVersionedType = fsVersionManager.GetVersionedType(instance.GetType());
            if (optionalVersionedType.HasValue)
            {
                var versionedType = optionalVersionedType.Value;

                // Serialize the actual object content; we'll just wrap it with
                // versioning metadata here.
                var result = InternalSerialize_4_Converter(overrideConverterType, instance, out data);
                if (result.Failed)
                {
                    return result;
                }

                // Add the versioning information
                EnsureDictionary(data);
                data.AsDictionary[Key_Version] = new fsData(versionedType.VersionString);

                return result;
            }

            // This type has no versioning information -- directly serialize it
            // using the selected converter.
            return InternalSerialize_4_Converter(overrideConverterType, instance, out data);
        }

        private fsResult InternalSerialize_4_Converter(Type overrideConverterType, object instance, out fsData data)
        {
            var instanceType = instance.GetType();
            return GetConverter(instanceType, overrideConverterType).TrySerialize(instance, out data, instanceType);
        }

        /// <summary>
        /// Attempts to deserialize a value from a serialized state.
        /// </summary>
        public fsResult TryDeserialize(fsData data, Type storageType, ref object result)
        {
            return TryDeserialize(data, storageType, null, ref result);
        }

        /// <summary>
        /// Attempts to deserialize a value from a serialized state.
        /// </summary>
        public fsResult TryDeserialize(fsData data, Type storageType, Type overrideConverterType, ref object result)
        {
            if (data.IsNull)
            {
                result = null;
                var processors = GetProcessors(storageType);
                Invoke_OnBeforeDeserialize(processors, storageType, ref data);
                Invoke_OnAfterDeserialize(processors, storageType, null);
                return fsResult.Success;
            }

            // Convert legacy data into modern style data
            ConvertLegacyData(ref data);

            try
            {
                // We wrap the entire deserialize call in a reference group so
                // that we can properly deserialize a "parallel" set of
                // references, ie, a list of objects that are cyclic w.r.t. the
                // list
                _references.Enter();

                List<fsObjectProcessor> processors;
                var r = InternalDeserialize_1_CycleReference(overrideConverterType, data, storageType, ref result, out processors);
                if (r.Succeeded)
                {
                    // LAZLO / LUDIQ FIX
                    // Converts exceptions thrown in OAD to a FS-understandable failure.
                    // Extremely useful in collections, this will allow fsIEnumerableConverter
                    // to continue (as it should) even if an item threw an exception
                    try
                    {
                        Invoke_OnAfterDeserialize(processors, storageType, result);
                    }
                    catch (Exception ex)
                    {
                        r += fsResult.Fail(ex.ToString());
                    }
                }
                return r;
            }
            finally
            {
                _references.Exit();
            }
        }

        private fsResult InternalDeserialize_1_CycleReference(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
        {
            // We handle object references first because we could be
            // deserializing a cyclic type that is inherited. If that is the
            // case, then if we handle references after inheritances we will try
            // to create an object instance for an abstract/interface type.

            // While object construction should technically be two-pass, we can
            // do it in one pass because of how serialization happens. We
            // traverse the serialization graph in the same order during
            // serialization and deserialization, so the first time we encounter
            // an object it'll always be the definition. Any times after that it
            // will be a reference. Because of this, if we encounter a reference
            // then we will have *always* already encountered the definition for
            // it.
            if (IsObjectReference(data))
            {
                var refId = int.Parse(data.AsDictionary[Key_ObjectReference].AsString);
                result = _references.GetReferenceObject(refId);
                processors = GetProcessors(result.GetType());
                return fsResult.Success;
            }

            return InternalDeserialize_2_Version(overrideConverterType, data, storageType, ref result, out processors);
        }

        private fsResult InternalDeserialize_2_Version(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
        {
            if (IsVersioned(data))
            {
                // data is versioned, but we might not need to do a migration
                var version = data.AsDictionary[Key_Version].AsString;

                var versionedType = fsVersionManager.GetVersionedType(storageType);
                if (versionedType.HasValue &&
                    versionedType.Value.VersionString != version)
                {
                    // we have to do a migration
                    var deserializeResult = fsResult.Success;

                    List<fsVersionedType> path;
                    deserializeResult += fsVersionManager.GetVersionImportPath(version, versionedType.Value, out path);
                    if (deserializeResult.Failed)
                    {
                        processors = GetProcessors(storageType);
                        return deserializeResult;
                    }

                    // deserialize as the original type
                    deserializeResult += InternalDeserialize_3_Inheritance(overrideConverterType, data, path[0].ModelType, ref result, out processors);
                    if (deserializeResult.Failed)
                    {
                        return deserializeResult;
                    }

                    // TODO: we probably should be invoking object processors all
                    //       along this pipeline
                    for (var i = 1; i < path.Count; ++i)
                    {
                        result = path[i].Migrate(result);
                    }

                    // Our data contained an object definition ($id) that was
                    // added to _references in step 4. However, in case we are
                    // doing versioning, it will contain the old version. To make
                    // sure future references to this object end up referencing
                    // the migrated version, we must update the reference.
                    if (IsObjectDefinition(data))
                    {
                        var sourceId = int.Parse(data.AsDictionary[Key_ObjectDefinition].AsString);
                        _references.AddReferenceWithId(sourceId, result);
                    }

                    processors = GetProcessors(deserializeResult.GetType());
                    return deserializeResult;
                }
            }

            return InternalDeserialize_3_Inheritance(overrideConverterType, data, storageType, ref result, out processors);
        }

        private fsResult InternalDeserialize_3_Inheritance(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
        {
            var deserializeResult = fsResult.Success;

            var objectType = storageType;

            // If the serialized state contains type information, then we need to
            // make sure to update our objectType and data to the proper values
            // so that when we construct an object instance later and run
            // deserialization we run it on the proper type.
            if (IsTypeSpecified(data))
            {
                objectType = GetDataType(ref data, storageType, ref deserializeResult);
            }
            RemapAbstractStorageTypeToDefaultType(ref objectType);

            // We wait until here to actually Invoke_OnBeforeDeserialize because
            // we do not have the correct set of processors to invoke until
            // *after* we have resolved the proper type to use for
            // deserialization.
            processors = GetProcessors(objectType);

            if (deserializeResult.Failed)
            {
                return deserializeResult;
            }

            // LAZLO / LUDIQ FIX
            try
            {
                Invoke_OnBeforeDeserialize(processors, storageType, ref data);
            }
            catch (Exception ex)
            {
                deserializeResult += fsResult.Fail(ex.ToString());
                return deserializeResult;
            }

            // Construct an object instance if we don't have one already. We also
            // need to construct an instance if the result type is of the wrong
            // type, which may be the case when we have a versioned import graph.
            if (ReferenceEquals(result, null) || result.GetType() != objectType)
            {
                result = GetConverter(objectType, overrideConverterType).CreateInstance(data, objectType);
            }

            // We call OnBeforeDeserializeAfterInstanceCreation here because we
            // still want to invoke the method even if the user passed in an
            // existing instance.
            try
            {
                Invoke_OnBeforeDeserializeAfterInstanceCreation(processors, storageType, result, ref data);
            }
            catch (Exception ex)
            {
                deserializeResult += fsResult.Fail(ex.ToString());
                return deserializeResult;
            }

            // NOTE: It is critically important that we pass the actual
            //       objectType down instead of using result.GetType() because it
            //       is not guaranteed that result.GetType() will equal
            //       objectType, especially because some converters are known to
            //       return dummy values for CreateInstance() (for example, the
            //       default behavior for structs is to just return the type of
            //       the struct).

            deserializeResult += InternalDeserialize_4_Cycles(overrideConverterType, data, objectType, ref result);
            return deserializeResult;
        }

        private fsResult InternalDeserialize_4_Cycles(Type overrideConverterType, fsData data, Type resultType, ref object result)
        {
            if (IsObjectDefinition(data))
            {
                // NOTE: object references are handled at stage 1

                // If this is a definition, then we have a serialization
                // invariant that this is the first time we have encountered the
                // object (TODO: verify in the deserialization logic)

                // Since at this stage in the deserialization process we already
                // have access to the object instance, so we just need to sync
                // the object id to the references database so that when we
                // encounter the instance we lookup this same object. We want to
                // do this before actually deserializing the object because when
                // deserializing the object there may be references to itself.

                var sourceId = int.Parse(data.AsDictionary[Key_ObjectDefinition].AsString);
                _references.AddReferenceWithId(sourceId, result);
            }

            // Nothing special, go through the standard deserialization logic.
            return InternalDeserialize_5_Converter(overrideConverterType, data, resultType, ref result);
        }

        private fsResult InternalDeserialize_5_Converter(Type overrideConverterType, fsData data, Type resultType, ref object result)
        {
            if (IsWrappedData(data))
            {
                data = data.AsDictionary[Key_Content];
            }

            return GetConverter(resultType, overrideConverterType).TryDeserialize(data, ref result, resultType);
        }


        // This method only really exists to make the InternalDeserialize_3_Inheritance() method cleaner.
        private static Type GetDataType(ref fsData data, Type defaultType, ref fsResult deserializeResult)
        {
            var objectType = defaultType;
            var dict = data.AsDictionary;

            var typeNameData = dict[Key_InstanceType];
            if (typeNameData.IsString == false)
            {
                deserializeResult.AddMessage(Key_InstanceType + " value must be a string (in " + data + ")");
                return objectType;
            }

            var typeName = typeNameData.AsString;

            if (!RuntimeCodebase.TryDeserializeType(typeName, out var markedType))
            {
                if (IsVisualScriptingUnit(data))
                {
                    //We store a copy of the node as a string in the hopes of being able to re-instantiate it later.
                    dict[Key_UnitFormerValue] = new fsData(data.ToString());

                    // We store the type that the unit should be, we will try to re-instantiate it if it becomes available again.
                    dict[Key_UnitFormerType] = typeNameData;
                    dict[Key_InstanceType] = new fsData(TypeName_MissingType);

                    // TODO: Ideally this would display as an error in the console instead of a warning. Using fsResult.Fail() aborts the deserialization.
                    deserializeResult += fsResult.Warn($"Type definition for '{typeName}' is missing.\nConverted '{typeName}' unit to '{TypeName_MissingType}'. Did you delete the type's script file?");

                    return Type_MissingType;
                }

                // This message is redundant if the above warning gets logged.
                deserializeResult += fsResult.Warn("Unable to find type: \"" + typeName + "\"");

                return objectType;
            }
            // Check if the former type of the MissingType unit is defined again (if the user added back the unit script).
            else if (typeName == TypeName_MissingType)
            {
                if (dict.ContainsKey(Key_UnitFormerType) && IsVisualScriptingUnit(data))
                {
                    var formerTypeName = dict[Key_UnitFormerType].AsString;
                    if (RuntimeCodebase.TryDeserializeType(formerTypeName, out var formerType))
                    {
                        // If the user tries to create a new script for the type, we must ensure that it derives from VS units so that it can be properly deserialized.
                        if (defaultType.IsAssignableFrom(formerType))
                        {
                            //TODO: Add checks if the json can't be parsed to a unit.
                            if (dict.ContainsKey(Key_UnitFormerValue))
                            {
                                // The node may have been moved while in dummy form.
                                fsData newPosition = dict[Key_UnitPosition];

                                data = fsJsonParser.Parse(dict[Key_UnitFormerValue].AsString);
                                dict = data.AsDictionary; // 'dict' and 'data' are meant to represent the same object.

                                dict[Key_UnitPosition] = newPosition;

                                deserializeResult += fsResult.Warn($"Missing unit type '{formerTypeName}' was found.\nConverted '{TypeName_MissingType}' unit back to '{formerTypeName}'");
                            }
                            else
                            {
                                // We want to restore the unit to its correct type.
                                dict[Key_InstanceType] = new fsData(formerTypeName);
                                deserializeResult += fsResult.Warn($"Missing unit type '{formerTypeName}' was found.\nConverted '{TypeName_MissingType}' unit back to '{formerTypeName}'\nNo former state can be found. Reverting node to defaults.\n" + data);
                            }

                            objectType = formerType;
                            return objectType;
                        }

                        // TODO: Ideally this would display as an error in the console instead of a warning. Using fsResult.Fail() aborts the deserialization.
                        deserializeResult += fsResult.Warn($"Missing unit type '{formerTypeName}' was found, but is not assignable to '{defaultType.FullName}'. Did you forget to inherit from '{TypeName_Unit}'?");
                    }
                    else
                    {
                        // TODO: Ideally this would display as an error in the console instead of a warning. Using fsResult.Fail() aborts the deserialization.
                        deserializeResult += fsResult.Warn($"Type definition for '{formerTypeName}' unit is missing. Did you remove its script file?");
                    }
                }
                else
                {
                    deserializeResult += fsResult.Warn($"Serialized '{TypeName_MissingType}' unit has an unrecognized format.");
                }
            }

            if (defaultType.IsAssignableFrom(markedType) == false)
            {
                // It's possible that the user removes their custom node's inheritance from the VS unit type.
                if (IsVisualScriptingUnit(data))
                {
                    // We store the type that the unit should be, we will try to re-instantiate it if it becomes valid again.
                    dict[Key_UnitFormerType] = typeNameData;
                    dict[Key_InstanceType] = new fsData(TypeName_MissingType);

                    // TODO: Ideally this would display as an error in the console instead of a warning. Using fsResult.Fail() aborts the deserialization.
                    deserializeResult += fsResult.Warn($"Type '{typeName}' is no longer assignable to '{defaultType.FullName}'. Did you remove inheritance from '{TypeName_Unit}'?\nConverted '{typeName}' unit to '{TypeName_MissingType}'.");

                    return Type_MissingType;
                }

                deserializeResult.AddMessage("Ignoring type specifier; a field/property of type " + defaultType + " cannot hold an instance of " + markedType);
                return objectType;
            }

            objectType = markedType;
            return objectType;
        }

        /// <summary>
        /// Ensures that the data is a dictionary. If it is not, then it is
        /// wrapped inside of one.
        /// </summary>
        private static void EnsureDictionary(fsData data)
        {
            if (data.IsDictionary == false)
            {
                var existingData = data.Clone();
                data.BecomeDictionary();
                data.AsDictionary[Key_Content] = existingData;
            }
        }

        /// <summary>
        /// This manages instance writing so that we do not write unnecessary $id
        /// fields. We only need to write out an $id field when there is a
        /// corresponding $ref field. This is able to write $id references lazily
        /// because the fsData instance is not actually written out to text until
        /// we have entirely finished serializing it.
        /// </summary>
        internal class fsLazyCycleDefinitionWriter
        {
            private Dictionary<int, fsData> _pendingDefinitions = new Dictionary<int, fsData>();
            private HashSet<int> _references = new HashSet<int>();

            public void WriteDefinition(int id, fsData data)
            {
                if (_references.Contains(id))
                {
                    EnsureDictionary(data);
                    data.AsDictionary[Key_ObjectDefinition] = new fsData(id.ToString());
                }
                else
                {
                    _pendingDefinitions[id] = data;
                }
            }

            public void WriteReference(int id, Dictionary<string, fsData> dict)
            {
                // Write the actual definition if necessary
                if (_pendingDefinitions.ContainsKey(id))
                {
                    var data = _pendingDefinitions[id];
                    EnsureDictionary(data);
                    data.AsDictionary[Key_ObjectDefinition] = new fsData(id.ToString());
                    _pendingDefinitions.Remove(id);
                }
                else
                {
                    _references.Add(id);
                }

                // Write the reference
                dict[Key_ObjectReference] = new fsData(id.ToString());
            }

            public void Clear()
            {
                _pendingDefinitions.Clear();
                _references.Clear();
            }
        }

        #region Keys

        private static HashSet<string> _reservedKeywords;

        static fsSerializer()
        {
            _reservedKeywords = new HashSet<string>
            {
                Key_ObjectReference,
                Key_ObjectDefinition,
                Key_InstanceType,
                Key_Version,
                Key_Content
            };
        }

        /// <summary>
        /// Returns true if the given key is a special keyword that full
        /// serializer uses to add additional metadata on top of the emitted
        /// JSON.
        /// </summary>
        public static bool IsReservedKeyword(string key)
        {
            return _reservedKeywords.Contains(key);
        }

        /// <summary>
        /// This is an object reference in part of a cyclic graph.
        /// </summary>
        private static readonly string Key_ObjectReference = $"{fsGlobalConfig.InternalFieldPrefix}ref";

        /// <summary>
        /// This is an object definition, as part of a cyclic graph.
        /// </summary>
        private static readonly string Key_ObjectDefinition = $"{fsGlobalConfig.InternalFieldPrefix}id";

        /// <summary>
        /// This specifies the actual type of an object (the instance type was
        /// different from the field type).
        /// </summary>
        private static readonly string Key_InstanceType = $"{fsGlobalConfig.InternalFieldPrefix}type";

        /// <summary>
        /// The version string for the serialized data.
        /// </summary>
        private static readonly string Key_Version = $"{fsGlobalConfig.InternalFieldPrefix}version";

        /// <summary>
        /// If we have to add metadata but the original serialized state was not
        /// a dictionary, then this will contain the original data.
        /// </summary>
        private static readonly string Key_Content = $"{fsGlobalConfig.InternalFieldPrefix}content";


        // THESE KEYS ARE SPECIFIC TO VISUAL SCRIPTING UNIT DESERIALIZATION
        // It's important to note here that VS-specific fields should not get the internal FullSerializer field prefix when serialized to avoid ambiguity.

        /// <summary>
        /// Dictionary for the default values of each of the unit's value port.
        /// </summary>
        internal static readonly string Key_UnitDefault = "defaultValues";

        /// <summary>
        /// Specifies the position of a unit in its respective graph.
        /// </summary>
        internal static readonly string Key_UnitPosition = "position";

        /// <summary>
        /// Unique guid of the unit instance.
        /// </summary>
        internal static readonly string Key_UnitGuid = "guid";

        /// <summary>
        /// Meant for 'missing type' units to remember the type of node they are supposed to be.
        /// This will allow us to try and convert back if the correct type becomes available again.
        /// </summary>
        internal static readonly string Key_UnitFormerType = "formerType";

        /// <summary>
        /// Meant for 'missing type' units to remember the state of the node they are supposed to be.
        /// This will allow to preserve the serialized instance values of the node when its type was lost.
        /// </summary>
        internal static readonly string Key_UnitFormerValue = "formerValue";

        /// <summary>
        /// Hard-coded fully qualified name of the 'unit' type.
        /// </summary>
        internal static readonly string TypeName_Unit = "Unity.VisualScripting.Unit";

        /// <summary>
        /// The 'Unit' type defined in VS.
        /// Made static as to only be deserialized once and potentially used multiple times.
        /// </summary>
        static readonly Type Type_Unit = RuntimeCodebase.DeserializeType(TypeName_Unit);

        /// <summary>
        /// Hard-coded fully qualified name of the 'missing type unit' type.
        /// </summary>
        internal static readonly string TypeName_MissingType = "Unity.VisualScripting.MissingType";

        /// <summary>
        /// The 'MissingType' type defined in VS.
        /// Made static as to only be deserialized once and potentially used multiple times.
        /// </summary>
        static readonly Type Type_MissingType = RuntimeCodebase.DeserializeType(TypeName_MissingType);


        private static bool IsObjectReference(fsData data)
        {
            if (data.IsDictionary == false)
            {
                return false;
            }
            return data.AsDictionary.ContainsKey(Key_ObjectReference);
        }

        private static bool IsObjectDefinition(fsData data)
        {
            if (data.IsDictionary == false)
            {
                return false;
            }
            return data.AsDictionary.ContainsKey(Key_ObjectDefinition);
        }

        private static bool IsVersioned(fsData data)
        {
            if (data.IsDictionary == false)
            {
                return false;
            }
            return data.AsDictionary.ContainsKey(Key_Version);
        }

        private static bool IsTypeSpecified(fsData data)
        {
            if (data.IsDictionary == false)
            {
                return false;
            }
            return data.AsDictionary.ContainsKey(Key_InstanceType);
        }

        private static bool IsWrappedData(fsData data)
        {
            if (data.IsDictionary == false)
            {
                return false;
            }
            return data.AsDictionary.ContainsKey(Key_Content);
        }

        /// <summary>
        /// Makes a guess as to if the data represents a VS unit.
        ///
        /// If this turns out to be too general and objects that are not meant to be units get
        /// converted to MissingType units, consider adding a dedicated serialized field exclusive to units
        /// as to be able to check against it. (Something named like "isVisualScriptingUnit")
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsVisualScriptingUnit(fsData data)
        {
            if (data.IsDictionary == false)
            {
                return false;
            }

            var dictionary = data.AsDictionary;

            // If the following are true, we can make a reasonable
            // assumption that the dictionary represents a VS unit.
            return
                dictionary.ContainsKey(Key_UnitDefault) &&
                dictionary.ContainsKey(Key_UnitPosition) &&
                dictionary.ContainsKey(Key_UnitGuid) &&
                // Maybe don't hard-code these?
                dictionary[Key_UnitPosition].AsDictionary.ContainsKey("x") &&
                dictionary[Key_UnitPosition].AsDictionary.ContainsKey("y");
        }


        /// <summary>
        /// Strips all deserialization metadata from the object, like $type and
        /// $content fields.
        /// </summary>
        /// <remarks>
        /// After making this call, you will *not* be able to deserialize the
        /// same object instance. The metadata is strictly necessary for
        /// deserialization!
        /// </remarks>
        public static void StripDeserializationMetadata(ref fsData data)
        {
            if (data.IsDictionary && data.AsDictionary.ContainsKey(Key_Content))
            {
                data = data.AsDictionary[Key_Content];
            }

            if (data.IsDictionary)
            {
                var dict = data.AsDictionary;
                dict.Remove(Key_ObjectReference);
                dict.Remove(Key_ObjectDefinition);
                dict.Remove(Key_InstanceType);
                dict.Remove(Key_Version);
            }
        }

        /// <summary>
        /// This function converts legacy serialization data into the new format,
        /// so that the import process can be unified and ignore the old format.
        /// </summary>
        private static void ConvertLegacyData(ref fsData data)
        {
            if (data.IsDictionary == false)
            {
                return;
            }

            var dict = data.AsDictionary;

            // fast-exit: metadata never had more than two items
            if (dict.Count > 2)
            {
                return;
            }

            // Key strings used in the legacy system
            var referenceIdString = "ReferenceId";
            var sourceIdString = "SourceId";
            var sourceDataString = "Data";
            var typeString = "Type";
            var typeDataString = "Data";

            // type specifier
            if (dict.Count == 2 && dict.ContainsKey(typeString) && dict.ContainsKey(typeDataString))
            {
                data = dict[typeDataString];
                EnsureDictionary(data);
                ConvertLegacyData(ref data);

                data.AsDictionary[Key_InstanceType] = dict[typeString];
            }
            // object definition
            else if (dict.Count == 2 && dict.ContainsKey(sourceIdString) && dict.ContainsKey(sourceDataString))
            {
                data = dict[sourceDataString];
                EnsureDictionary(data);
                ConvertLegacyData(ref data);

                data.AsDictionary[Key_ObjectDefinition] = dict[sourceIdString];
            }
            // object reference
            else if (dict.Count == 1 && dict.ContainsKey(referenceIdString))
            {
                data = fsData.CreateDictionary();
                data.AsDictionary[Key_ObjectReference] = dict[referenceIdString];
            }
        }

        #endregion Keys

        #region Utility Methods

        private static void Invoke_OnBeforeSerialize(List<fsObjectProcessor> processors, Type storageType, object instance)
        {
            for (var i = 0; i < processors.Count; ++i)
            {
                processors[i].OnBeforeSerialize(storageType, instance);
            }
        }

        private static void Invoke_OnAfterSerialize(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data)
        {
            // We run the after calls in reverse order; this significantly
            // reduces the interaction burden between multiple processors - it
            // makes each one much more independent and ignorant of the other
            // ones.

            for (var i = processors.Count - 1; i >= 0; --i)
            {
                processors[i].OnAfterSerialize(storageType, instance, ref data);
            }
        }

        private static void Invoke_OnBeforeDeserialize(List<fsObjectProcessor> processors, Type storageType, ref fsData data)
        {
            for (var i = 0; i < processors.Count; ++i)
            {
                processors[i].OnBeforeDeserialize(storageType, ref data);
            }
        }

        private static void Invoke_OnBeforeDeserializeAfterInstanceCreation(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data)
        {
            for (var i = 0; i < processors.Count; ++i)
            {
                processors[i].OnBeforeDeserializeAfterInstanceCreation(storageType, instance, ref data);
            }
        }

        private static void Invoke_OnAfterDeserialize(List<fsObjectProcessor> processors, Type storageType, object instance)
        {
            for (var i = processors.Count - 1; i >= 0; --i)
            {
                processors[i].OnAfterDeserialize(storageType, instance);
            }
        }

        #endregion Utility Methods
    }
}
