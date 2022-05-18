#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
#if UNITY_EDITOR
using CZToolKit.Core.Editors;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.GraphProcessor.Editors
{
    [CustomObjectEditor(typeof(BaseGraphView))]
    public class BaseGraphInspector : ObjectEditor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();

        PropertyTree propertyTree;

        public override void OnEnable()
        {
            var view = Target as BaseGraphView;
            if (view.Model != null)
                propertyTree = PropertyTree.Create(view.Model);
        }

        public override void OnInspectorGUI()
        {
            var view = Target as BaseGraphView;
            if (view == null || view.Model == null)
                return;

            if (!ContextDataCache.TryGetContextData<GUIStyle>("BigLabel", out var bigLabel))
            {
                bigLabel.value = new GUIStyle(GUI.skin.label);
                bigLabel.value.fontSize = 18;
                bigLabel.value.fontStyle = FontStyle.Bold;
                bigLabel.value.alignment = TextAnchor.MiddleLeft;
                bigLabel.value.stretchWidth = true;
            }

            EditorGUILayoutExtension.BeginVerticalBoxGroup();
            GUILayout.Label(string.Concat("Nodes：", view.Model.Nodes.Count), bigLabel.value);
            GUILayout.Label(string.Concat("Connections：", view.Model.Connections.Count), bigLabel.value);
            EditorGUILayoutExtension.EndVerticalBoxGroup();

            if (propertyTree == null)
                return;
            propertyTree.BeginDraw(false);
            foreach (var property in propertyTree.EnumerateTree(false, true))
            {
                EditorGUI.BeginChangeCheck();
                property.Draw();
                if (EditorGUI.EndChangeCheck() && view.Model.TryGetValue(property.Name, out var bindableProperty))
                    bindableProperty.SetValueWithNotify(property.ValueEntry.WeakSmartValue);
            }
            propertyTree.EndDraw();
        }
    }
}
#endif