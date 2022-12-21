//https://github.com/marijnz/unity-toolbar-extender

using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace online.kamishiro.externaltoolslauncher
{
    public static class ToolbarCallback
    {
        private static Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static Type m_guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
        private static Type m_iWindowBackendType = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");
        private static PropertyInfo m_windowBackend = m_guiViewType.GetProperty("windowBackend", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static PropertyInfo m_viewVisualTree = m_iWindowBackendType.GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
        private static PropertyInfo m_viewVisualTree = m_guiViewType.GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        private static FieldInfo m_imguiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static ScriptableObject m_currentToolbar;

        /// <summary>
        /// Callback for toolbar OnGUI method.
        /// </summary>
        public static Action OnToolbarGUI;
        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (m_currentToolbar == null)
            {
                // Find toolbar
                UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
                m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (m_currentToolbar != null)
                {
#if UNITY_2020_1_OR_NEWER
                    FieldInfo root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    object rawRoot = root.GetValue(m_currentToolbar);
                    VisualElement mRoot = rawRoot as VisualElement;
                    RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
                    RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);

                    void RegisterCallback(string root, Action cb)
                    {
                        VisualElement toolbarZone = mRoot.Q(root);

                        VisualElement parent = new VisualElement()
                        {
                            style = {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                        };
                        IMGUIContainer container = new IMGUIContainer();
                        container.style.flexGrow = 1;
                        container.onGUIHandler += () =>
                        {
                            cb?.Invoke();
                        };
                        parent.Add(container);
                        toolbarZone.Add(parent);
                    }
#else
#if UNITY_2020_1_OR_NEWER
                    object windowBackend = m_windowBackend.GetValue(m_currentToolbar);

                    // Get it's visual tree
                    VisualElement visualTree = (VisualElement)m_viewVisualTree.GetValue(windowBackend, null);
#else
                    // Get it's visual tree
                    VisualElement visualTree = (VisualElement)m_viewVisualTree.GetValue(m_currentToolbar, null);
#endif

                    // Get first child which 'happens' to be toolbar IMGUIContainer
                    IMGUIContainer container = (IMGUIContainer)visualTree[0];

                    // (Re)attach handler
                    Action handler = (Action)m_imguiContainerOnGui.GetValue(container);
                    handler -= OnGUI;
                    handler += OnGUI;
                    m_imguiContainerOnGui.SetValue(container, handler);

#endif
                }
            }
        }

        private static void OnGUI()
        {
            Action handler = OnToolbarGUI;
            if (handler != null) handler();
        }
    }
}