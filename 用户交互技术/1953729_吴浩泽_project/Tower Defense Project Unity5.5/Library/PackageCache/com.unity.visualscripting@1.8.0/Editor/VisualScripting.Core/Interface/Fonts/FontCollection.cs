using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class FontCollection
    {
        public FontCollection(Func<FontVariant, Font> load)
        {
            if (load == null)
            {
                throw new ArgumentNullException(nameof(load));
            }

            this.load = load;

            variants = new Dictionary<FontVariant, Font>();
        }

        private readonly Dictionary<FontVariant, Font> variants;
        private readonly Func<FontVariant, Font> load;

        public Font this[FontWeight weight, FontStyle style]
        {
            get
            {
                var variant = new FontVariant(weight, style);

                if (!variants.ContainsKey(variant))
                {
                    variants.Add(variant, load(variant));
                }

                return variants[variant];
            }
        }
    }
}
