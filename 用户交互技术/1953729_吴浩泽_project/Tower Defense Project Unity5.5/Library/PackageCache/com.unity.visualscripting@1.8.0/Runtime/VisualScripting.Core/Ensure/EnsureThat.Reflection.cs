using System;

namespace Unity.VisualScripting
{
    public partial class EnsureThat
    {
        public void HasAttribute(Type param, Type attributeType)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!param.HasAttribute(attributeType))
            {
                throw new ArgumentException(ExceptionMessages.Reflection_HasAttribute_Failed.Inject(param.ToString(), attributeType.ToString()), paramName);
            }
        }

        public void HasAttribute<TAttribute>(Type param) where TAttribute : Attribute => HasAttribute(param, typeof(TAttribute));

        private void HasConstructorAccepting(Type param, Type[] parameterTypes, bool nonPublic)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (param.GetConstructorAccepting(parameterTypes, nonPublic) == null)
            {
                var message = nonPublic ? ExceptionMessages.Reflection_HasConstructor_Failed : ExceptionMessages.Reflection_HasPublicConstructor_Failed;

                throw new ArgumentException(message.Inject(param.ToString(), parameterTypes.ToCommaSeparatedString()), paramName);
            }
        }

        public void HasConstructorAccepting(Type param, params Type[] parameterTypes) => HasConstructorAccepting(param, parameterTypes, true);

        public void HasPublicConstructorAccepting(Type param, params Type[] parameterTypes) => HasConstructorAccepting(param, parameterTypes, false);
    }
}
