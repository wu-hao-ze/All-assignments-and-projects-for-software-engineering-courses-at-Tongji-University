using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class VersionControlUtility
    {
        // Perforce and other VCS have a lock mechanism that usually
        // only makes the file writable once checked out. We need to
        // check them out before writing to them for auto-generated files.
        public static void Unlock(string path)
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            UnityAPI.Await
                (
                    () =>
                    {
                        // The API changed in 2019, adding a third optional ChangeSet parameter
                        // which defaults to null but breaks the compiled signature below
                        // Furthermore, we can't even so much as have the call in the body of this method,
                        // or it will fail even if the if branch evaluates to false. So we

                        if (File.Exists(path) && Provider.enabled && Provider.isActive && Provider.hasCheckoutSupport)
                        {
                            try
                            {
                                var provider = typeof(Provider);

                                if (EditorApplicationUtility.unityVersion >= "2019.1.0")
                                {
                                    var method = provider.GetMethods()
                                        .FirstOrDefault
                                        (
                                            m => m.Name == "Checkout" &&
                                            m.GetParameters().Length == 3 &&
                                            m.GetParameters()[0].ParameterType == typeof(string) &&
                                            m.GetParameters()[1].ParameterType == typeof(CheckoutMode)
                                        );

                                    if (method == null)
                                    {
                                        throw new MissingMemberException(provider.FullName, "Checkout");
                                    }

                                    method.InvokeOptimized(null, PathUtility.FromProject(path), CheckoutMode.Both, null);
                                }
                                else
                                {
                                    var method = provider.GetMethods()
                                        .FirstOrDefault
                                        (
                                            m => m.Name == "Checkout" &&
                                            m.GetParameters().Length == 2 &&
                                            m.GetParameters()[0].ParameterType == typeof(string) &&
                                            m.GetParameters()[1].ParameterType == typeof(CheckoutMode)
                                        );

                                    if (method == null)
                                    {
                                        throw new MissingMemberException(provider.FullName, "Checkout");
                                    }

                                    method.InvokeOptimized(null, PathUtility.FromProject(path), CheckoutMode.Both);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning($"Failed to automatically checkout file from version control:\n{path}\n{ex}");
                            }
                        }

                        if (File.Exists(path))
                        {
                            var info = new FileInfo(path);

                            if (info.IsReadOnly)
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine($"File '{info.Name}' is read-only despite attempted checkout. Manually forcing to writable.");
                                sb.AppendLine($"This may cause version control issues. Please report the following debug information:");
                                sb.AppendLine($"File Exists: {File.Exists(path)}");
                                sb.AppendLine($"Provider.enabled: {Provider.enabled}");
                                sb.AppendLine($"Provider.isActive: {Provider.isActive}");
                                sb.AppendLine($"Provider.hasCheckoutSupport: {Provider.hasCheckoutSupport}");
                                Debug.LogWarning(sb.ToString());

                                info.IsReadOnly = false;
                            }
                        }
                    }
                );
        }
    }
}
