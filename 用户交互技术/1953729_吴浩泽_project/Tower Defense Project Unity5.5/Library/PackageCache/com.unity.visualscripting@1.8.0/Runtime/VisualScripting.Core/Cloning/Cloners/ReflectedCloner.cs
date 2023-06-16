using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class ReflectedCloner : Cloner<object>
    {
        public override bool Handles(Type type)
        {
            return false; // Should only be used as a fallback cloner
        }

        public override void FillClone(Type type, ref object clone, object original, CloningContext context)
        {
            if (PlatformUtility.supportsJit)
            {
                foreach (var accessor in GetOptimizedAccessors(type))
                {
                    if (context.tryPreserveInstances)
                    {
                        var cloneProperty = accessor.GetValue(clone);
                        Cloning.CloneInto(context, ref cloneProperty, accessor.GetValue(original));
                        accessor.SetValue(clone, cloneProperty);
                    }
                    else
                    {
                        accessor.SetValue(clone, Cloning.Clone(context, accessor.GetValue(original)));
                    }
                }
            }
            else
            {
                foreach (var accessor in GetAccessors(type))
                {
                    if (accessor is FieldInfo)
                    {
                        var field = (FieldInfo)accessor;

                        if (context.tryPreserveInstances)
                        {
                            var cloneProperty = field.GetValue(clone);
                            Cloning.CloneInto(context, ref cloneProperty, field.GetValue(original));
                            field.SetValue(clone, cloneProperty);
                        }
                        else
                        {
                            field.SetValue(clone, Cloning.Clone(context, field.GetValue(original)));
                        }
                    }
                    else if (accessor is PropertyInfo)
                    {
                        var property = (PropertyInfo)accessor;

                        if (context.tryPreserveInstances)
                        {
                            var cloneProperty = property.GetValue(clone, null);
                            Cloning.CloneInto(context, ref cloneProperty, property.GetValue(original, null));
                            property.SetValue(clone, cloneProperty, null);
                        }
                        else
                        {
                            property.SetValue(clone, Cloning.Clone(context, property.GetValue(original, null)), null);
                        }
                    }
                }
            }
        }

        private readonly Dictionary<Type, MemberInfo[]> accessors = new Dictionary<Type, MemberInfo[]>();

        private MemberInfo[] GetAccessors(Type type)
        {
            if (!accessors.ContainsKey(type))
            {
                accessors.Add(type, GetMembers(type).ToArray());
            }

            return accessors[type];
        }

        private readonly Dictionary<Type, IOptimizedAccessor[]> optimizedAccessors = new Dictionary<Type, IOptimizedAccessor[]>();

        private IOptimizedAccessor[] GetOptimizedAccessors(Type type)
        {
            if (!optimizedAccessors.ContainsKey(type))
            {
                var list = new List<IOptimizedAccessor>();

                foreach (var member in GetMembers(type))
                {
                    if (member is FieldInfo)
                    {
                        list.Add(((FieldInfo)member).Prewarm());
                    }
                    else if (member is PropertyInfo)
                    {
                        list.Add(((PropertyInfo)member).Prewarm());
                    }
                }

                optimizedAccessors.Add(type, list.ToArray());
            }

            return optimizedAccessors[type];
        }

        protected virtual IEnumerable<MemberInfo> GetMembers(Type type)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            return LinqUtility.Concat<MemberInfo>
                (
                    type.GetFields(bindingFlags).Where(IncludeField),
                    type.GetProperties(bindingFlags).Where(IncludeProperty)
                );
        }

        protected virtual bool IncludeField(FieldInfo field)
        {
            return false;
        }

        protected virtual bool IncludeProperty(PropertyInfo property)
        {
            return false;
        }
    }
}
