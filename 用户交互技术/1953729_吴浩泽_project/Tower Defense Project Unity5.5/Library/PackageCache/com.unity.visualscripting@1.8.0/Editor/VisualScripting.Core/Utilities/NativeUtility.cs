using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Unity.VisualScripting
{
    public static class NativeUtility
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr handle);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string name);

        private static readonly Dictionary<string, int> usageCount = new Dictionary<string, int>();

        private static readonly object @lock = new object();

        // See: https://stackoverflow.com/questions/41450065/c-sharp-pinvoke-and-reloading-a-dll
        // https://stackoverflow.com/questions/28728891/have-to-do-freelibrary-2-times-although-i-have-done-loadlibrary-only-1-time-als
        private static bool supported => false; // Should work on Windows, but too buggy at the moment.

        public static void LoadModule(string name)
        {
            if (!supported)
            {
                return;
            }

            lock (@lock)
            {
                if (!usageCount.ContainsKey(name))
                {
                    usageCount.Add(name, 0);
                }

                usageCount[name]++;

                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    if (module.ModuleName == name)
                    {
                        UnityEngine.Debug.Log($"Module {name} is already loaded, skipping.\n");

                        return;
                    }
                }

                UnityEngine.Debug.Log($"Loading module {name}.\n");

                foreach (var path in Directory.GetFiles(Paths.assets, name, SearchOption.AllDirectories))
                {
                    UnityEngine.Debug.Log(path);

                    if (LoadLibrary(path) != IntPtr.Zero)
                    {
                        return;
                    }
                }

                throw new FileNotFoundException($"Failed to load native module '{name}'.", name);
            }
        }

        public static void UnloadModule(string name)
        {
            if (!supported)
            {
                return;
            }

            lock (@lock)
            {
                if (usageCount.ContainsKey(name))
                {
                    usageCount[name]--;

                    if (usageCount[name] == 0)
                    {
                        usageCount.Remove(name);
                    }
                }

                if (usageCount.ContainsKey(name))
                {
                    // Module is still in use
                    return;
                }

                var unloaded = false;

                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    if (module.ModuleName == name)
                    {
                        UnityEngine.Debug.Log($"Unloading module {name}.\n");

                        do { }
                        while (FreeLibrary(module.BaseAddress));

                        unloaded = true;
                    }
                }

                if (!unloaded)
                {
                    UnityEngine.Debug.Log($"Module {name} was not found to unload.\n");
                }
            }
        }

        public static ModuleScope Module(string name)
        {
            return new ModuleScope(name);
        }

        public struct ModuleScope : IDisposable
        {
            private readonly string name;

            public ModuleScope(string name)
            {
                this.name = name;

                LoadModule(name);
            }

            public void Dispose()
            {
                UnloadModule(name);
            }
        }
    }
}
