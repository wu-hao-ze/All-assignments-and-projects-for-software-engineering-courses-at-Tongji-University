using System;
using System.Reflection;
using Unity.VisualScripting.FullSerializer.Internal;

namespace Unity.VisualScripting.FullSerializer
{
    /// <summary>
    /// A property or field on a MetaType. This unifies the FieldInfo and
    /// PropertyInfo classes.
    /// </summary>
    public class fsMetaProperty
    {
        internal fsMetaProperty(fsConfig config, FieldInfo field)
        {
            _memberInfo = field;
            StorageType = field.FieldType;
            MemberName = field.Name;
            IsPublic = field.IsPublic;
            IsReadOnly = field.IsInitOnly;
            CanRead = true;
            CanWrite = true;

            CommonInitialize(config);
        }

        internal fsMetaProperty(fsConfig config, PropertyInfo property)
        {
            _memberInfo = property;
            StorageType = property.PropertyType;
            MemberName = property.Name;
            IsPublic = property.GetGetMethod() != null && property.GetGetMethod().IsPublic && property.GetSetMethod() != null && property.GetSetMethod().IsPublic;
            IsReadOnly = false;
            CanRead = property.CanRead;
            CanWrite = property.CanWrite;

            CommonInitialize(config);
        }

        /// <summary>
        /// Internal handle to the reflected member.
        /// </summary>
        internal MemberInfo _memberInfo;

        /// <summary>
        /// The type of value that is stored inside of the property. For example,
        /// for an int field, StorageType will be typeof(int).
        /// </summary>
        public Type StorageType { get; private set; }

        /// <summary>
        /// A custom fsBaseConverter instance to use for this field/property, if
        /// requested. This will be null if the default converter selection
        /// algorithm should be used. This is specified using the [fsObject]
        /// annotation with the Converter field.
        /// </summary>
        public Type OverrideConverterType { get; private set; }

        /// <summary>
        /// Can this property be read?
        /// </summary>
        public bool CanRead { get; private set; }

        /// <summary>
        /// Can this property be written to?
        /// </summary>
        public bool CanWrite { get; private set; }

        /// <summary>
        /// The serialized name of the property, as it should appear in JSON.
        /// </summary>
        public string JsonName { get; private set; }

        /// <summary>
        /// The name of the actual member.
        /// </summary>
        public string MemberName { get; private set; }

        /// <summary>
        /// Is this member public?
        /// </summary>
        public bool IsPublic { get; private set; }

        /// <summary>
        /// Is this type readonly? We can modify readonly properties using
        /// reflection, but not using generated C#.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        private void CommonInitialize(fsConfig config)
        {
            var attr = fsPortableReflection.GetAttribute<fsPropertyAttribute>(_memberInfo);
            if (attr != null)
            {
                JsonName = attr.Name;
                OverrideConverterType = attr.Converter;
            }

            if (string.IsNullOrEmpty(JsonName))
            {
                JsonName = config.GetJsonNameFromMemberName(MemberName, _memberInfo);
            }
        }

        /// <summary>
        /// Writes a value to the property that this MetaProperty represents,
        /// using given object instance as the context.
        /// </summary>
        public void Write(object context, object value)
        {
            var field = _memberInfo as FieldInfo;
            var property = _memberInfo as PropertyInfo;

            if (field != null)
            {
                if (PlatformUtility.supportsJit)
                {
                    field.SetValueOptimized(context, value);
                }
                else
                {
                    field.SetValue(context, value);
                }
            }
            else if (property != null)
            {
                if (PlatformUtility.supportsJit)
                {
                    if (property.CanWrite)
                    {
                        property.SetValueOptimized(context, value);
                    }
                }
                else
                {
                    var setMethod = property.GetSetMethod(/*nonPublic:*/ true);
                    if (setMethod != null)
                    {
                        setMethod.Invoke(context, new[] { value });
                    }
                }
            }
        }

        /// <summary>
        /// Reads a value from the property that this MetaProperty represents,
        /// using the given object instance as the context.
        /// </summary>
        public object Read(object context)
        {
            if (_memberInfo is PropertyInfo)
            {
#if LUDIQ_OPTIMIZE
                return ((PropertyInfo)_memberInfo).GetValueOptimized(context);
#else
                return ((PropertyInfo)_memberInfo).GetValue(context, null); // Attempt fix-1588
#endif
            }
            else
            {
#if LUDIQ_OPTIMIZE
                return ((FieldInfo)_memberInfo).GetValueOptimized(context);
#else
                return ((FieldInfo)_memberInfo).GetValue(context);
#endif
            }
        }
    }
}
