using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class FuzzyWindow : EditorWindow
    {
        private static readonly Color lightSkinColor = new Color(0.75f, 0.75f, 0.75f);

        private static Color backgroundWindowColor;
        public void Populate(FuzzyOptionNode node, IEnumerable<object> childrenValues, CancellationToken? cancellation = null)
        {
            if (node.isPopulated)
            {
                return;
            }

            var i = 0;

            var _childrenValues = childrenValues.ToArray();

            lock (guiLock)
            {
                node.hasChildren = _childrenValues.Length > 0;
            }

            foreach (var childValue in _childrenValues)
            {
                var childOption = tree.Option(childValue);

                try
                {
                    childOption.OnPopulate();
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to display {childOption.GetType()}: \n{ex}");
                    continue;
                }

                DisplayProgressBar($"{childOption.label}... ({++i} / {_childrenValues.Length})", (float)i / _childrenValues.Length);

                var hasChildren = tree.Children(childValue).Any();

                var include = !childOption.parentOnly || hasChildren;

                if (!include)
                {
                    continue;
                }

                string label;

                if (node == searchRoot)
                {
                    label = tree.SearchResultLabel(childValue, query);
                }
                else if (node == favoritesRoot)
                {
                    label = tree.FavoritesLabel(childValue);
                }
                else
                {
                    label = childOption.label;
                }

                var childNode = new FuzzyOptionNode(childOption, label);

                childNode.hasChildren = hasChildren;

                lock (guiLock)
                {
                    node.children.Add(childNode);
                }

                cancellation?.ThrowIfCancellationRequested();
            }

            lock (guiLock)
            {
                node.isPopulated = true;
            }
        }

        static FuzzyWindow()
        {
            backgroundWindowColor = EditorGUIUtility.isProSkin ? GUI.color : lightSkinColor;

            ShowAsDropDownFitToScreen = typeof(FuzzyWindow).GetMethod("ShowAsDropDownFitToScreen", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static GUIStyle defaultOptionStyle => Styles.optionWithIcon;

        private static Event e => Event.current;

        public static class Styles
        {
            static Styles()
            {
                header = new GUIStyle("In BigTitle");

                header.font = EditorStyles.boldLabel.font;
                header.normal.textColor = ColorPalette.unityForeground;
                header.alignment = TextAnchor.MiddleCenter;
                header.padding = new RectOffset(0, 0, 0, 0);

                footerBackground = new GUIStyle("In BigTitle");

                optionWithIcon = new GUIStyle("PR Label");
                optionWithIcon.richText = true;
                optionWithIcon.alignment = TextAnchor.MiddleLeft;
                optionWithIcon.padding.left -= 15;
                optionWithIcon.fixedHeight = 20f;

                optionWithoutIcon = new GUIStyle(optionWithIcon);
                optionWithoutIcon.padding.left += 17;

                optionWithIconDim = new GUIStyle(optionWithIcon);
                optionWithIconDim.normal.textColor = ColorPalette.unityForegroundDim;

                optionWithoutIconDim = new GUIStyle(optionWithIcon);
                optionWithoutIconDim.normal.textColor = ColorPalette.unityForegroundDim;

                background = new GUIStyle("grey_border");

                rightArrow = new GUIStyle("AC RightArrow");

                leftArrow = new GUIStyle("AC LeftArrow");

                searchField = new GUIStyle("SearchTextField");

                searchFieldCancelButton = new GUIStyle("SearchCancelButton");

                searchFieldCancelButtonEmpty = new GUIStyle("SearchCancelButtonEmpty");

                insufficientSearch = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                insufficientSearch.normal.textColor = Color.grey;
                insufficientSearch.wordWrap = true;
                insufficientSearch.padding = new RectOffset(10, 10, 10, 10);
                insufficientSearch.alignment = TextAnchor.MiddleCenter;

                check = new GUIStyle();
                var checkTexture = BoltCore.Resources.LoadTexture("Check.png", new TextureResolution[] { 12, 24 }, CreateTextureOptions.PixelPerfect);
                check.normal.background = checkTexture?[12];
                check.normal.scaledBackgrounds = new[] { checkTexture?[24] };
                check.fixedHeight = 12;
                check.fixedWidth = 12;

                star = new GUIStyle();
                var starOffTexture = BoltCore.Resources.LoadIcon("StarOff.png");
                var starOnTexture = BoltCore.Resources.LoadIcon("StarOn.png");
                star.normal.background = starOffTexture?[16];
                star.normal.scaledBackgrounds = new[] { starOffTexture?[32] };
                star.onNormal.background = starOnTexture?[16];
                star.onNormal.scaledBackgrounds = new[] { starOnTexture?[32] };
                star.fixedHeight = 16;
                star.fixedWidth = 16;
                favoritesIcon = starOnTexture;
            }

            public static readonly GUIStyle header;
            public static readonly GUIStyle footerBackground;
            public static readonly GUIStyle optionWithIcon;
            public static readonly GUIStyle optionWithoutIcon;
            public static readonly GUIStyle optionWithIconDim;
            public static readonly GUIStyle optionWithoutIconDim;
            public static readonly GUIStyle background;
            public static readonly GUIStyle rightArrow;
            public static readonly GUIStyle leftArrow;
            public static readonly GUIStyle searchField;
            public static readonly GUIStyle searchFieldCancelButton;
            public static readonly GUIStyle searchFieldCancelButtonEmpty;
            public static readonly GUIStyle insufficientSearch;
            public static readonly GUIStyle check;
            public static readonly GUIStyle star;
            public static readonly EditorTexture favoritesIcon;
            public static readonly float searchFieldHeight = 20;
            public static readonly float headerHeight = 25;
            public static readonly float optionHeight = 20;
            public static readonly float maxOptionWidth = 800;
        }

        private class Root : FuzzyOption<object>
        {
            public Root(GUIContent header)
            {
                label = header.text;
                icon = EditorTexture.Single(header.image);
            }
        }

        private class SearchRoot : FuzzyOption<object>
        {
            public SearchRoot()
            {
                label = searchHeader;
            }
        }

        public class FavoritesRoot : FuzzyOption<object>
        {
            public FavoritesRoot()
            {
                label = "Favorites";
                icon = Styles.favoritesIcon;
            }
        }

        #region Lifecycle

        private Action<IFuzzyOption> callback;

        [DoNotSerialize]
        public static FuzzyWindow instance { get; private set; }

        private IFuzzyOptionTree tree;

        private bool requireRepaint;

        public static void Show(Rect activatorPosition, IFuzzyOptionTree optionTree, Action<IFuzzyOption> callback)
        {
            Ensure.That(nameof(optionTree)).IsNotNull(optionTree);

            // Makes sure control exits DelayedTextFields before opening the window
            GUIUtility.keyboardControl = 0;

            if (instance != null)
            {
                instance.Close();
            }
            else
            {
                instance = CreateInstance<FuzzyWindow>();

                instance.Initialize(optionTree, callback);

                instance.CreateWindow(activatorPosition);
            }
        }

        private void OnEnable()
        {
            query = string.Empty;
        }

        private void OnDisable()
        {
            instance = null;
        }

#if UNITY_EDITOR_LINUX
        private void OnLostFocus()
        {
            Close();
        }
#endif

        private void Update()
        {
            //This variable should not be null, but when we click the play button the FuzzyFinder (DropDown window) does not close on 2019 and 2020
            //but it does close on 2018.
            //The FuzzyFinder (DropDown window) should close if you click anywhere in the Editor
            //Once this issue is fixed this null check can be removed
            if (stack != null)
            {
                activeParent.isLoading = !activeParent.isPopulated;

                if (requireRepaint)
                {
                    Repaint();
                    requireRepaint = false;
                }
            }
        }

        private void CreateWindow(Rect activatorPosition)
        {
            // Port the activator position to screen space
            activatorPosition.position = GUIUtility.GUIToScreenPoint(activatorPosition.position);
            this.activatorPosition = activatorPosition;

            // Show and focus the window
            wantsMouseMove = true;
            var initialSize = new Vector2(activatorPosition.width, height);

            this.ShowAsDropDownWithKeyboardFocus(activatorPosition, initialSize);

            Focus();
        }

        private void Initialize(IFuzzyOptionTree optionTree, Action<IFuzzyOption> callback)
        {
            tree = optionTree;

            // Create the hierarchy

            stack = new Stack<FuzzyOptionNode>();
            root = new FuzzyOptionNode(new Root(optionTree.header));

            ExecuteTask(delegate
            {
                optionTree.Prewarm();

                Populate(root, optionTree.Root());

                // Fit height to children if there is no depth and no search

                var hasSubChildren = root.children.Any(option => option.hasChildren);

                if (!optionTree.searchable && !hasSubChildren)
                {
                    height = 0;

                    if (!string.IsNullOrEmpty(root.option.headerLabel))
                    {
                        height += Styles.headerHeight;
                    }

                    height += root.children.Count * Styles.optionHeight + 1;
                }
            });

            // Add favorites

            favoritesRoot = new FuzzyOptionNode(new FavoritesRoot());
            UpdateFavorites();

            // Setup the search

            searchRoot = new FuzzyOptionNode(new SearchRoot());
            Search();

            // Assign the callback

            this.callback = callback;
            // Show and focus the window
        }

        #endregion

        #region Hierarchy

        private FuzzyOptionNode root;
        private FuzzyOptionNode searchRoot;
        private Stack<FuzzyOptionNode> stack;

        private FuzzyOptionNode activeRoot => !hasSearch ? root : searchRoot;

        private FuzzyOptionNode activeParent => stack.Peek();

        private int activeSelectedIndex
        {
            get
            {
                return activeParent.selectedIndex;
            }
            set
            {
                lock (guiLock)
                {
                    activeParent.selectedIndex = value;
                }
            }
        }

        private IList<FuzzyOptionNode> activeNodes => activeParent.children;

        private FuzzyOptionNode activeNode
        {
            get
            {
                if (activeSelectedIndex >= 0 && activeSelectedIndex < activeNodes.Count)
                {
                    return activeNodes[activeSelectedIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        private FuzzyOptionNode GetLevelRelative(int levelOffset)
        {
            return stack.Skip(-levelOffset).FirstOrDefault();
        }

        private void SelectParent()
        {
            if (stack.Count > 1)
            {
                animTarget = 0;
                lastRepaintTime = DateTime.Now;
            }
        }

        private void EnterChild(FuzzyOptionNode node)
        {
            if (node == null || !node.hasChildren)
            {
                return;
            }

            ExecuteTask(delegate
            {
                Populate(node, tree.Children(node.option.value));
            });

            lastRepaintTime = DateTime.Now;

            if (animTarget == 0)
            {
                animTarget = 1;
            }
            else if (anim == 1)
            {
                anim = 0;
                stack.Push(node);
            }
        }

        private void SelectChild(FuzzyOptionNode node)
        {
            if (node == null)
            {
                return;
            }

            if (node.hasChildren)
            {
                EnterChild(node);
            }
            else if (callback != null)
            {
                callback(node.option);
            }
        }

        #endregion

        #region Search

        private int minSearchLength = 2;
        private string query;
        private string delayedQuery;
        private bool letQueryClear;
        private string searchFieldName = "FuzzySearch";
        private static string searchHeader = "Search";
        private CancellationTokenSource searchCancellationTokenSource;

        private bool hasSearch => !string.IsNullOrEmpty(query);

        private bool hasSufficientSearch => hasSearch && query.Length >= minSearchLength;

        private void Search()
        {
            stack.Clear();

            if (hasSearch)
            {
                searchCancellationTokenSource?.Cancel();

                if (hasSufficientSearch)
                {
                    var queryNow = query;

                    searchCancellationTokenSource = new CancellationTokenSource();
                    var searchCancellationToken = searchCancellationTokenSource.Token;

                    ExecuteTask(delegate
                    {
                        if (searchCancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        DisplayProgressBar($"Searching for '{queryNow}'...", 0);

                        lock (guiLock)
                        {
                            searchRoot.children.Clear();
                            searchRoot.isPopulated = false;
                        }

                        Populate(searchRoot, tree.OrderedSearchResults(query, searchCancellationToken).Cancellable(searchCancellationToken).Take(BoltCore.Configuration.maxSearchResults));
                        activeSelectedIndex = activeNodes.Count >= 1 ? 0 : -1;
                    });
                }
                else
                {
                    ExecuteTask(delegate
                    {
                        lock (guiLock)
                        {
                            searchRoot.children.Clear();
                            searchRoot.isPopulated = true;
                        }
                    });
                }

                stack.Push(searchRoot);
            }
            else
            {
                stack.Push(root);
                animTarget = 1;
                lastRepaintTime = DateTime.Now;
                return;
            }
        }

        #endregion

        #region Favorites

        private FuzzyOptionNode favoritesRoot;

        private void UpdateFavorites()
        {
            ExecuteTask(delegate
            {
                if (tree.favorites != null)
                {
                    DisplayProgressBar("Fetching favorites...", 0);
                    favoritesRoot.children.Clear();
                    favoritesRoot.isPopulated = false;
                    // Adding a where clause in case a favorited item was later changed to be unfavoritable.
                    Populate(favoritesRoot, tree.favorites.Where(favorite => tree.CanFavorite(favorite)));
                }
                else
                {
                    favoritesRoot.children.Clear();
                    favoritesRoot.isPopulated = true;
                }

                root.children.Remove(favoritesRoot);

                if (favoritesRoot.hasChildren)
                {
                    root.children.Insert(0, favoritesRoot);
                }
            });
        }

        #endregion

        #region Animation

        private float anim = 1;
        private int animTarget = 1;
        private float animationSpeed = 4;

        private DateTime lastRepaintTime;

        public float repaintDeltaTime => (float)(DateTime.Now - lastRepaintTime).TotalSeconds;

        private bool isAnimating => anim != animTarget;

        #endregion

        #region Positioning

        private static readonly MethodInfo ShowAsDropDownFitToScreen;

        private float maxHeight = 320;
        private float height = 320;
        private float minWidth = 200;
        private float minOptionWidth;
        private float headerWidth;
        private float footerHeight;
        private Rect activatorPosition;
        private bool scrollToSelected;
        private float initialY;
        private bool initialYSet;

        private void OnPositioning()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            if (!initialYSet)
            {
                initialY = this.position.y;
                initialYSet = true;
            }

            var totalWidth = Mathf.Max(minWidth, activatorPosition.width, minOptionWidth + 36, headerWidth + 36);

            var totalHeight = Mathf.Min(height, maxHeight);

            var position = (Rect)ShowAsDropDownFitToScreen.Invoke(this, new object[] { activatorPosition, new Vector2(totalWidth, totalHeight), null });

            position.y = initialY;

            if (!isAnimating && activeNode?.option != null && activeNode.option.hasFooter)
            {
                var newfooterHeight = activeNode.option.GetFooterHeight(totalWidth - footerWidthMargin);
                footerHeight = footerHeight < newfooterHeight ? newfooterHeight : footerHeight;
            }
            position.height += footerHeight;

            if (Application.platform == RuntimePlatform.OSXEditor && BoltCore.Configuration.limitFuzzyFinderHeight)
            {
                // OSX disregards the Y entirely if the window is higher than the desktop space
                // and will try to move it up until it fits. Therefore, we'll cut the window down here.
                // However, we can't use the screen resolution, because it doesn't include the dock.

                var maxY = LudiqGUIUtility.mainEditorWindowPosition.yMax;

                if (position.yMax > maxY)
                {
                    position.height -= (position.yMax - maxY);
                }
            }

            if (this.position != position || minSize != position.size)
            {
                minSize = maxSize = position.size;
                this.position = position;
            }

            GUIUtility.ExitGUI();
        }

        #endregion

        #region GUI

        //Be sure do not call any layout change while the UI is repainting or force a repaint
        //before the layout is completed.
        //This will probably break the UI
        private void OnGUI()
        {
            try
            {
                lock (guiLock)
                {
                    //This variable should not be null, but when we click the play button the FuzzyFinder (DropDown window) does not close on 2019 and 2020
                    //but it does close on 2018.
                    //The FuzzyFinder (DropDown window) should close if you click anywhere in the Editor
                    //Once this issue is fixed this null check can be removed

                    if (tree != null)
                    {
                        GUI.Label(new Rect(0, 0, position.width, position.height), GUIContent.none, Styles.background);

                        HandleKeyboard();

                        if (tree.searchable)
                        {
                            LudiqGUI.Space(7);

                            if (letQueryClear)
                            {
                                letQueryClear = false;
                            }
                            else
                            {
                                EditorGUI.FocusTextInControl(searchFieldName);
                            }

                            var searchFieldPosition = GUILayoutUtility.GetRect(10, Styles.searchFieldHeight);
                            searchFieldPosition.x += 8;
                            searchFieldPosition.width -= 16;

                            var newQuery = OnSearchGUI(searchFieldPosition, delayedQuery ?? query);

                            if (newQuery != query || delayedQuery != null)
                            {
                                if (!isAnimating)
                                {
                                    query = delayedQuery ?? newQuery;
                                    Search();
                                    delayedQuery = null;
                                }
                                else
                                {
                                    delayedQuery = newQuery;
                                }
                            }
                        }

                        OnLevelGUI(anim, GetLevelRelative(0), GetLevelRelative(-1));

                        if (anim < 1)
                        {
                            OnLevelGUI(anim + 1, GetLevelRelative(-1), GetLevelRelative(-2));
                        }

                        if (isAnimating && e.type == EventType.Repaint)
                        {
                            anim = Mathf.MoveTowards(anim, animTarget, repaintDeltaTime * animationSpeed);

                            if (animTarget == 0 && anim == 0)
                            {
                                anim = 1;
                                animTarget = 1;
                                stack.Pop();
                            }

                            requireRepaint = true;
                        }

                        if (e.type == EventType.Repaint)
                        {
                            lastRepaintTime = DateTime.Now;
                        }

                        if (!activeParent.isLoading)
                        {
                            if (tree.searchable && hasSearch && !hasSufficientSearch)
                            {
                                EditorGUI.LabelField
                                    (
                                        new Rect(0, 0, position.width, position.height),
                                        $"Enter at least {minSearchLength} characters to search.",
                                        Styles.insufficientSearch
                                    );
                            }

                            OnFooterGUI();

                            OnPositioning();
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                if (tree.multithreaded && ex.Message.StartsWith("Getting control "))
                {
                    // A bunch of happens that might affect the GUI could happen on a
                    // secondary thread, leading to Unity complaining about the amount
                    // of controls changing between the draw call and the layout call.
                    // Because these are hamless and last just one frame, we can safely
                    // ignore them and repaint right away.
                    requireRepaint = true;
                }
                else
                {
                    throw;
                }
            }
        }

        private string OnSearchGUI(Rect position, string query)
        {
            var fieldPosition = position;
            fieldPosition.width -= 15;

            var cancelButtonPosition = position;
            cancelButtonPosition.x += position.width - 15;
            cancelButtonPosition.width = 15;

            GUI.SetNextControlName(searchFieldName);
            query = EditorGUI.TextField(fieldPosition, query, Styles.searchField);

            if (GUI.Button(cancelButtonPosition, GUIContent.none, string.IsNullOrEmpty(query) ? Styles.searchFieldCancelButtonEmpty : Styles.searchFieldCancelButton) && query != string.Empty)
            {
                query = string.Empty;
                GUIUtility.keyboardControl = 0;
                letQueryClear = true;
            }

            return query;
        }

        private void OnHeaderGUI(FuzzyOptionNode parent, Rect headerPosition)
        {
            EditorGUIUtility.SetIconSize(new Vector2(IconSize.Small, IconSize.Small));

            var headerContent = new GUIContent(parent.option.headerLabel, parent.option.showHeaderIcon ? parent.option.icon?[IconSize.Small] : null);

            headerWidth = Styles.header.CalcSize(headerContent).x;

            if (e.type == EventType.Repaint)
            {
                GUI.Label(headerPosition, headerContent, Styles.header);
            }

            EditorGUIUtility.SetIconSize(default(Vector2));
        }

        private void OnLevelGUI(float anim, FuzzyOptionNode parent, FuzzyOptionNode grandParent)
        {
            anim = Mathf.Floor(anim) + Mathf.SmoothStep(0, 1, Mathf.Repeat(anim, 1));

            var levelPosition = new Rect
                (
                position.width * (1 - anim) + 1,
                tree.searchable ? 30 : 0,
                position.width - 2,
                height - (tree.searchable ? 31 : 1)
                );

            GUILayout.BeginArea(levelPosition);

            if (grandParent != null || !string.IsNullOrEmpty(parent.option.headerLabel))
            {
                var headerPosition = GUILayoutUtility.GetRect(10, Styles.headerHeight);

                OnHeaderGUI(parent, headerPosition);

                if (grandParent != null)
                {
                    var leftArrowPosition = new Rect(headerPosition.x + 4, headerPosition.y + 7, 13, 13);

                    if (e.type == EventType.Repaint)
                    {
                        Styles.leftArrow.Draw(leftArrowPosition, false, false, false, false);
                    }

                    if (!isAnimating)
                    {
                        if (e.type == EventType.MouseDown && (e.button == (int)MouseButton.Right || headerPosition.Contains(e.mousePosition)))
                        {
                            SelectParent();
                            e.Use();

                            GUIUtility.ExitGUI();
                        }
                    }
                }
            }

            OnOptionsGUI(parent, levelPosition.height - Styles.headerHeight);

            GUILayout.EndArea();
        }

        private Vector2 lastMouseMovePosition;

        private void OnOptionsGUI(FuzzyOptionNode parent, float scrollViewHeight)
        {
            if (parent.isLoading || (tree.showBackgroundWorkerProgress && BackgroundWorker.hasProgress))
            {
                LudiqGUI.BeginVertical();
                LudiqGUI.FlexibleSpace();
                LudiqGUI.BeginHorizontal();
                LudiqGUI.FlexibleSpace();
                LudiqGUI.LoaderLayout();
                LudiqGUI.FlexibleSpace();
                LudiqGUI.EndHorizontal();

                LudiqGUI.Space(16);
                LudiqGUI.BeginHorizontal();
                LudiqGUI.Space(10);
                var progressBarPosition = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(19), GUILayout.ExpandWidth(true));

                if (progressBarPosition.width > 0 && progress > 0)
                {
                    if (tree.showBackgroundWorkerProgress && BackgroundWorker.hasProgress)
                    {
                        EditorGUI.ProgressBar(progressBarPosition, BackgroundWorker.progressProportion, BackgroundWorker.progressLabel);
                    }
                    else if (showProgress)
                    {
                        EditorGUI.ProgressBar(progressBarPosition, progress, progressText);
                    }
                }

                LudiqGUI.Space(10);
                LudiqGUI.EndHorizontal();

                LudiqGUI.FlexibleSpace();
                LudiqGUI.EndVertical();

                return;
            }

            parent.scroll = GUILayout.BeginScrollView(parent.scroll);

            EditorGUIUtility.SetIconSize(new Vector2(IconSize.Small, IconSize.Small));

            var selectedOptionPosition = default(Rect);

            if (e.type == EventType.Repaint)
            {
                minOptionWidth = 0;
            }

            foreach (var node in parent.children)
            {
                node.EnsureDrawable();

                minOptionWidth = Mathf.Max(minOptionWidth, Mathf.Min(node.width, Styles.maxOptionWidth));
            }


            for (var i = 0; i < parent.children.Count; i++)
            {
                var node = parent.children[i];

                var optionPosition = GUILayoutUtility.GetRect(IconSize.Small, Styles.optionHeight, GUILayout.ExpandWidth(true));

                if (!isAnimating)
                {
                    if (((e.type == EventType.MouseMove && GUIUtility.GUIToScreenPoint(e.mousePosition) != lastMouseMovePosition) || e.type == EventType.MouseDown) &&
                        parent.selectedIndex != i && optionPosition.Contains(e.mousePosition))
                    {
                        parent.selectedIndex = i;

                        requireRepaint = true;

                        lastMouseMovePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);

                        GUIUtility.ExitGUI();
                    }
                }

                var optionIsSelected = false;

                if (i == parent.selectedIndex)
                {
                    optionIsSelected = true;
                    selectedOptionPosition = optionPosition;
                }

                // Clipping
                if (optionPosition.yMax < parent.scroll.y || optionPosition.yMin > parent.scroll.y + scrollViewHeight)
                {
                    continue;
                }

                if (e.type == EventType.Repaint)
                {
                    node.style.Draw(optionPosition, node.label, false, false, optionIsSelected, optionIsSelected);
                }

                var right = optionPosition.xMax;

                if (node.hasChildren)
                {
                    right -= 13;
                    var rightArrowPosition = new Rect(right, optionPosition.y + 4, 13, 13);

                    if (e.type == EventType.Repaint)
                    {
                        Styles.rightArrow.Draw(rightArrowPosition, false, false, false, false);
                    }
                }

                if (!node.hasChildren && tree.selected.Contains(node.option.value))
                {
                    right -= 16;
                    var checkPosition = new Rect(right, optionPosition.y + 4, 12, 12);

                    if (e.type == EventType.Repaint)
                    {
                        Styles.check.Draw(checkPosition, false, false, false, false);
                    }
                }

                if (tree.favorites != null && tree.CanFavorite(node.option.value) && (optionIsSelected || tree.favorites.Contains(node.option.value)))
                {
                    right -= 19;
                    var starPosition = new Rect(right, optionPosition.y + 2, IconSize.Small, IconSize.Small);

                    EditorGUI.BeginChangeCheck();

                    var isFavorite = tree.favorites.Contains(node.option.value);

                    isFavorite = GUI.Toggle(starPosition, isFavorite, GUIContent.none, Styles.star);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (isFavorite)
                        {
                            tree.favorites.Add(node.option.value);
                        }
                        else
                        {
                            tree.favorites.Remove(node.option.value);
                        }

                        tree.OnFavoritesChange();

                        UpdateFavorites();
                    }
                }

                if (!isAnimating)
                {
                    if (e.type == EventType.MouseDown && e.button == (int)MouseButton.Left && optionPosition.Contains(e.mousePosition))
                    {
                        e.Use();
                        parent.selectedIndex = i;
                        SelectChild(node);

                        GUIUtility.ExitGUI();
                    }
                }
            }

            EditorGUIUtility.SetIconSize(default(Vector2));

            GUILayout.EndScrollView();

            if (scrollToSelected && e.type == EventType.Repaint)
            {
                scrollToSelected = false;

                var lastRect = GUILayoutUtility.GetLastRect();


                if (selectedOptionPosition.yMax - lastRect.height > parent.scroll.y)
                {
                    var scroll = parent.scroll;
                    scroll.y = selectedOptionPosition.yMax - lastRect.height;
                    parent.scroll = scroll;

                    requireRepaint = true;
                }

                if (selectedOptionPosition.y < parent.scroll.y)
                {
                    var scroll = parent.scroll;
                    scroll.y = selectedOptionPosition.y;
                    parent.scroll = scroll;

                    requireRepaint = true;
                }
            }
        }

        const int footerWidthMargin = 2;

        private void OnFooterGUI()
        {
            var footerPosition = new Rect
                (
                1,
                height - 1,
                position.width - footerWidthMargin,
                footerHeight
                );

            var backgroundPosition = footerPosition;
            backgroundPosition.height += 1;

            if (e.type == EventType.Repaint)
            {
                Styles.footerBackground.Draw(backgroundPosition, false, false, false, false);
            }

            if (activeNode != null && activeNode.option.hasFooter)
            {
                activeNode.option.OnFooterGUI(footerPosition);
            }
        }

        private void HandleKeyboard()
        {
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.DownArrow)
                {
                    activeSelectedIndex = Mathf.Clamp(activeSelectedIndex + 1, 0, activeNodes.Count - 1);
                    scrollToSelected = true;
                    e.Use();
                }
                else if (e.keyCode == KeyCode.UpArrow)
                {
                    activeSelectedIndex = Mathf.Clamp(activeSelectedIndex - 1, 0, activeNodes.Count);
                    scrollToSelected = true;
                    e.Use();
                }
                else if ((e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter) && !activeParent.isLoading)
                {
                    SelectChild(activeNode);
                    e.Use();
                }
                else if ((e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.Backspace) && activeParent != activeRoot)
                {
                    SelectParent();
                    e.Use();
                }
                else if (e.keyCode == KeyCode.RightArrow && !activeParent.isLoading)
                {
                    EnterChild(activeNode);
                    e.Use();
                }
                else if (e.keyCode == KeyCode.Escape)
                {
                    Close();
                    e.Use();
                }
            }
        }

        #endregion

        #region Threading

        private readonly object guiLock = new object();

        private void ExecuteTask(Action task)
        {
            Ensure.That(nameof(task)).IsNotNull(task);

            if (!tree.multithreaded)
            {
                task();
                ClearProgressBar();
                return;
            }

            lock (queue)
            {
                queue.Enqueue(task);
            }

            if (workerThread == null)
            {
                workerThread = new Thread(Work);
                workerThread.Name = "Fuzzy Window";
                workerThread.Start();
            }
        }

        private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();
        private Thread workerThread;

        private void Work()
        {
            while (true)
            {
                if (!queue.TryDequeue(out var task))
                {
                    break;
                }

                try
                {
                    task();
                }
                catch (OperationCanceledException) { }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                finally
                {
                    ClearProgressBar();
                    requireRepaint = true;
                }
            }

            workerThread = null;
        }

        private string progressText;
        private float progress;
        private bool showProgress;

        public void DisplayProgressBar(string text, float progress)
        {
            progressText = text;
            this.progress = progress;
            showProgress = true;
        }

        public void DisplayProgressBar(float progress)
        {
            DisplayProgressBar(null, progress);
        }

        public static void ClearProgressBar()
        {
            if (instance == null)
            {
                return;
            }

            instance.progressText = null;
            instance.progress = 0;
            instance.showProgress = false;
        }

        #endregion
    }
}
