///MIT License
///Copyright(c) 2019 InnoGames GmbH

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityAtoms.Editor
{
    // Do not run this on the server because it could slow down tests and builds:
#if !CONTINUOUS_INTEGRATION

    /// <summary>
    /// This class draws additional columns into the project window.
    /// </summary>
    [InitializeOnLoad]
    public static class ProjectWindowDetails
    {
        private static readonly List<ProjectWindowDetailBase> _details = new List<ProjectWindowDetailBase>();
        private static readonly ProjectWindowDetailsData detailsData;
        private static GUIStyle _rightAlignedStyle;

        private const int SPACE_BETWEEN_COLUMNS = 10;
        private const int MENU_ICON_WIDTH = 20;

        static ProjectWindowDetails()
        {
            EditorApplication.projectWindowItemOnGUI += DrawAssetDetails;

            detailsData = ProjectWindowDetailsData.LoadSettings();

            foreach (var type in GetAllDetailTypes())
            {
                ProjectWindowDetailBase lastValue = (ProjectWindowDetailBase)Activator.CreateInstance(type);
                _details.Add(lastValue);
                lastValue.Visible = detailsData.GetVisible(lastValue.Name);
            }
        }

        private static IEnumerable<Type> GetAllDetailTypes()
        {
            // Get all classes that inherit from ProjectViewDetailBase:
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.BaseType == typeof(ProjectWindowDetailBase))
                {
                    yield return type;
                }
            }
        }

        private static void DrawAssetDetails(string guid, Rect rect)
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (!IsMainListAsset(rect))
            {
                return;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                return;
            }

            if (Event.current.type == EventType.MouseDown &&
                Event.current.button == 0 &&
                Event.current.mousePosition.x > rect.xMax - MENU_ICON_WIDTH)
            {
                Event.current.Use();
                ShowContextMenu(new Rect(Event.current.mousePosition, Vector2.zero));
            }

            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            var isSelected = Array.IndexOf(Selection.assetGUIDs, guid) >= 0;

            // Right align label and leave some space for the menu icon:
            rect.x += rect.width;
            rect.x -= MENU_ICON_WIDTH;
            rect.width = MENU_ICON_WIDTH;

            if (isSelected)
            {
                DrawMenuIcon(rect);
            }

            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (asset == null)
            {
                // this entry could be Favourites or Packages. Ignore it.
                return;
            }

            for (var i = _details.Count - 1; i >= 0; i--)
            {
                var detail = _details[i];
                if (!detail.Visible)
                {
                    continue;
                }

                string label = detail.GetLabel(guid, assetPath, asset);

                if (!string.IsNullOrEmpty(detail.GetLabel(guid, assetPath, asset)))
                {
                    rect.width = detail.ColumnWidth;
                    rect.x -= detail.ColumnWidth + SPACE_BETWEEN_COLUMNS;
                    GUI.Label(rect, new GUIContent(label, detail.Name),
                        GetStyle(detail.Alignment));
                }
            }
        }
        private static void DrawMenuIcon(Rect rect)
        {
            var icon = EditorGUIUtility.IconContent("_Menu");
            EditorGUI.LabelField(rect, icon);
        }

        private static GUIStyle GetStyle(TextAlignment alignment)
        {
            return alignment == TextAlignment.Left ? EditorStyles.label : RightAlignedStyle;
        }

        private static GUIStyle RightAlignedStyle
        {
            get
            {
                if (_rightAlignedStyle == null)
                {
                    _rightAlignedStyle = new GUIStyle(EditorStyles.label);
                    _rightAlignedStyle.alignment = TextAnchor.MiddleRight;
                }

                return _rightAlignedStyle;
            }
        }

        private static void ShowContextMenu(Rect rect = default)
        {
            var menu = new GenericMenu();
            foreach (var detail in _details)
            {
                menu.AddItem(new GUIContent(detail.Name), detail.Visible, ToggleMenu, detail);
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("All"), false, ShowAllDetails);
            menu.AddItem(new GUIContent("None"), false, HideAllDetails);
            if(rect == default)
            {
                menu.DropDown(default);
            }
            else
            {
                menu.DropDown(rect);
            }
        }

        private static void HideAllDetails()
        {
            foreach (var detail in _details)
            {
                detail.Visible = false;
                detailsData.SetValueOrCreateNew(detail.Name, detail.Visible);
            }
            ProjectWindowDetailsData.SaveSettings(detailsData);
        }

        private static void ShowAllDetails()
        {
            foreach (var detail in _details)
            {
                detail.Visible = true;
                detailsData.SetValueOrCreateNew(detail.Name, detail.Visible);
            }
            ProjectWindowDetailsData.SaveSettings(detailsData);
        }

        private static void ToggleMenu(object data)
        {
            var detail = (ProjectWindowDetailBase)data;
            detail.Visible = !detail.Visible;
            detailsData.SetValueOrCreateNew(detail.Name, detail.Visible);
            ProjectWindowDetailsData.SaveSettings(detailsData);
        }


        private static bool IsMainListAsset(Rect rect)
        {
            // Don't draw details if project view shows large preview icons:
            if (rect.height > 20)
            {
                return false;
            }
            // Don't draw details if this asset is a sub asset:
            if (rect.x > 16)
            {
                return false;
            }
            return true;
        }
    }
#endif
}
