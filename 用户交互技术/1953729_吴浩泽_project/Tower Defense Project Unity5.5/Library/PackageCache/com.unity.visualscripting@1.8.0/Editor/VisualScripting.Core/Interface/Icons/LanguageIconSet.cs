using System;

namespace Unity.VisualScripting
{
    public class LanguageIconSet
    {
        public LanguageIconSet(string name)
        {
            @public = LoadAccessibility(name, false);
            @private = LoadAccessibility(name + "_Private", false) ?? @public;
            @protected = LoadAccessibility(name + "_Protected", false) ?? @private ?? @public;
            @internal = LoadAccessibility(name + "_Internal", false) ?? @private ?? @public;
        }

        public EditorTexture @public { get; private set; }
        public EditorTexture @private { get; private set; }
        public EditorTexture @protected { get; private set; }
        public EditorTexture @internal { get; private set; }

        public static LanguageIconSet Load(string name)
        {
            return new LanguageIconSet(name);
        }

        private static EditorTexture LoadAccessibility(string name, bool required)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var path = $"{name}.png";

            return BoltCore.Resources.LoadIcon(path, required);
        }
    }
}
