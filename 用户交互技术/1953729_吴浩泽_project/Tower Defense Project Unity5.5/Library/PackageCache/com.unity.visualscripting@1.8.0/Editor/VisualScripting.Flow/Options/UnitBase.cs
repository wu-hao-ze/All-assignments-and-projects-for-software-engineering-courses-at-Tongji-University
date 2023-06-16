using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [InitializeAfterPlugins]
    public static class UnitBase
    {
        static UnitBase()
        {
            staticUnitsExtensions = new NonNullableList<Func<IEnumerable<IUnitOption>>>();
            dynamicUnitsExtensions = new NonNullableList<Func<IEnumerable<IUnitOption>>>();
            contextualUnitsExtensions = new NonNullableList<Func<GraphReference, IEnumerable<IUnitOption>>>();
            BackgroundWorker.Schedule(AutoLoad);
        }

        private static readonly object @lock = new object();

        private static HashSet<IUnitOption> options;

        private static void AutoLoad()
        {
            lock (@lock)
            {
                // If the fuzzy finder was opened really fast,
                // a load operation might already have started.
                if (options == null)
                {
                    Load();
                }
            }
        }

        private static void Load()
        {
            if (IsUnitOptionsBuilt())
            {
                // Update before loading if required, ensuring no "in-between" state
                // where the loaded options are not yet loaded.
                // The update code will not touch the options array if it is null.
                if (BoltFlow.Configuration.updateNodesAutomatically)
                {
                    try
                    {
                        ProgressUtility.DisplayProgressBar("Checking for codebase changes...", null, 0);

                        if (requiresUpdate)
                        {
                            Update();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to update node options.\nRetry with '{UnitOptionUtility.GenerateUnitDatabasePath}'.\n{ex}");
                    }
                    finally
                    {
                        ProgressUtility.ClearProgressBar();
                    }
                }

                lock (@lock)
                {
                    using (ProfilingUtility.SampleBlock("Load Node Database"))
                    {
                        using (NativeUtility.Module("sqlite3.dll"))
                        {
                            ProgressUtility.DisplayProgressBar("Loading node database...", null, 0);

                            SQLiteConnection database = null;

                            try
                            {
                                database = new SQLiteConnection(BoltFlow.Paths.unitOptions, SQLiteOpenFlags.ReadOnly);

                                int total;

                                total = database.Table<UnitOptionRow>().Count();

                                var progress = 0f;

                                options = new HashSet<IUnitOption>();

                                var failedOptions = new Dictionary<UnitOptionRow, Exception>();

                                foreach (var row in database.Table<UnitOptionRow>())
                                {
                                    try
                                    {
                                        var option = row.ToOption();

                                        options.Add(option);
                                    }
                                    catch (Exception rowEx)
                                    {
                                        failedOptions.Add(row, rowEx);
                                    }

                                    ProgressUtility.DisplayProgressBar("Loading node database...", BoltCore.Configuration.humanNaming ? row.labelHuman : row.labelProgrammer, progress++ / total);
                                }

                                if (failedOptions.Count > 0)
                                {
                                    var sb = new StringBuilder();

                                    sb.AppendLine($"{failedOptions.Count} node options failed to load and were skipped.");
                                    sb.AppendLine($"Try rebuilding the node options with '{UnitOptionUtility.GenerateUnitDatabasePath}' to purge outdated nodes.");
                                    sb.AppendLine();

                                    foreach (var failedOption in failedOptions)
                                    {
                                        sb.AppendLine(failedOption.Key.favoriteKey);
                                    }

                                    sb.AppendLine();

                                    foreach (var failedOption in failedOptions)
                                    {
                                        sb.AppendLine(failedOption.Key.favoriteKey + ": ");
                                        sb.AppendLine(failedOption.Value.ToString());
                                        sb.AppendLine();
                                    }

                                    Debug.LogWarning(sb.ToString());
                                }
                            }
                            catch (Exception ex)
                            {
                                options = new HashSet<IUnitOption>();
                                Debug.LogError($"Failed to load node options.\nTry to rebuild them with '{UnitOptionUtility.GenerateUnitDatabasePath}'.\n\n{ex}");
                            }
                            finally
                            {
                                database?.Close();
                                //ConsoleProfiler.Dump();
                                ProgressUtility.ClearProgressBar();
                            }
                        }
                    }
                }
            }
        }

        private static bool IsUnitOptionsBuilt()
        {
            return File.Exists(BoltFlow.Paths.unitOptions);
        }

        public static void Rebuild()
        {
            if (IsUnitOptionsBuilt())
            {
                VersionControlUtility.Unlock(BoltFlow.Paths.unitOptions);
                File.Delete(BoltFlow.Paths.unitOptions);
            }

            Build();
        }

        public static void Build(bool initialBuild = false)
        {
            if (IsUnitOptionsBuilt()) return;

            if (initialBuild)
            {
                ProgressUtility.SetTitleOverride("Visual Scripting: Initial Node Generation...");
            }

            const string progressTitle = "Visual Scripting: Building node database...";

            lock (@lock)
            {
                using (ProfilingUtility.SampleBlock("Update Node Database"))
                {
                    using (NativeUtility.Module("sqlite3.dll"))
                    {
                        SQLiteConnection database = null;

                        try
                        {
                            ProgressUtility.DisplayProgressBar(progressTitle, "Creating database...", 0);

                            PathUtility.CreateParentDirectoryIfNeeded(BoltFlow.Paths.unitOptions);
                            database = new SQLiteConnection(BoltFlow.Paths.unitOptions);
                            database.CreateTable<UnitOptionRow>();

                            ProgressUtility.DisplayProgressBar(progressTitle, "Updating codebase...", 0);

                            UpdateCodebase();

                            ProgressUtility.DisplayProgressBar(progressTitle, "Updating type mappings...", 0);

                            UpdateTypeMappings();

                            ProgressUtility.DisplayProgressBar(progressTitle,
                                "Converting codebase to node options...", 0);

                            options = new HashSet<IUnitOption>(GetStaticOptions());

                            var rows = new HashSet<UnitOptionRow>();

                            var progress = 0;
                            var lastShownProgress = 0f;

                            foreach (var option in options)
                            {
                                try
                                {
                                    var shownProgress = (float)progress / options.Count;

                                    if (shownProgress > lastShownProgress + 0.01f)
                                    {
                                        ProgressUtility.DisplayProgressBar(progressTitle,
                                            "Converting codebase to node options...", shownProgress);
                                        lastShownProgress = shownProgress;
                                    }

                                    rows.Add(option.Serialize());
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError($"Failed to save option '{option.GetType()}'.\n{ex}");
                                }

                                progress++;
                            }

                            ProgressUtility.DisplayProgressBar(progressTitle, "Writing to database...", 1);

                            try
                            {
                                database.CreateTable<UnitOptionRow>();
                                database.InsertAll(rows);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"Failed to write options to database.\n{ex}");
                            }
                        }
                        finally
                        {
                            database?.Close();
                            ProgressUtility.ClearProgressBar();
                            ProgressUtility.ClearTitleOverride();
                            AssetDatabase.Refresh();
                            //ConsoleProfiler.Dump();
                        }
                    }
                }
            }
        }

        public static void Update()
        {
            if (!IsUnitOptionsBuilt())
            {
                Build();
                return;
            }

            lock (@lock)
            {
                using (ProfilingUtility.SampleBlock("Update Node Database"))
                {
                    using (NativeUtility.Module("sqlite3.dll"))
                    {
                        var progressTitle = "Updating node database...";

                        SQLiteConnection database = null;

                        try
                        {
                            VersionControlUtility.Unlock(BoltFlow.Paths.unitOptions);

                            var steps = 7f;
                            var step = 0f;

                            ProgressUtility.DisplayProgressBar(progressTitle, "Connecting to database...", step++ / steps);

                            database = new SQLiteConnection(BoltFlow.Paths.unitOptions);

                            ProgressUtility.DisplayProgressBar(progressTitle, "Updating type mappings...", step++ / steps);

                            UpdateTypeMappings();

                            ProgressUtility.DisplayProgressBar(progressTitle, "Fetching modified scripts...", step++ / steps);

                            var modifiedScriptGuids = GetModifiedScriptGuids().Distinct().ToHashSet();

                            ProgressUtility.DisplayProgressBar(progressTitle, "Fetching deleted scripts...", step++ / steps);

                            var deletedScriptGuids = GetDeletedScriptGuids().Distinct().ToHashSet();

                            ProgressUtility.DisplayProgressBar(progressTitle, "Updating codebase...", step++ / steps);

                            var modifiedScriptTypes = modifiedScriptGuids.SelectMany(GetScriptTypes).ToArray();

                            UpdateCodebase(modifiedScriptTypes);

                            var outdatedScriptGuids = new HashSet<string>();
                            outdatedScriptGuids.UnionWith(modifiedScriptGuids);
                            outdatedScriptGuids.UnionWith(deletedScriptGuids);

                            ProgressUtility.DisplayProgressBar(progressTitle, "Removing outdated node options...", step++ / steps);

                            options?.RemoveWhere(option => outdatedScriptGuids.Overlaps(option.sourceScriptGuids));

                            // We want to use the database level WHERE here for speed,
                            // so we'll run multiple queries, one for each outdated script GUID.

                            foreach (var outdatedScriptGuid in outdatedScriptGuids)
                            {
                                foreach (var outdatedRowId in database.Table<UnitOptionRow>()
                                         .Where(row => row.sourceScriptGuids.Contains(outdatedScriptGuid))
                                         .Select(row => row.id))
                                {
                                    database.Delete<UnitOptionRow>(outdatedRowId);
                                }
                            }

                            ProgressUtility.DisplayProgressBar(progressTitle, "Converting codebase to node options...", step++ / steps);

                            var newOptions = new HashSet<IUnitOption>(modifiedScriptGuids.SelectMany(GetScriptTypes)
                                .Distinct()
                                .SelectMany(GetIncrementalOptions));

                            var rows = new HashSet<UnitOptionRow>();

                            float progress = 0;

                            foreach (var newOption in newOptions)
                            {
                                options?.Add(newOption);

                                try
                                {
                                    ProgressUtility.DisplayProgressBar(progressTitle, newOption.label, (step / steps) + ((1 / step) * (progress / newOptions.Count)));
                                    rows.Add(newOption.Serialize());
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError($"Failed to serialize option '{newOption.GetType()}'.\n{ex}");
                                }

                                progress++;
                            }

                            ProgressUtility.DisplayProgressBar(progressTitle, "Writing to database...", 1);

                            try
                            {
                                database.InsertAll(rows);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"Failed to write options to database.\n{ex}");
                            }

                            // Make sure the database is touched to the current date,
                            // even if we didn't do any change. This will avoid unnecessary
                            // analysis in future update checks.
                            File.SetLastWriteTimeUtc(BoltFlow.Paths.unitOptions, DateTime.UtcNow);
                        }
                        finally
                        {
                            database?.Close();
                            ProgressUtility.ClearProgressBar();
                            UnityAPI.Async(AssetDatabase.Refresh);
                            //ConsoleProfiler.Dump();
                        }
                    }
                }
            }
        }

        public static IEnumerable<IUnitOption> Subset(UnitOptionFilter filter, GraphReference reference)
        {
            lock (@lock)
            {
                if (options == null)
                {
                    Load();
                }

                var dynamicOptions = UnityAPI.Await(() => GetDynamicOptions().ToHashSet());
                var contextualOptions = UnityAPI.Await(() => GetContextualOptions(reference).ToHashSet());

                return LinqUtility.Concat<IUnitOption>(options, dynamicOptions, contextualOptions)
                    .Where((filter ?? UnitOptionFilter.Any).ValidateOption)
                    .ToArray();
            }
        }

        #region Units

        private static CodebaseSubset codebase;

        private static void UpdateCodebase(IEnumerable<Type> typeSet = null)
        {
            using var profilerScope = ProfilingUtility.SampleBlock("UpdateCodebase");
            if (typeSet == null)
            {
                typeSet = Codebase.settingsTypes;
            }
            else
            {
                typeSet = typeSet.Where(t => Codebase.settingsTypes.Contains(t));
            }

            Codebase.UpdateSettings();
            codebase = Codebase.Subset(typeSet, TypeFilter.Any.Configured(), MemberFilter.Any.Configured(), TypeFilter.Any.Configured(false));
            codebase.Cache();
        }

        private static IEnumerable<IUnitOption> GetStaticOptions()
        {
            // Standalones

            foreach (var unit in Codebase.ludiqRuntimeTypes.Where(t => typeof(IUnit).IsAssignableFrom(t) &&
                t.IsConcrete() &&
                t.GetDefaultConstructor() != null &&
                !t.HasAttribute<SpecialUnitAttribute>() &&
                (EditorPlatformUtility.allowJit || !t.HasAttribute<AotIncompatibleAttribute>()) &&
                t.GetDefaultConstructor() != null)
                     .Select(t => (IUnit)t.Instantiate()))
            {
                yield return unit.Option();
            }

            // Self

            yield return new This().Option();

            // Types

            foreach (var type in codebase.types)
            {
                foreach (var typeOption in GetTypeOptions(type))
                {
                    yield return typeOption;
                }
            }

            // Members

            foreach (var member in codebase.members)
            {
                foreach (var memberOption in GetMemberOptions(member))
                {
                    yield return memberOption;
                }
            }

            // Events

            foreach (var eventType in Codebase.ludiqRuntimeTypes.Where(t => typeof(IEventUnit).IsAssignableFrom(t) && t.IsConcrete()))
            {
                yield return ((IEventUnit)eventType.Instantiate()).Option();
            }

            // Blank Variables

            yield return new GetVariableOption(VariableKind.Flow);
            yield return new GetVariableOption(VariableKind.Graph);
            yield return new GetVariableOption(VariableKind.Object);
            yield return new GetVariableOption(VariableKind.Scene);
            yield return new GetVariableOption(VariableKind.Application);
            yield return new GetVariableOption(VariableKind.Saved);

            yield return new SetVariableOption(VariableKind.Flow);
            yield return new SetVariableOption(VariableKind.Graph);
            yield return new SetVariableOption(VariableKind.Object);
            yield return new SetVariableOption(VariableKind.Scene);
            yield return new SetVariableOption(VariableKind.Application);
            yield return new SetVariableOption(VariableKind.Saved);

            yield return new IsVariableDefinedOption(VariableKind.Flow);
            yield return new IsVariableDefinedOption(VariableKind.Graph);
            yield return new IsVariableDefinedOption(VariableKind.Object);
            yield return new IsVariableDefinedOption(VariableKind.Scene);
            yield return new IsVariableDefinedOption(VariableKind.Application);
            yield return new IsVariableDefinedOption(VariableKind.Saved);

            // Blank Super Unit

            yield return SubgraphUnit.WithInputOutput().Option();

            // Extensions

            foreach (var staticUnitsExtension in staticUnitsExtensions)
            {
                foreach (var extensionStaticUnit in staticUnitsExtension())
                {
                    yield return extensionStaticUnit;
                }
            }
        }

        private static IEnumerable<IUnitOption> GetIncrementalOptions(Type type)
        {
            if (!codebase.ValidateType(type))
            {
                yield break;
            }

            foreach (var typeOption in GetTypeOptions(type))
            {
                yield return typeOption;
            }

            foreach (var member in codebase.FilterMembers(type))
            {
                foreach (var memberOption in GetMemberOptions(member))
                {
                    yield return memberOption;
                }
            }
        }

        private static IEnumerable<IUnitOption> GetTypeOptions(Type type)
        {
            if (type == typeof(object))
            {
                yield break;
            }

            // Struct Initializer

            if (type.IsStruct())
            {
                yield return new CreateStruct(type).Option();
            }

            // Literals

            if (type.HasInspector())
            {
                yield return new Literal(type).Option();

                if (EditorPlatformUtility.allowJit)
                {
                    var listType = typeof(List<>).MakeGenericType(type);

                    yield return new Literal(listType).Option();
                }
            }

            // Exposes

            if (!type.IsEnum)
            {
                yield return new Expose(type).Option();
            }
        }

        private static IEnumerable<IUnitOption> GetMemberOptions(Member member)
        {
            // Operators are handled with special math units
            // that are more elegant than the raw methods
            if (member.isOperator)
            {
                yield break;
            }

            // Conversions are handled automatically by connections
            if (member.isConversion)
            {
                yield break;
            }

            if (member.isAccessor)
            {
                if (member.isPubliclyGettable)
                {
                    yield return new GetMember(member).Option();
                }

                if (member.isPubliclySettable)
                {
                    yield return new SetMember(member).Option();
                }
            }
            else if (member.isPubliclyInvocable)
            {
                yield return new InvokeMember(member).Option();
            }
        }

        private static IEnumerable<IUnitOption> GetDynamicOptions()
        {
            // Super Units

            var flowMacros = AssetUtility.GetAllAssetsOfType<ScriptGraphAsset>().ToArray();

            foreach (var superUnit in flowMacros.Select(flowMacro => new SubgraphUnit(flowMacro)))
            {
                yield return superUnit.Option();
            }

            // Extensions

            foreach (var dynamicUnitsExtension in dynamicUnitsExtensions)
            {
                foreach (var extensionDynamicUnit in dynamicUnitsExtension())
                {
                    yield return extensionDynamicUnit;
                }
            }
        }

        private static IEnumerable<IUnitOption> GetContextualOptions(GraphReference reference)
        {
            foreach (var variableKind in Enum.GetValues(typeof(VariableKind)).Cast<VariableKind>())
            {
                foreach (var graphVariableName in EditorVariablesUtility.GetVariableNameSuggestions(variableKind, reference))
                {
                    yield return new GetVariableOption(variableKind, graphVariableName);
                    yield return new SetVariableOption(variableKind, graphVariableName);
                    yield return new IsVariableDefinedOption(variableKind, graphVariableName);
                }
            }

            // Extensions

            foreach (var contextualUnitsExtension in contextualUnitsExtensions)
            {
                foreach (var extensionContextualUnitOption in contextualUnitsExtension(reference))
                {
                    yield return extensionContextualUnitOption;
                }
            }
        }

        #endregion

        #region Scripts

        private static Dictionary<Type, HashSet<string>> typesToGuids;
        private static Dictionary<string, HashSet<Type>> guidsToTypes;

        private static void UpdateTypeMappings()
        {
            using var profilerScope = ProfilingUtility.SampleBlock("UpdateTypeMappings");

            typesToGuids = new Dictionary<Type, HashSet<string>>();
            guidsToTypes = new Dictionary<string, HashSet<Type>>();

            UnityAPI.AwaitForever(() =>
            {
                foreach (var script in UnityEngine.Resources.FindObjectsOfTypeAll<MonoScript>())
                {
                    var type = script.GetClass();

                    // Skip scripts without types
                    if (type == null)
                    {
                        continue;
                    }

                    var path = AssetDatabase.GetAssetPath(script);
                    // Skip built-in Unity plugins, which are referenced by full path
                    if (!path.StartsWith("Assets"))
                    {
                        continue;
                    }

                    var guid = AssetDatabase.AssetPathToGUID(path);
                    // Add the GUID to the list, even if it doesn't have any type
                    if (!guidsToTypes.ContainsKey(guid))
                    {
                        guidsToTypes.Add(guid, new HashSet<Type>());
                    }

                    if (!typesToGuids.ContainsKey(type))
                    {
                        typesToGuids.Add(type, new HashSet<string>());
                    }

                    typesToGuids[type].Add(guid);
                    guidsToTypes[guid].Add(type);
                }
            });
        }

        public static IEnumerable<string> GetScriptGuids(Type type)
        {
            if (typesToGuids == null)
            {
                UpdateTypeMappings();
            }

            using (var recursion = Recursion.New(1))
            {
                return GetScriptGuids(recursion, type).ToArray(); // No delayed execution for recursion disposal
            }
        }

        private static IEnumerable<string> GetScriptGuids(Recursion recursion, Type type)
        {
            if (!recursion?.TryEnter(type) ?? false)
            {
                yield break;
            }

            if (typesToGuids.ContainsKey(type))
            {
                foreach (var guid in typesToGuids[type])
                {
                    yield return guid;
                }
            }

            // Loop through generic arguments.
            // For example, a List<Enemy> type should return the script GUID for Enemy.
            if (type.IsGenericType)
            {
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    foreach (var genericGuid in GetScriptGuids(recursion, genericArgument))
                    {
                        yield return genericGuid;
                    }
                }
            }
        }

        public static IEnumerable<Type> GetScriptTypes(string guid)
        {
            if (guidsToTypes == null)
            {
                UpdateTypeMappings();
            }

            if (guidsToTypes.ContainsKey(guid))
            {
                return guidsToTypes[guid];
            }
            else
            {
                return Enumerable.Empty<Type>();
            }
        }

        private static bool requiresUpdate => GetModifiedScriptGuids().Any() || GetDeletedScriptGuids().Any();

        private static IEnumerable<string> GetModifiedScriptGuids()
        {
            var guids = new HashSet<string>();

            UnityAPI.AwaitForever(() =>
            {
                var databaseTimestamp = File.GetLastWriteTimeUtc(BoltFlow.Paths.unitOptions);

                foreach (var script in UnityEngine.Resources.FindObjectsOfTypeAll<MonoScript>())
                {
                    var path = AssetDatabase.GetAssetPath(script);
                    var guid = AssetDatabase.AssetPathToGUID(path);

                    // Skip built-in Unity plugins, which are referenced by full path
                    if (!path.StartsWith("Assets"))
                    {
                        continue;
                    }

                    var scriptTimestamp = File.GetLastWriteTimeUtc(Path.Combine(Paths.project, path));

                    if (scriptTimestamp > databaseTimestamp)
                    {
                        guids.Add(guid);
                    }
                }
            });

            return guids;
        }

        private static IEnumerable<string> GetDeletedScriptGuids()
        {
            if (!IsUnitOptionsBuilt())
            {
                return Enumerable.Empty<string>();
            }

            using (NativeUtility.Module("sqlite3.dll"))
            {
                SQLiteConnection database = null;

                try
                {
                    HashSet<string> databaseGuids;

                    lock (@lock)
                    {
                        database = new SQLiteConnection(BoltFlow.Paths.unitOptions);

                        databaseGuids = database.Query<UnitOptionRow>($"SELECT DISTINCT {nameof(UnitOptionRow.sourceScriptGuids)} FROM {nameof(UnitOptionRow)}")
                            .Select(row => row.sourceScriptGuids)
                            .NotNull()
                            .SelectMany(guids => guids.Split(','))
                            .ToHashSet();
                    }

                    var assetGuids = UnityAPI.AwaitForever(() => UnityEngine.Resources
                        .FindObjectsOfTypeAll<MonoScript>()
                        .Where(script => script.GetClass() != null)
                        .Select(script => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(script)))
                        .ToHashSet());

                    databaseGuids.ExceptWith(assetGuids);

                    return databaseGuids;
                }
                finally
                {
                    database?.Close();
                }
            }
        }

        #endregion

        #region Extensions

        public static NonNullableList<Func<IEnumerable<IUnitOption>>> staticUnitsExtensions { get; }

        public static NonNullableList<Func<IEnumerable<IUnitOption>>> dynamicUnitsExtensions { get; }

        public static NonNullableList<Func<GraphReference, IEnumerable<IUnitOption>>> contextualUnitsExtensions { get; }

        #endregion

        #region Duplicates

        public static IEnumerable<T> WithoutInheritedDuplicates<T>(this IEnumerable<T> items, Func<T, IUnitOption> optionSelector, CancellationToken cancellation)
        {
            // Doing everything we can to avoid reflection here, as it then becomes the main search bottleneck

            var _items = items.ToArray();

            var pseudoDeclarers = new HashSet<Member>();

            foreach (var item in _items.Cancellable(cancellation))
            {
                var option = optionSelector(item);

                if (option is IMemberUnitOption memberOption)
                {
                    if (memberOption.targetType == memberOption.pseudoDeclarer.targetType)
                    {
                        pseudoDeclarers.Add(memberOption.pseudoDeclarer);
                    }
                }
            }

            foreach (var item in _items.Cancellable(cancellation))
            {
                var option = optionSelector(item);

                if (option is IMemberUnitOption memberOption)
                {
                    if (pseudoDeclarers.Contains(memberOption.member) || !pseudoDeclarers.Contains(memberOption.pseudoDeclarer))
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

        #endregion
    }
}
