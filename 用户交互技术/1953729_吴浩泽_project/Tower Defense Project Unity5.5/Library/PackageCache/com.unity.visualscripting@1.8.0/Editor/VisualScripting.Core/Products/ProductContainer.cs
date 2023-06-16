using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class ProductContainer
    {
        private static Dictionary<string, Product> productsById;

        private static Dictionary<string, Type> productTypesById;

        public static bool initialized { get; private set; }

        public static bool initializing { get; private set; }

        public static IEnumerable<Product> products
        {
            get
            {
                EnsureInitialized();

                return productsById.Values;
            }
        }

        internal static void Initialize()
        {
            initializing = true;

            productTypesById = Codebase.editorTypes
                .Where(t => typeof(Product).IsAssignableFrom(t) && t.IsConcrete())
                .ToDictionary(GetProductID);

            productsById = new Dictionary<string, Product>();

            foreach (var productTypeById in productTypesById)
            {
                var productId = productTypeById.Key;
                var productType = productTypeById.Value;

                Product product;

                try
                {
                    product = (Product)productType.Instantiate();
                }
                catch (Exception ex)
                {
                    throw new TargetInvocationException($"Could not instantiate product '{productId}' ('{productType.CSharpName()}').", ex);
                }

                foreach (var plugin in PluginContainer.plugins)
                {
                    var productAttribute = plugin.GetType().GetAttribute<ProductAttribute>();

                    if (productAttribute?.id == productId)
                    {
                        product._plugins.Add(plugin);
                    }
                }

                productsById.Add(productId, product);
            }

            foreach (var product in products)
            {
                try
                {
                    product.Initialize();
                }
                catch (Exception ex)
                {
                    Debug.LogException(new Exception($"Failed to initialize product '{product.id}'.", ex));
                }
            }

            initializing = false;

            initialized = true;
        }

        private static void EnsureInitialized()
        {
            if (initializing)
            {
                return;
            }

            if (!initialized)
            {
                throw new InvalidOperationException("Trying to access product container before it is initialized.");
            }
        }

        public static string GetProductID(Type productType)
        {
            EnsureInitialized();

            Ensure.That(nameof(productType)).IsNotNull(productType);
            Ensure.That(nameof(productType)).IsOfType<Product>(productType);
            Ensure.That(nameof(productType)).HasAttribute<ProductAttribute>(productType);

            return productType.GetAttribute<ProductAttribute>().id;
        }

        public static Type GetProductType(string productId)
        {
            EnsureInitialized();

            Ensure.That(nameof(productId)).IsNotNull(productId);
            Ensure.That(nameof(productId)).IsKeyOf(productTypesById, productId);

            return productTypesById[productId];
        }

        public static Product GetProduct(string productId)
        {
            EnsureInitialized();

            Ensure.That(nameof(productId)).IsNotNull(productId);
            Ensure.That(nameof(productId)).IsKeyOf(productsById, productId);

            return productsById[productId];
        }

        public static bool HasProduct(string productId)
        {
            EnsureInitialized();

            Ensure.That(nameof(productId)).IsNotNull(productId);

            return productsById.ContainsKey(productId);
        }
    }
}
